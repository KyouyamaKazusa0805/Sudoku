namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Miscellaneous Deadly Pattern</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
public abstract class MiscellaneousDeadlyPatternStep(StepConclusions conclusions, View[]? views, StepGathererOptions options) :
	DeadlyPatternStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public sealed override bool? IsUnconditional => null;
}
