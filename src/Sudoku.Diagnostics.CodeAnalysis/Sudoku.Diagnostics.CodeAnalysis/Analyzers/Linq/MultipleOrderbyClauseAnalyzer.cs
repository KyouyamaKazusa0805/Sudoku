using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.CodeGen;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS0305")]
	public sealed partial class MultipleOrderbyClauseAnalyzer : DiagnosticAnalyzer
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
			if (context.Node is not QueryExpressionSyntax { Body: { Clauses: { Count: not 0 } clauses } } node)
			{
				return;
			}

			for (int i = 0, count = clauses.Count; i < count - 1; i++)
			{
				if (
					(clauses[i], clauses[i + 1]) is not (
						OrderByClauseSyntax { Orderings: { Count: not 0 } } formerClause,
						OrderByClauseSyntax { Orderings: { Count: not 0 } } latterClause
					)
				)
				{
					continue;
				}

				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SS0305,
						location: latterClause.GetLocation(),
						messageArgs: null,
						additionalLocations: new[] { formerClause.GetLocation() }
					)
				);
			}
		}
	}
}
