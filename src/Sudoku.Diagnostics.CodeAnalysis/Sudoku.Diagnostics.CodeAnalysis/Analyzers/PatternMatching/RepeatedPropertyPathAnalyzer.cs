#warning SS0625 doesn't support now.


namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SS0623", "SS0625")]
public sealed partial class RepeatedPropertyPathAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(
			static context =>
			{
				CheckSS0623(context);
				CheckSS0625(context);
			},
			new[] { SyntaxKind.PropertyPatternClause }
		);
	}


	private static void CheckSS0623(SyntaxNodeAnalysisContext context)
	{
		if (
			context.Node is not PropertyPatternClauseSyntax
			{
				Subpatterns: { Count: >= 2 } subpatterns
			}
		)
		{
			return;
		}

		var listOfNames = (
			from subpattern in subpatterns
			let nameColonNode = subpattern.NameColon
			where nameColonNode is not null
			select (PatternNode: subpattern, nameColonNode.Name.Identifier.ValueText)
		).ToArray();

		for (int i = 0, length = listOfNames.Length, iterationLength = length - 1; i < iterationLength; i++)
		{
			for (int j = i + 1; j < length; j++)
			{
				if (listOfNames[i].ValueText == listOfNames[j].ValueText)
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0623,
							location: listOfNames[j].PatternNode.GetLocation(),
							messageArgs: null
						)
					);
				}
			}
		}
	}

	private static void CheckSS0625(SyntaxNodeAnalysisContext context)
	{
#if false
		// Due to current enviornment, the API doesn't support extended property pattern yet.
		// Code for reference:
		if (
			context.Node is not ExtendedPropertyPatternClauseSyntax
			{
				Subpatterns: { Count: >= 2 } subpatterns
			}
		)
		{
			return;
		}

		var listOfNames = (
			from subpattern in subpatterns
			let nameColonNode = subpattern.NameColon
			where nameColonNode is not null
			select (PatternNode: subpattern, nameColonNode.ToString())
		).ToArray();

		for (int i = 0, length = listOfNames.Length; i < length - 1; i++)
		{
			for (int j = i + 1; j < length; j++)
			{
				if (listOfNames[i].ValueText == listOfNames[j].ValueText)
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0623,
							location: listOfNames[j].PatternNode.GetLocation(),
							messageArgs: null
						)
					);
				}
			}
		}
#endif
	}
}
