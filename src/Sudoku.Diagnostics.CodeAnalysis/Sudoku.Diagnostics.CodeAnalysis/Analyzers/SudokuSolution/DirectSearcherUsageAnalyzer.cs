using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.CodeGenerating;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SD0404")]
public sealed partial class DirectSearcherUsageAnalyzer : DiagnosticAnalyzer
{
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

		Func<ISymbol?, ISymbol?, bool> f = SymbolEqualityComparer.Default.Equals;
		Func<string, INamedTypeSymbol?> c = compilation.GetTypeByMetadataName;
		var attributeSymbol = c("Sudoku.Solving.Manual.DirectSearcherAttribute")!;
		var stepSearcherSymbol = c("Sudoku.Solving.Manual.StepSearcher");
		switch (operation)
		{
			case ILocalFunctionOperation { Body: { Locals: { Length: var length and not 0 } locals } }:
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

		bool hasMarkedDirectSearcherAttribute(INamedTypeSymbol localType, ISymbol attributeSymbol)
		{
			var attributes = localType.GetAttributes();
			foreach (var attribute in attributes)
			{
				if (!f(attribute.AttributeClass, attributeSymbol))
				{
					continue;
				}

				var namedArgs = attribute.NamedArguments;
				return namedArgs.Length == 1
					&& namedArgs[0] is var pair
					&& pair.Key == "IsAllow"
					&& pair.Value.Value is true
					|| namedArgs.Length == 0;
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
					&& f(staticTypeSymbol, c("Sudoku.Solving.Manual.FastProperties"))
					&& arguments[0] is { Expression: var argExpr }
					&& semanticModel.GetOperation(argExpr) is { Type: var argTypeSymbol }
					&& f(argTypeSymbol, c("Sudoku.Data.SudokuGrid")):
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
