namespace Sudoku.Solving.Manual.Steps.Subsets;

/// <summary>
/// Provides with a step that is a <b>Subset</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Region">The region that structure lies in.</param>
/// <param name="Cells">All cells used.</param>
/// <param name="DigitsMask">The maks that contains all digits used.</param>
public abstract record SubsetStep(
	in ImmutableArray<Conclusion> Conclusions,
	in ImmutableArray<PresentationData> Views,
	int Region,
	in Cells Cells,
	short DigitsMask
) : Step(Conclusions, Views)
{
	/// <inheritdoc/>
	public sealed override bool IsSstsStep => true;

	/// <inheritdoc/>
	public sealed override bool ShowDifficulty => base.ShowDifficulty;

	/// <summary>
	/// Indicates the size of the subset.
	/// </summary>
	public int Size => PopCount((uint)DigitsMask);

	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Moderate;

	/// <inheritdoc/>
	public sealed override TechniqueTags TechniqueTags => TechniqueTags.Subsets;

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.Subset;

	/// <inheritdoc/>
	public sealed override Stableness Stableness => base.Stableness;

	/// <inheritdoc/>
	public sealed override Rarity Rarity => Size switch
	{
		2 => Rarity.Often,
		3 => Rarity.Sometimes,
		4 => Rarity.Seldom
	};
}
