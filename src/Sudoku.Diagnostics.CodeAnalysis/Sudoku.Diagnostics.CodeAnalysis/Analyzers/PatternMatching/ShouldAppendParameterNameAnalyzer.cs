namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SS0636")]
public sealed partial class ShouldAppendParameterNameAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.RecursivePattern });
	}


	private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
	{
		var (semanticModel, _, originalNode) = context;

		if (
			context.Node is not RecursivePatternSyntax
			{
				Parent: not RecursivePatternSyntax { PositionalPatternClause.Subpatterns: { Count: not 0 } },
				PositionalPatternClause.Subpatterns: { Count: not 0 } subpatterns
			} node
		)
		{
			return;
		}

		recursion(subpatterns);


		void recursion(in SeparatedSyntaxList<SubpatternSyntax> subpatterns)
		{
			foreach (var subpattern in subpatterns)
			{
				switch (subpattern.Pattern)
				{
					case RecursivePatternSyntax
					{
						PositionalPatternClause.Subpatterns: { Count: not 0 } nestedSubpatterns
					}:
					{
						recursion(nestedSubpatterns);

						break;
					}
					case not (DiscardPatternSyntax or VarPatternSyntax) when subpattern.NameColon is null:
					{
						context.ReportDiagnostic(
							Diagnostic.Create(
								descriptor: SS0636,
								location: subpattern.GetLocation(),
								messageArgs: null
							)
						);

						break;
					}
				}
			}
		}
	}
}
