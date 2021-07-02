using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.CodeGenerating;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS9001")]
	public sealed partial class ForLoopBadConditionAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// Indicates the regular expression to match a member access expression.
		/// </summary>
		private static readonly Regex MemberAccessExpressionRegex = new(
			@"[\w\.]+", RegexOptions.Compiled, TimeSpan.FromSeconds(5)
		);


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.ForStatement });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			if (context is { Node: ForStatementSyntax { Condition: { } conditionNode } node })
			{
				AnalyzeSyntaxNodeRecursively(context, node, conditionNode);
			}
		}

		private static void AnalyzeSyntaxNodeRecursively(
			in SyntaxNodeAnalysisContext context, ForStatementSyntax forStatement, SyntaxNode originalNode)
		{
			switch (originalNode)
			{
				case BinaryExpressionSyntax { RawKind: var kind, Left: var leftExpr, Right: var rightExpr }:
				{
					switch (kind)
					{
						case (int)SyntaxKind.EqualsExpression:
						case (int)SyntaxKind.NotEqualsExpression:
						case (int)SyntaxKind.GreaterThanExpression:
						case (int)SyntaxKind.GreaterThanEqualsToken:
						case (int)SyntaxKind.LessThanExpression:
						case (int)SyntaxKind.LessThanEqualsToken:
						{
							foreach (var possibleExpressions in new[] { rightExpr, leftExpr })
							{
								if (possibleExpressions.IsSimpleExpression())
								{
									continue;
								}

								if (
									context.SemanticModel.GetOperation(rightExpr) is
									{
										ConstantValue: { HasValue: true }
									}
								)
								{
									continue;
								}

								string exprStr = possibleExpressions.ToString();
								var match = MemberAccessExpressionRegex.Match(exprStr);
								string? suggestedName;
								if (match.Success)
								{
									string matchStr = match.ToString();
									suggestedName = matchStr.Substring(matchStr.LastIndexOf('.') + 1);
								}
								else
								{
									suggestedName = null;
								}

								context.ReportDiagnostic(
									Diagnostic.Create(
										descriptor: SS9001,
										location: possibleExpressions.GetLocation(),
										properties: ImmutableDictionary.CreateRange(
											new KeyValuePair<string, string?>[]
											{
												new("SuggestedName", suggestedName?.ToCamelCase())
											}
										),
										additionalLocations: new[] { forStatement.GetLocation() },
										messageArgs: null
									)
								);

								// This algorithm only analyzes for one side.
								// if the condition is like "a.Length > b.Length",
								// the analyzer will do nothing.
								break;
							}

							break;
						}
						case (int)SyntaxKind.LogicalAndExpression:
						case (int)SyntaxKind.LogicalOrExpression:
						//case (int)SyntaxKind.ExclusiveOrExpression:
						{
							AnalyzeSyntaxNodeRecursively(context, forStatement, leftExpr);
							AnalyzeSyntaxNodeRecursively(context, forStatement, rightExpr);

							break;
						}
					}

					break;
				}
				case PrefixUnaryExpressionSyntax
				{
					RawKind: (int)SyntaxKind.LogicalNotExpression,
					Operand: var operand
				}:
				{
					AnalyzeSyntaxNodeRecursively(context, forStatement, operand);

					break;
				}
			}
		}
	}
}
