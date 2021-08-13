namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SS0304F")]
public sealed partial class WhereClausePureBooleanAnalyzer : DiagnosticAnalyzer
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

		foreach (var clause in clauses)
		{
			if (
				clause is not WhereClauseSyntax
				{
					Condition: LiteralExpressionSyntax
					{
						RawKind: var kind and (
							(int)SyntaxKind.TrueLiteralExpression or (int)SyntaxKind.FalseLiteralExpression
						)
					}
				}
			)
			{
				continue;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS0304,
					location: clause.GetLocation(),
					messageArgs: new string[]
					{
						kind switch
						{
							(int)SyntaxKind.TrueLiteralExpression => "true",
							(int)SyntaxKind.FalseLiteralExpression => "false"
						}
					}
				)
			);
		}
	}
}
