using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.CodeGen;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS0310")]
	public sealed partial class OrderbyAfterWhereFilterAnalyzer : DiagnosticAnalyzer
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
			if (context.Node is not QueryExpressionSyntax { Body: { Clauses: { Count: not 0 } clauses } })
			{
				return;
			}

			for (int i = 0, count = clauses.Count; i < count - 1; i++)
			{
				if (
					(clauses[i], clauses[i + 1]) is not (
						WhereClauseSyntax
						{
							Condition: BinaryExpressionSyntax
							{
								RawKind: (int)SyntaxKind.EqualsExpression,
								Left: var leftExpr,
								Right: var rightExpr
							} condition
						},
						OrderByClauseSyntax { Orderings: { Count: not 0 } orderings }
					)
				)
				{
					continue;
				}

				foreach (var ordering in orderings)
				{
					if (
						ordering is not
						{
							Expression: IdentifierNameSyntax
							{
								Identifier: { ValueText: var identifier }
							}
						}
					)
					{
						continue;
					}

					if (
						!(
							leftExpr is IdentifierNameSyntax { Identifier: { ValueText: var leftIdentifier } }
							&& leftIdentifier == identifier
							|| rightExpr is IdentifierNameSyntax { Identifier: { ValueText: var rightIdentifier } }
							&& rightIdentifier == identifier
						)
					)
					{
						continue;
					}

					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0310,
							location: condition.GetLocation(),
							messageArgs: new[] { condition.ToString() }
						)
					);
				}
			}
		}
	}
}
