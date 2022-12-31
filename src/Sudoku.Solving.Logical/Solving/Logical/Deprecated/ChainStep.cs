#if false
namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Chain</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
[Obsolete("This type is being deprecated.", false)]
internal abstract record ChainStep(ConclusionList Conclusions, ViewList Views) : Step(Conclusions, Views), IChainStep, IChainLikeStep
{
	/// <inheritdoc/>
	public abstract int FlatComplexity { get; }

	/// <inheritdoc/>
	public abstract ChainTypeCode SortKey { get; }
}

#endif