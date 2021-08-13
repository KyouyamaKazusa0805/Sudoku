namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SS0626")]
public sealed partial class PairKeywordNotAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.NotPattern });
	}


	private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
	{
		if (
			context.Node is not UnaryPatternSyntax
			{
				Parent: not UnaryPatternSyntax { RawKind: (int)SyntaxKind.NotPattern },
				RawKind: (int)SyntaxKind.NotPattern,
				Pattern: var pattern
			} node
		)
		{
			return;
		}

		int count = 1;
		PatternSyntax patternNode;
		for (
			patternNode = pattern;
			patternNode is UnaryPatternSyntax
			{
				RawKind: (int)SyntaxKind.NotPattern,
				Pattern: var nestedPattern
			};
			patternNode = nestedPattern,
			count++
		) ;

		if (count < 2)
		{
			return;
		}

		context.ReportDiagnostic(
			Diagnostic.Create(
				descriptor: SS0626,
				location: node.GetLocation(),
				messageArgs: null,
				properties: ImmutableDictionary.CreateRange(
					new KeyValuePair<string, string?>[] { new("Count", count.ToString()) }
				),
				additionalLocations: new[] { patternNode.GetLocation() }
			)
		);
	}
}
