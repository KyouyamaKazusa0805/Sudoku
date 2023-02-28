namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Bowman's Bingo</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="ContradictionLinks">Indicates the list of contradiction links.</param>
[StepDisplayingFeature(StepDisplayingFeature.DifficultyRatingNotStable)]
internal sealed record BowmanBingoStep(ConclusionList Conclusions, ViewList Views, ConclusionList ContradictionLinks) :
	LastResortStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 8.0M;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[]
		{
			new(ExtraDifficultyCaseNames.Length, ChainDifficultyRating.GetExtraDifficultyByLength(ContradictionLinks.Length))
		};

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.LongChaining | TechniqueTags.ForcingChains;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BowmanBingo;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.BowmanBingo;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Often;


	[ResourceTextFormatter]
	internal string ContradictionSeriesStr() => ConclusionFormatter.Format(ContradictionLinks.ToArray(), " -> ", false);
}
