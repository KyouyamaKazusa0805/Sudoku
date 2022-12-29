namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Chain</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
internal abstract record ChainStep(ConclusionList Conclusions, ViewList Views) : Step(Conclusions, Views), IChainLikeStep
{
	/// <inheritdoc/>
	public abstract int FlatComplexity { get; }
}
