using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.CodeGen;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS0312")]
	public sealed partial class MultipleWhereClauseAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.QueryExpression });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			if (context.Node is not QueryExpressionSyntax { Body: { Clauses: { Count: >= 2 } clauses } body })
			{
				return;
			}

			for (int i = 0, count = clauses.Count; i < count - 1; i++)
			{
				if (
					(clauses[i], clauses[i + 1]) is not (
						WhereClauseSyntax { Condition: var formerCondition },
						WhereClauseSyntax { Condition: var latterCondition } latter
					)
				)
				{
					continue;
				}

				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SS0312,
						location: latter.GetLocation(),
						properties: ImmutableDictionary.CreateRange(
							new KeyValuePair<string, string?>[]
							{
								new(
									"ShouldAppendParen",
									(
										latterCondition is not BinaryExpressionSyntax
										{
											RawKind: (int)SyntaxKind.GreaterThanExpression
												or (int)SyntaxKind.GreaterThanOrEqualExpression
												or (int)SyntaxKind.LessThanExpression
												or (int)SyntaxKind.LessThanOrEqualExpression
												or (int)SyntaxKind.EqualsExpression
												or (int)SyntaxKind.NotEqualsExpression
												or (int)SyntaxKind.LogicalNotExpression
												or (int)SyntaxKind.LogicalAndExpression
										}
									).ToString()
								),
								new("Index", i.ToString())
							}
						),
						messageArgs: null,
						additionalLocations: new[]
						{
							body.GetLocation(),
							formerCondition.GetLocation(),
							latterCondition.GetLocation()
						}
					)
				);
			}
		}
	}
}
