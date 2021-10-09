namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[Generator]
[CodeAnalyzer("SD0404")]
public sealed partial class StepSearcherAttributeUsageAnalyzer : ISourceGenerator
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
				context is not (
					var originalNode,
					semanticModel: { Compilation: var compilation } semanticModel,
					var operation
				)
			)
			{
				return;
			}

			var attributeSymbol = compilation.GetTypeByMetadataName(TypeNames.StepSearcherAttribute)!;
			var stepSearcherSymbol = compilation.GetTypeByMetadataName(TypeNames.IStepSearcher);
			switch (operation)
			{
				case ILocalFunctionOperation { Body.Locals: { Length: var length and not 0 } locals }:
				{
					checkAndReportLocal(locals, length == 1);

					break;
				}

				case IMethodBodyOperation { BlockBody: var blockBody, ExpressionBody: var exprBody }:
				{
					switch ((BlockBody: blockBody, ExpressionBody: exprBody))
					{
						case (BlockBody: { Locals: { Length: var length and not 0 } blockBodyLocals }, _):
						{
							checkAndReportLocal(blockBodyLocals, length == 1);

							break;
						}
						case (_, ExpressionBody: { Locals: { Length: not 0 } exprBodyLocals }):
						{
							checkAndReportLocal(exprBodyLocals, true);

							break;
						}
					}

					break;
				}
			}


			void checkAndReportLocal(ImmutableArray<ILocalSymbol> locals, bool isExpressionBody)
			{
				foreach (var local in locals)
				{
					if (local is not { Type: INamedTypeSymbol { AllInterfaces: var allInterfaces } localType })
					{
						continue;
					}

					if (!allInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, stepSearcherSymbol)))
					{
						continue;
					}

					if (attributeIsDirectIsTrue(localType, attributeSymbol))
					{
						continue;
					}

					var location = local.Locations[0];
					if (isExpressionBody)
					{
						// Only contains a variable declaration.
						DiagnosticList.Add(
							Diagnostic.Create(
								descriptor: SD0404,
								location: location,
								messageArgs: null
							)
						);

						return;
					}

					// Check invocation 'FastProperties.InitializeMaps(grid)'.
					var rootNode = semanticModel.SyntaxTree.GetRoot(CancellationToken);
					if (
						rootNode.FindToken(location.SourceSpan.Start) is not
						{
							RawKind: (int)SyntaxKind.IdentifierToken,
							Parent: VariableDeclaratorSyntax
							{
								Parent: VariableDeclarationSyntax
								{
									Parent: LocalDeclarationStatementSyntax
									{
										Parent:
										{
											Parent: var possibleMethodNode and (
												LocalFunctionStatementSyntax or MethodDeclarationSyntax
											)
										} and (BlockSyntax or ArrowExpressionClauseSyntax)
									}
								}
							}
						} localIdentifier
					)
					{
						continue;
					}

					switch (possibleMethodNode)
					{
						case LocalFunctionStatementSyntax { Body.Statements: { Count: not 1 } statements }
						when containsThatMethodInvocation(statements, localIdentifier):
						{
							// Only find one valid invocation is okay.
							return;
						}
						case MethodDeclarationSyntax { Body.Statements: { Count: not 1 } statements }
						when containsThatMethodInvocation(statements, localIdentifier):
						{
							// Only find one valid invocation is okay.
							return;
						}
					}

					// Report diagnostic result.
					DiagnosticList.Add(
						Diagnostic.Create(
							descriptor: SD0404,
							location: location,
							messageArgs: null
						)
					);
				}
			}

			bool attributeIsDirectIsTrue(INamedTypeSymbol localType, ISymbol attributeSymbol)
			{
				foreach (var attributeData in localType.GetAttributes())
				{
					// The attribute isn't StepSearcherAttribute.
					if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, attributeSymbol))
					{
						// Return the value.
						return attributeData.TryGetNamedArgument(MemberNames.IsDirect, out var result)
							&& ((bool?)result.Value ?? false);
					}
				}

				// The attribute isn't marked.
				return false;
			}

			bool containsThatMethodInvocation(SyntaxList<StatementSyntax> statements, SyntaxToken local)
			{
				foreach (var statement in statements)
				{
					switch (statement)
					{
						case ExpressionStatementSyntax
						{
							Expression: InvocationExpressionSyntax
							{
								Expression: MemberAccessExpressionSyntax
								{
									RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
									Expression: IdentifierNameSyntax
									{
										Identifier.ValueText: MemberNames.FastProperties
									} typeNode,
									Name.Identifier.ValueText: MemberNames.InitializeMaps
								},
								ArgumentList.Arguments: { Count: 1 } arguments
							}
						}
						when semanticModel.GetSymbolInfo(typeNode) is { Symbol: var staticTypeSymbol }
						&& SymbolEqualityComparer.Default.Equals(staticTypeSymbol, compilation.GetTypeByMetadataName(TypeNames.FastProperties))
						&& arguments[0] is { Expression: var argExpr }
						&& semanticModel.GetOperation(argExpr) is { Type: var argTypeSymbol }
						&& (
							SymbolEqualityComparer.Default.Equals(argTypeSymbol, compilation.GetTypeByMetadataName(TypeNames.Grid))
							|| SymbolEqualityComparer.Default.Equals(argTypeSymbol, compilation.GetTypeByMetadataName(TypeNames.SudokuGrid))
						):
						{
							return true;
						}

						case LocalDeclarationStatementSyntax
						{
							Declaration: VariableDeclarationSyntax { Variables: { Count: not 0 } variables }
						}
						when local.ValueText is var i && variables.Any(v => v.Identifier.ValueText == i):
						{
							return false;
						}
					}
				}

				throw new InvalidOperationException("The current collection contains no valid elements to check.");
			}
		}
	}
}
