using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Sudoku.CodeGenerating;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS0627")]
	public sealed partial class AvailableLengthPatternAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				AnalyzeSyntaxNode,
				new[]
				{
					SyntaxKind.RecursivePattern,
					SyntaxKind.EqualsExpression,
					SyntaxKind.NotEqualsExpression
				}
			);
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, _, originalNode) = context;

			switch (originalNode)
			{
				// obj.Length == length
				// obj.Length != length
				case BinaryExpressionSyntax
				{
					RawKind: var kind and (
						(int)SyntaxKind.EqualsExpression or (int)SyntaxKind.NotEqualsExpression
					),
					Left: MemberAccessExpressionSyntax
					{
						RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
						Expression: var expr,
						Name: { Identifier: { ValueText: "Count" or "Length" } }
					},
					Right: var possibleConstantExpr
				}
				when semanticModel.GetOperation(possibleConstantExpr) is { ConstantValue: { HasValue: true } }:
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0627,
							location: originalNode.GetLocation(),
							properties: ImmutableDictionary.CreateRange(
								new KeyValuePair<string, string?>[]
								{
									new(
										"IsEqualExpression",
										(kind == (int)SyntaxKind.EqualsExpression).ToString()
									)
								}
							),
							messageArgs: new[]
							{
								$@"{expr} is [{(
									kind != (int)SyntaxKind.EqualsExpression ? "not" : string.Empty
								)}{possibleConstantExpr}]"
							}
						)
					);

					break;
				}

				// length == obj.Length
				// length != obj.Length
				case BinaryExpressionSyntax
				{
					RawKind: var kind and (
						(int)SyntaxKind.EqualsExpression or (int)SyntaxKind.NotEqualsExpression
					),
					Left: var possibleConstantExpr,
					Right: MemberAccessExpressionSyntax
					{
						RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
						Expression: var expr,
						Name: { Identifier: { ValueText: "Count" or "Length" } }
					}
				}
				when semanticModel.GetOperation(possibleConstantExpr) is { ConstantValue: { HasValue: true } }:
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0627,
							location: originalNode.GetLocation(),
							properties: ImmutableDictionary.CreateRange(
								new KeyValuePair<string, string?>[]
								{
									new(
										"IsEqualExpression",
										(kind == (int)SyntaxKind.EqualsExpression).ToString()
									)
								}
							),
							messageArgs: new[]
							{
								$@"{expr} is [{(
									kind != (int)SyntaxKind.EqualsExpression ? "not" : string.Empty
								)}{possibleConstantExpr}]"
							}
						)
					);

					break;
				}

				// s is { Length: length }
				// s is not { Length: length }
				case RecursivePatternSyntax
				{
					Parent: var parent,
					PropertyPatternClause: { Subpatterns: { Count: not 0 } subpatterns }
				} recursivePattern:
				{
					foreach (var subpattern in subpatterns)
					{
						if (
							subpattern is not
							{
								NameColon: { Name: { Identifier: { ValueText: "Length" or "Count" } } },
								Pattern: var pattern
							}
						)
						{
							continue;
						}

						var node = recursivePattern.Ancestors().OfType<IsPatternExpressionSyntax>().First();
						context.ReportDiagnostic(
							Diagnostic.Create(
								descriptor: SS0627,
								location: originalNode.GetLocation(),
								properties: ImmutableDictionary.CreateRange(
									new KeyValuePair<string, string?>[] { new("IsEqualExpression", null) }
								),
								messageArgs: new[]
								{
									$@"{node.Expression} {(
										parent is UnaryPatternSyntax ? "is not" : "is"
									)} [{pattern}]"
								}
							)
						);
					}

					break;
				}
			}
		}
	}
}
