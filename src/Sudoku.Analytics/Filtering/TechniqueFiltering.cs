namespace Sudoku.Filtering;

/// <summary>
/// Defines a way that is used for filtering the gathered step.
/// </summary>
/// <remarks>
/// Supported expressions:
/// <list type="bullet">
/// <item><c><see langword="rating"/> (<![CDATA[>]]>,<![CDATA[>=]]>,<![CDATA[<]]>,<![CDATA[<=]]>,<![CDATA[==]]>,<![CDATA[!=]]>) (value)</c></item>
/// <item><c><see langword="name"/> (<![CDATA[==]]>,<![CDATA[!=]]>) (technique-name)</c></item>
/// <item><c><see langword="name"/> <see langword="like"/> (pattern)</c></item>
/// <item><c><see langword="conclusion"/> (<![CDATA[==]]>,<![CDATA[!=]]>) (conclusion)</c></item>
/// <item><c><see langword="conclusion"/> <see langword="has"/> (coordinate)</c></item>
/// <item><c><see langword="count"/> <see langword="conclusion"/> (<![CDATA[>]]>,<![CDATA[>=]]>,<![CDATA[<]]>,<![CDATA[<=]]>,<![CDATA[==]]>,<![CDATA[!=]]>) (count)</c></item>
/// </list>
/// </remarks>
[Obsolete("This type will be removed in the future.", false)]
public static partial class TechniqueFiltering
{
	/// <summary>
	/// Indicates the rating keyword.
	/// </summary>
	private const string RatingKeyword = "rating";

	/// <summary>
	/// Indicates the name keyword.
	/// </summary>
	private const string NameKeyword = "name";

	/// <summary>
	/// Indicates the conclusion keyword.
	/// </summary>
	private const string ConclusionKeyword = "conclusion";


	/// <summary>
	/// Filters some invalid steps.
	/// </summary>
	/// <param name="steps">The found steps.</param>
	/// <param name="conditionString">The condition.</param>
	/// <exception cref="ExpressiveException">Throws when the expression being evaluated is invalid.</exception>
	public static Step[] Filter(Step[] steps, string conditionString)
		=> string.IsNullOrWhiteSpace(conditionString)
			? steps // Special case: If the condition string is empty, just return.
			: from step in steps where Parse(conditionString, step).Evaluate<bool>() select step;

	/// <summary>
	/// To parse a condition string, converting the string into a valid <see cref="Expression"/> instance.
	/// </summary>
	/// <param name="conditionString">The condition string.</param>
	/// <param name="step">The step used.</param>
	/// <returns>The <see cref="Expression"/> instance.</returns>
	private static Expression Parse(string conditionString, Step step)
	{
		var result = new Expression(
			KeywordPattern()
				.Replace(
					conditionString,
					match => match.Value.ToLower() switch
					{
						RatingKeyword => $"{step.Difficulty}",
						NameKeyword => $"'{step.Name}'",
						ConclusionKeyword => $"'{step.Options.Converter.ConclusionConverter(step.Conclusions)}'",
						_ => throw new NotSupportedException("The specified match is not supported to be replaced.")
					}
				),
			ExpressiveOptions.IgnoreCaseForParsing
		);

		result.RegisterOperator(NameLikeOperator.Instance);
		result.RegisterOperator(ConclusionHasOperator.Instance);
		result.RegisterOperator(CountConclusionOperator.Instance);

		return result;
	}

	[GeneratedRegex("""(rating|name|conclusion)""", RegexOptions.Compiled | RegexOptions.IgnoreCase, 5000)]
	private static partial Regex KeywordPattern();
}
