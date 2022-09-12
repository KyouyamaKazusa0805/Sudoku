namespace Sudoku.Filtering;

/// <summary>
/// Defines a way that is used for filtering the gathered step after
/// <see cref="IStepGatherableSearcher.Search(in Grid, CancellationToken)"/> invoked.
/// </summary>
/// <seealso cref="IStepGatherableSearcher"/>
public static partial class TechniqueFiltering
{
	/// <summary>
	/// Filters some invalid steps.
	/// </summary>
	/// <param name="steps">The found steps.</param>
	/// <param name="conditionString">The condition.</param>
	/// <returns>Filtered steps.</returns>
	/// <exception cref="ExpressiveException">Throws when the expression being evaluated is invalid.</exception>
	public static IEnumerable<IStep> Filter(IEnumerable<IStep> steps, string conditionString)
		=>
		from step in steps
		where Parse(conditionString, step).Evaluate<bool>()
		select step;

	/// <summary>
	/// To parse a condition string, converting the string into a valid <see cref="Expression"/> instance.
	/// </summary>
	/// <param name="conditionString">The condition string.</param>
	/// <param name="step">The step used.</param>
	/// <returns>The <see cref="Expression"/> instance.</returns>
	private static Expression Parse(string conditionString, IStep step)
		=> new(
			KeywordPattern()
				.Replace(
					conditionString,
					match => match.Value.ToLower() switch
					{
						"&&" => " and ",
						"||" => " or ",
						"!" => "not ",
						"d" or "difficulty" => step.Difficulty.ToString(),
						var @default => @default
					}
				),
			ExpressiveOptions.IgnoreCaseForParsing
		);

	[RegexGenerator("""(d(i{2}iculty)?|\&{2}|\|{2}|!)""", RegexOptions.Compiled | RegexOptions.IgnoreCase, 5000)]
	private static partial Regex KeywordPattern();
}
