using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.CodeGen;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS0601", "SS0602", "SS0603")]
	public sealed partial class TypePatternAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				static context =>
				{
					CheckSS0601AndSS0603(context);
					CheckSS0602(context);
				},
				new[]
				{
					SyntaxKind.LogicalNotExpression,
					SyntaxKind.IsExpression,
					SyntaxKind.IsPatternExpression
				}
			);
		}


		private static void CheckSS0601AndSS0603(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, _, originalNode) = context;
			switch (originalNode)
			{
				// obj is T
				case BinaryExpressionSyntax
				{
					RawKind: (int)SyntaxKind.IsExpression,
					Left: var expr,
					Right: var type
				} node:
				{
					switch (type)
					{
						case PredefinedTypeSyntax { Keyword: { RawKind: (int)SyntaxKind.ObjectKeyword } }:
						{
							context.ReportDiagnostic(
								Diagnostic.Create(
									descriptor: SS0603,
									location: node.GetLocation(),
									messageArgs: null,
									additionalLocations: new[] { expr.GetLocation() }
								)
							);

							break;
						}
						//case PredefinedTypeSyntax:
						//{
						//	// CS0183 has already covered this case (SS0601). Please visit
						//	//
						//	//     https://docs.microsoft.com/en-us/dotnet/csharp/misc/cs0183
						//	//
						//	// for more information.
						//	return;
						//}
						case TypeSyntax:
						{
							if (semanticModel.GetOperation(expr) is not { Type: var definedType })
							{
								return;
							}

							if (
								semanticModel.GetOperation(node) is not IIsTypeOperation
								{
									TypeOperand: { } matchedType
								}
							)
							{
								return;
							}

							if (!SymbolEqualityComparer.Default.Equals(definedType, matchedType))
							{
								return;
							}

							context.ReportDiagnostic(
								Diagnostic.Create(
									descriptor: SS0601,
									location: type.GetLocation(),
									messageArgs: null
								)
							);

							break;
						}
					}

					break;
				}

				// obj is T variable
				// obj is object _
				case IsPatternExpressionSyntax
				{
					Expression: var expression,
					Pattern: DeclarationPatternSyntax { Type: var type, Designation: var designation } pattern
				} node:
				{
					switch (type)
					{
						case PredefinedTypeSyntax
						{
							Parent: { Parent: not ParenthesizedExpressionSyntax },
							Keyword: { RawKind: (int)SyntaxKind.ObjectKeyword }
						}
						when designation is DiscardDesignationSyntax:
						{
							context.ReportDiagnostic(
								Diagnostic.Create(
									descriptor: SS0603,
									location: node.GetLocation(),
									messageArgs: null
								)
							);

							break;
						}
						default:
						{
							if (semanticModel.GetOperation(expression) is not { Type: var definedType })
							{
								return;
							}

							if (
								semanticModel.GetOperation(pattern) is not IDeclarationPatternOperation
								{
									MatchedType: { } matchedType
								}
							)
							{
								return;
							}

							if (!SymbolEqualityComparer.Default.Equals(definedType, matchedType))
							{
								return;
							}

							context.ReportDiagnostic(
								Diagnostic.Create(
									descriptor: SS0601,
									location: type.GetLocation(),
									messageArgs: null,
									additionalLocations: new[]
									{
										pattern.GetLocation(),
										designation.GetLocation()
									}
								)
							);

							break;
						}
					}

					break;
				}
			}
		}

		private static void CheckSS0602(SyntaxNodeAnalysisContext context)
		{
			switch (context.Node)
			{
				// !(o is object)
				case PrefixUnaryExpressionSyntax
				{
					Operand: ParenthesizedExpressionSyntax
					{
						Expression: BinaryExpressionSyntax
						{
							RawKind: (int)SyntaxKind.IsExpression,
							Left: var expr,
							Right: PredefinedTypeSyntax { Keyword: { RawKind: (int)SyntaxKind.ObjectKeyword } }
						} expression
					}
				} node:
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0602,
							location: node.GetLocation(),
							messageArgs: null,
							additionalLocations: new[] { expr.GetLocation() }
						)
					);

					break;
				}

				// o is not object
				case IsPatternExpressionSyntax
				{
					Expression: var expr,
					Pattern: UnaryPatternSyntax
					{
						RawKind: (int)SyntaxKind.NotPattern,
						Pattern: TypePatternSyntax
						{
							Type: PredefinedTypeSyntax { Keyword: { RawKind: (int)SyntaxKind.ObjectKeyword } }
						}
					}
				} pattern:
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0602,
							location: pattern.GetLocation(),
							messageArgs: null,
							additionalLocations: new[] { expr.GetLocation() }
						)
					);

					break;
				}
			}
		}
	}
}
