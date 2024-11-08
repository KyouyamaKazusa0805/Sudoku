namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Specialized Chain</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
public abstract class SpecializedChainStep(StepConclusions conclusions, View[]? views, StepGathererOptions options) :
	ChainStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public sealed override bool IsSpecialized => true;
}
