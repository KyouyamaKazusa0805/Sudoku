namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

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

		for (int i = 0, iterationCount = clauses.Count - 1; i < iterationCount; i++)
		{
			if (
				(Left: clauses[i], Right: clauses[i + 1]) is not (
					Left: OrderByClauseSyntax { Orderings: { Count: not 0 } } formerClause,
					Right: OrderByClauseSyntax { Orderings: { Count: not 0 } } latterClause
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
