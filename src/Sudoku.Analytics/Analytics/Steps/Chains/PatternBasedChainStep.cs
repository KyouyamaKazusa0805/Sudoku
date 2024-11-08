namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Pattern-Based Chain</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="pattern">Indicates the pattern to be used.</param>
public abstract partial class PatternBasedChainStep(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	[Property] IChainOrForcingChains pattern
) : ChainStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public sealed override bool IsSpecialized => false;
}
