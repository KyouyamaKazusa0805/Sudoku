namespace Sudoku.Analytics.Configuration;

/// <summary>
/// Represents condition options applied to <see cref="StepSearcher"/>s.
/// </summary>
/// <seealso cref="StepSearcher"/>
public sealed record StepSearcherConditionalOptions : IStepSearcherOptions<StepSearcherConditionalOptions?>
{
#if SINGLE_TECHNIQUE_LIMIT_FLAG
	/// <summary>
	/// Indicates whether the step searcher will checks for hidden singles in rows or columns.
	/// </summary>
	public bool AllowsHiddenSingleInLines { get; init; } = false;

	/// <summary>
	/// Indicates the preferred single technique. If the value is not (<see cref="SingleTechniqueFlag"/>)0,
	/// the analyzer will ignore the other single technique not related to this field.
	/// </summary>
	/// <remarks>
	/// For example, if the value is <see cref="SingleTechniqueFlag.HiddenSingle"/>,
	/// the analyzer will automatically ignore naked single steps to analyze the grid.
	/// </remarks>
	/// <seealso cref="SingleTechniqueFlag"/>
	public SingleTechniqueFlag LimitedSingle { get; init; } = 0;
#endif


	/// <inheritdoc/>
	public static StepSearcherConditionalOptions? Default => null;
}
