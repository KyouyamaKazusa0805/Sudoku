namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[Generator]
[CodeAnalyzer("SD0411", "SD0412")]
public sealed partial class RegexAttributeAnalyzer : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		foreach (var diagnostic in ((CodeAnalyzer)context.SyntaxContextReceiver!).DiagnosticList)
		{
			context.ReportDiagnostic(diagnostic);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) =>
		context.RegisterForSyntaxNotifications(() => new CodeAnalyzer(context.CancellationToken));


	/// <summary>
	/// Defines the syntax receiver.
	/// </summary>
	private sealed class CodeAnalyzer : IAnalyzer
	{
		/// <summary>
		/// Initializes a <see cref="CodeAnalyzer"/> instance via the specified cancellation token.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
		public CodeAnalyzer(CancellationToken cancellationToken) => CancellationToken = cancellationToken;


		/// <inheritdoc/>
		public CancellationToken CancellationToken { get; }

		/// <inheritdoc/>
		public IList<Diagnostic> DiagnosticList { get; } = new List<Diagnostic>();


		/// <inheritdoc/>
		public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
		{
			if (
				context is not
				{
					Node: var originalNode,
					SemanticModel: { Compilation: var compilation } semanticModel
				}
			)
			{
				return;
			}

			var symbol = semanticModel.GetSymbolInfo(originalNode, CancellationToken).Symbol;
			if (symbol is not INamedTypeSymbol namedType)
			{
				return;
			}

			var attribute = compilation.GetTypeByMetadataName(TypeNames.RegexAttribute);
			if (attribute is null)
			{
				return;
			}

			var stringSymbol = compilation.GetSpecialType(SpecialType.System_String);
			foreach (var field in namedType.GetMembers().OfType<IFieldSymbol>())
			{
				var attributesData = field.GetAttributes();
				if (attributesData.Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
				{
					continue;
				}

				checkSD0411(field);
				checkSD0412(field);
			}


			void checkSD0411(IFieldSymbol field)
			{
				if (SymbolEqualityComparer.Default.Equals(field.Type, stringSymbol))
				{
					return;
				}

				var fieldSyntax = field.DeclaringSyntaxReferences[0].GetSyntax(CancellationToken);
				DiagnosticList.Add(
					Diagnostic.Create(
						descriptor: SD0411,
						location: fieldSyntax.GetLocation(),
						messageArgs: null
					)
				);
			}

			void checkSD0412(IFieldSymbol field)
			{
				if (field is not { HasConstantValue: true, ConstantValue: string value })
				{
					return;
				}

				try
				{
					Regex.Match(string.Empty, value, RegexOptions.ExplicitCapture, TimeSpan.FromSeconds(5));
				}
				catch (Exception ex) when (ex is ArgumentException or RegexMatchTimeoutException)
				{
					var fieldSyntax = field.DeclaringSyntaxReferences[0].GetSyntax(CancellationToken);
					DiagnosticList.Add(
						Diagnostic.Create(
							descriptor: SD0412,
							location: fieldSyntax.GetLocation(),
							messageArgs: null
						)
					);
				}
			}
		}
	}
}
