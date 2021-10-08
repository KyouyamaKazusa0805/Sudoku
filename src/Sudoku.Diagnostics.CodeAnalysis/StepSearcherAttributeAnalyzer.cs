namespace Sudoku.Diagnostics.CodeAnalysis;

[Generator]
[CodeAnalyzer("SD0403")]
public sealed partial class StepSearcherAttributeAnalyzer : ISourceGenerator
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

			var attributeSymbol = compilation.GetTypeByMetadataName(TypeNames.StepSearcherAttribute);
			switch (originalNode)
			{
				case RecordDeclarationSyntax { Identifier: { ValueText: var recordName } identifier }
				when semanticModel.GetDeclaredSymbol(originalNode, CancellationToken)! is var typeSymbol:
				{
					var attributesData = typeSymbol.GetAttributes();
					if (attributesData.Length == 0)
					{
						break;
					}

					if (attributesData.All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol)))
					{
						break;
					}

					DiagnosticList.Add(
						Diagnostic.Create(
							descriptor: SD0403,
							location: identifier.GetLocation(),
							messageArgs: new[] { recordName }
						)
					);

					break;
				}

				case ClassDeclarationSyntax { Identifier: { ValueText: var className } identifier }
				when semanticModel.GetDeclaredSymbol(originalNode)! is var typeSymbol:
				{
					var attributeList = typeSymbol.GetAttributes();
					if (attributeList.Length == 0)
					{
						break;
					}

					if (attributeList.All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol)))
					{
						break;
					}

					var currentSymbol = (INamedTypeSymbol)semanticModel.GetDeclaredSymbol(originalNode)!;
					var stepSearcherSymbol = compilation.GetTypeByMetadataName(TypeNames.IStepSearcher)!;
					if (currentSymbol.DerivedFrom(stepSearcherSymbol))
					{
						break;
					}

					DiagnosticList.Add(
						Diagnostic.Create(
							descriptor: SD0403,
							location: identifier.GetLocation(),
							messageArgs: new[] { className }
						)
					);

					break;
				}
			}
		}
	}
}
