namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Chain</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
public abstract record ChainStep(ImmutableArray<Conclusion> Conclusions, ImmutableArray<View> Views) :
	Step(Conclusions, Views),
	IChainStep,
	IChainLikeStep
{
	/// <inheritdoc/>
	public abstract int FlatComplexity { get; }

	/// <inheritdoc/>
	public abstract ChainTypeCode SortKey { get; }
}
