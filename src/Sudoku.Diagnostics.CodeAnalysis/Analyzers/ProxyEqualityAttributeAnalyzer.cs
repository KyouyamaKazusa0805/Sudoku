namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[Generator]
[CodeAnalyzer("SD0406", "SD0407", "SD0408", "SD0409", "SD0410")]
public sealed partial class ProxyEqualityAttributeAnalyzer : ISourceGenerator
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

			if (
				originalNode is not MethodDeclarationSyntax
				{
					AttributeLists: { Count: not 0 } attributes,
					Modifiers: var modifiers,
					ReturnType: var returnType,
					ParameterList.Parameters: { Count: var count } parameters,
					Identifier: var identifier
				} node
			)
			{
				return;
			}

			if (semanticModel.GetDeclaredSymbol(originalNode) is not IMethodSymbol methodSymbol)
			{
				return;
			}

			var attributeSymbol = compilation.GetTypeByMetadataName(TypeNames.ProxyEqualityAttribute);
			if (
				methodSymbol.GetAttributes().All(
					attribute => !SymbolEqualityComparer.Default.Equals(
						attribute.AttributeClass,
						attributeSymbol
					)
				)
			)
			{
				return;
			}

			checkSD0406(modifiers, identifier);
			checkSD0407(returnType, semanticModel, compilation);
			checkSD0408(node, parameters, semanticModel);
			checkSD0409(node, attributeSymbol, semanticModel);
			checkSD0410(identifier, count);


			void checkSD0406(SyntaxTokenList modifiers, SyntaxToken identifier)
			{
				if (modifiers.Any(static modifier => modifier.RawKind == (int)SyntaxKind.StaticKeyword))
				{
					return;
				}

				DiagnosticList.Add(
					Diagnostic.Create(
						descriptor: SD0406,
						location: identifier.GetLocation(),
						messageArgs: null
					)
				);
			}

			void checkSD0407(TypeSyntax returnType, SemanticModel semanticModel, Compilation compilation)
			{
				if (
					SymbolEqualityComparer.Default.Equals(
						compilation.GetSpecialType(SpecialType.System_Boolean),
						semanticModel.GetSymbolInfo(returnType, CancellationToken).Symbol
					)
				)
				{
					return;
				}

				DiagnosticList.Add(
					Diagnostic.Create(
						descriptor: SD0407,
						location: returnType.GetLocation(),
						messageArgs: null
					)
				);
			}

			void checkSD0408(
				MethodDeclarationSyntax node,
				SeparatedSyntaxList<ParameterSyntax> parameters,
				SemanticModel semanticModel
			)
			{
				if (node.Parent is not TypeDeclarationSyntax type)
				{
					return;
				}

				if (semanticModel.GetDeclaredSymbol(type, CancellationToken) is not { } typeSymbol)
				{
					return;
				}

				foreach (var parameter in parameters)
				{
					if (parameter.Type is not { } typeNodeToCheck)
					{
						// Parameter can't empty its type.

						continue;
					}

					var typeToCheck = semanticModel.GetSymbolInfo(typeNodeToCheck, CancellationToken).Symbol;
					if (SymbolEqualityComparer.Default.Equals(typeSymbol, typeToCheck))
					{
						continue;
					}

					DiagnosticList.Add(
						Diagnostic.Create(
							descriptor: SD0408,
							location: parameter.GetLocation(),
							messageArgs: null
						)
					);
				}
			}

			void checkSD0409(
				MethodDeclarationSyntax node,
				INamedTypeSymbol? attributeSymbol,
				SemanticModel semanticModel
			)
			{
				if (node.Parent is not TypeDeclarationSyntax type)
				{
					return;
				}

				if (semanticModel.GetDeclaredSymbol(type, CancellationToken) is not { } typeSymbol)
				{
					return;
				}

				if (
					typeSymbol.GetMembers().OfType<IMethodSymbol>().Count(
						method => method.GetAttributes().Any(
							attribute => SymbolEqualityComparer.Default.Equals(
								attribute.AttributeClass,
								attributeSymbol
							)
						)
					) == 1
				)
				{
					return;
				}

				DiagnosticList.Add(
					Diagnostic.Create(
						descriptor: SD0409,
						location: type.Identifier.GetLocation(),
						messageArgs: null
					)
				);
			}

			void checkSD0410(SyntaxToken identifier, int count)
			{
				if (count == 2)
				{
					return;
				}

				DiagnosticList.Add(
					Diagnostic.Create(
						descriptor: SD0410,
						location: identifier.GetLocation(),
						messageArgs: null
					)
				);
			}
		}
	}
}
