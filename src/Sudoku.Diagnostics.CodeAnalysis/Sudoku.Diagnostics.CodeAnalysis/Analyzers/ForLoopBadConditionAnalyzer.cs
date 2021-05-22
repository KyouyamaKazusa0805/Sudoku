using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates an analyzer that analyzes the code for closed <see langword="enum"/> types.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
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

			context.RegisterSyntaxNodeAction(
				static context => AnalyzeSyntaxNodeRecursively(context, context.Node),
				new[] { SyntaxKind.ForStatement }
			);
		}


		private static void AnalyzeSyntaxNodeRecursively(
			in SyntaxNodeAnalysisContext context, SyntaxNode originalNode)
		{
			if (originalNode is not ForStatementSyntax node)
			{
				return;
			}

			switch (node)
			{
				case
				{
					Condition: BinaryExpressionSyntax
					{
						RawKind: var kind,
						Left: var leftExpr,
						Right: var rightExpr
					}
				}:
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
							foreach (var (expressionNode, anotherNode) in
								new[] { (rightExpr, leftExpr), (leftExpr, rightExpr) })
							{
								if (!expressionNode.IsSimpleExpression())
								{
									string exprStr = expressionNode.ToString();
									string? suggestedName = MemberAccessExpressionRegex.Match(exprStr) is
									{
										Success: true
									} match
									&& match.ToString() is var matchStr
									&& matchStr.LastIndexOf('.') is var pos ? matchStr.Substring(pos + 1) : null;

									context.ReportDiagnostic(
										Diagnostic.Create(
											descriptor: SS9001,
											location: expressionNode.GetLocation(),
											properties: ImmutableDictionary.CreateRange(
												new KeyValuePair<string, string?>[]
												{
													new("VariableName", anotherNode.ToString()),
													new("SuggestedName", suggestedName?.ToCamelCase())
												}
											),
											additionalLocations: new[] { node.GetLocation() },
											messageArgs: null
										)
									);

									break;
								}
							}

							break;
						}
						case (int)SyntaxKind.LogicalAndExpression:
						case (int)SyntaxKind.LogicalOrExpression:
						{
							foreach (var subExpression in leftExpr.DescendantNodes())
							{
								AnalyzeSyntaxNodeRecursively(context, subExpression);
							}
							foreach (var subExpression in rightExpr.DescendantNodes())
							{
								AnalyzeSyntaxNodeRecursively(context, subExpression);
							}

							break;
						}
					}

					break;
				}
				case
				{
					Condition: PrefixUnaryExpressionSyntax
					{
						RawKind: (int)SyntaxKind.LogicalNotExpression,
						Operand: var operand
					}
				}:
				{
					foreach (var subExpression in operand.DescendantNodes())
					{
						AnalyzeSyntaxNodeRecursively(context, subExpression);
					}

					break;
				}
			}
		}
	}
}
