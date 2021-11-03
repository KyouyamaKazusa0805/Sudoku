namespace Sudoku.Solving.Manual.Steps.LastResorts;

/// <summary>
/// Provides with a step that is a <b>Bowman's Bingo</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="ContradictionLinks">Indicates the list of contradiction links.</param>
public sealed record BowmanBingoStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	ImmutableArray<Conclusion> ContradictionLinks
) : LastResortStep(Conclusions, Views), IChainLikeStep
{
	/// <inheritdoc/>
	public override decimal Difficulty =>
		8.0M + IChainLikeStep.GetExtraDifficultyByLength(ContradictionLinks.Length);

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => TechniqueTags.LongChaining | TechniqueTags.ForcingChains;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BowmanBingo;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.BowmanBingo;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Often;

	/// <inheritdoc/>
	public override Stableness Stableness => Stableness.LessUnstable;

	[FormatItem]
	private string ContradictionSeriesStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new ConclusionCollection(ContradictionLinks).ToString(false, " -> ");
	}
}
