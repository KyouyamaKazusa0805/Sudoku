namespace Sudoku.Analytics.Configuration;

/// <summary>
/// Represents condition options.
/// </summary>
internal sealed record ConditionalOptions
{
#if SINGLE_TECHNIQUE_LIMIT_FLAG
	/// <summary>
	/// Indicates whether the step searcher will checks for hidden singles in rows or columns.
	/// </summary>
	public bool AllowsHiddenSingleInLines { get; init; } = false;

	/// <summary>
	/// Indicates the preferred single technique. If the value is not (<see cref="SingleTechnique"/>)0,
	/// the analyzer will ignore the other single technique not related to this field.
	/// </summary>
	/// <remarks>
	/// For example, if the value is <see cref="SingleTechnique.HiddenSingle"/>,
	/// the analyzer will automatically ignore naked single steps to analyze the grid.
	/// If the puzzle cannot be solved, the analyzer will return an <see cref="AnalyzerResult"/> with
	/// <see cref="AnalyzerResult.IsSolved"/> a <see langword="false"/> value.
	/// </remarks>
	/// <seealso cref="SingleTechnique"/>
	/// <seealso cref="AnalyzerResult.IsSolved"/>
	public SingleTechnique LimitedSingle { get; init; } = 0;
#endif
}
