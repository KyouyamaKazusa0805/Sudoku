namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Conditional Deadly Pattern</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
public abstract class ConditionalDeadlyPatternStep(ReadOnlyMemory<Conclusion> conclusions, View[]? views, StepGathererOptions options) :
	DeadlyPatternStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public sealed override bool? IsUnconditional => false;
}
