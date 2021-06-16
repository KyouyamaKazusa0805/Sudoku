using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.CodeGen;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SD0404")]
	public sealed partial class DirectSearcherUsageAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// Indicates the type name of the <c>StepSearcher</c>.
		/// </summary>
		private const string StepSearcherTypeName = "Sudoku.Solving.Manual.StepSearcher";

		/// <summary>
		/// Indicates the type name of the <c>DirectSearcherAttribute</c>.
		/// </summary>
		private const string AttributeTypeName = "Sudoku.Solving.Manual.DirectSearcherAttribute";

		/// <summary>
		/// Indicates the type name of the <c>FastProperties</c>.
		/// </summary>
		private const string FastPropertiesTypeName = "Sudoku.Solving.Manual.FastProperties";

		/// <summary>
		/// Indicates the type name of the <c>SudokuGrid</c>.
		/// </summary>
		private const string SudokuGridTypeName = "Sudoku.Data.SudokuGrid";


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterOperationAction(
				AnalyzeOperation,
				new[] { OperationKind.LocalFunction, OperationKind.MethodBody }
			);
		}


		private static void AnalyzeOperation(OperationAnalysisContext context)
		{
			var (compilation, operation) = context;

			// Check whether the semantic model exists.
			if (operation.SemanticModel is not { } semanticModel)
			{
				return;
			}

			var attributeSymbol = compilation.GetTypeByMetadataName(AttributeTypeName)!;
			var stepSearcherSymbol = compilation.GetTypeByMetadataName(StepSearcherTypeName);
			switch (operation)
			{
				/*length-pattern*/
				case ILocalFunctionOperation { Body: { Locals: { Length: var length and not 0 } locals } }:
				{
					checkAndReportLocal(locals, length == 1);

					break;
				}

				case IMethodBodyOperation
				{
					BlockBody: var blockBody,
					ExpressionBody: var exprBody
				}:
				{
					switch ((blockBody, exprBody))
					{
						/*length-pattern*/
						case ({ Locals: { Length: var length and not 0 } blockBodyLocals }, _):
						{
							checkAndReportLocal(blockBodyLocals, length == 1);

							break;
						}
						/*length-pattern*/
						case (_, { Locals: { Length: not 0 } exprBodyLocals }):
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
					if (local is not { Type: INamedTypeSymbol localType })
					{
						continue;
					}

					if (!localType.DerivedFrom(stepSearcherSymbol))
					{
						continue;
					}

					if (hasMarkedDirectSearcherAttribute(localType, attributeSymbol))
					{
						continue;
					}

					var location = local.Locations[0];
					if (isExpressionBody)
					{
						// Only contains a variable declaration.
						context.ReportDiagnostic(
							Diagnostic.Create(
								descriptor: SD0404,
								location: location,
								messageArgs: null
							)
						);

						return;
					}

					// Check invocation 'FastProperties.InitializeMaps(grid)'.
					var rootNode = semanticModel.SyntaxTree.GetRoot(context.CancellationToken);
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
						case LocalFunctionStatementSyntax { Body: { Statements: { Count: not 1 } statements } }
						when containsThatMethodInvocation(statements, localIdentifier):
						{
							// Only find one valid invocation is okay.
							return;
						}
						case MethodDeclarationSyntax { Body: { Statements: { Count: not 1 } statements } }
						when containsThatMethodInvocation(statements, localIdentifier):
						{
							// Only find one valid invocation is okay.
							return;
						}
					}

					// Report diagnostic result.
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SD0404,
							location: location,
							messageArgs: null
						)
					);
				}
			}

			static bool hasMarkedDirectSearcherAttribute(INamedTypeSymbol localType, ISymbol attributeSymbol)
			{
				var attributes = localType.GetAttributes();
				foreach (var attribute in attributes)
				{
					if (!SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeSymbol))
					{
						continue;
					}

					/*length-pattern*/
					if (attribute.ConstructorArguments is not { Length: not 0 } arguments)
					{
						continue;
					}

					/*slice-pattern*/
					if (arguments[0] is { Value: true })
					{
						continue;
					}

					return true;
				}

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
										Identifier: { ValueText: "FastProperties" }
									} typeNode,
									Name: { Identifier: { ValueText: "InitializeMaps" } }
								},
								ArgumentList: { Arguments: { Count: 1 } arguments }
							}
						}
						when semanticModel.GetSymbolInfo(typeNode) is { Symbol: var staticTypeSymbol }
						&& SymbolEqualityComparer.Default.Equals(staticTypeSymbol, c(FastPropertiesTypeName))
						/*slice-pattern*/
						&& arguments[0] is { Expression: var argExpr }
						&& semanticModel.GetOperation(argExpr) is { Type: var argTypeSymbol }
						&& SymbolEqualityComparer.Default.Equals(argTypeSymbol, c(SudokuGridTypeName)):
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


						INamedTypeSymbol? c(string name) => compilation.GetTypeByMetadataName(name);
					}
				}

				throw new InvalidOperationException("The current collection contains no valid elements to check.");
			}
		}
	}
}
