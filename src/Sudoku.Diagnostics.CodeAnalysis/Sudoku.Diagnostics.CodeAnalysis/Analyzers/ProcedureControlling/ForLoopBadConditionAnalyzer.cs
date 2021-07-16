using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
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
			@"[\w\.]+",
			RegexOptions.Compiled,
			TimeSpan.FromSeconds(5)
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
			var (semanticModel, _, originalNode, _, cancellationToken) = context;
			if (originalNode is ForStatementSyntax { Condition: { } conditionNode } node)
			{
				AnalyzeSyntaxNodeRecursively(context, node, conditionNode, semanticModel, cancellationToken);
			}
		}

		private static void AnalyzeSyntaxNodeRecursively(
			in SyntaxNodeAnalysisContext context, ForStatementSyntax forStatement, SyntaxNode originalNode,
			SemanticModel semanticModel, in CancellationToken cancellationToken)
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
							foreach (var possibleExpression in new[] { rightExpr, leftExpr })
							{
								// Check whether the expression is a constant value.
								if (possibleExpression.IsSimpleExpression())
								{
									continue;
								}

								// Check whether the expression is a constant expression.
								if (semanticModel.GetOperation(rightExpr) is { ConstantValue: { HasValue: true } })
								{
									continue;
								}

								// Check whether the expression is as a same type as the variable declaration part.
								if (
									forStatement.Declaration?.Type is { } declarationType
									&& semanticModel.GetSymbolInfo(declarationType, cancellationToken) is
									{
										Symbol: var declarationTypeSymbol
									}
									&& semanticModel.GetOperation(possibleExpression, cancellationToken) is
									{
										Type: var typeToCheck
									}
									&& !SymbolEqualityComparer.Default.Equals(declarationTypeSymbol, typeToCheck)
								)
								{
									continue;
								}

								string exprStr = possibleExpression.ToString();
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
										location: possibleExpression.GetLocation(),
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
							AnalyzeSyntaxNodeRecursively(context, forStatement, leftExpr, semanticModel, cancellationToken);
							AnalyzeSyntaxNodeRecursively(context, forStatement, rightExpr, semanticModel, cancellationToken);

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
					AnalyzeSyntaxNodeRecursively(context, forStatement, operand, semanticModel, cancellationToken);

					break;
				}
			}
		}
	}
}
