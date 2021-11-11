namespace Sudoku.Solving.Manual.Steps.AlmostLockedSets;

/// <summary>
/// Provides with a step that is a <b>Almost Locked Sets</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
public abstract record AlmostLockedSetsStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views
) : Step(Conclusions, Views)
{
	/// <inheritdoc/>
	public sealed override bool ShowDifficulty => base.ShowDifficulty;

	/// <inheritdoc/>
	public sealed override bool IsElementary => base.IsElementary;

	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	public override string? Format => base.Format;

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.AlsChainingLike;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => TechniqueTags.Als;

	/// <inheritdoc/>
	public sealed override Stableness Stableness => base.Stableness;
}
