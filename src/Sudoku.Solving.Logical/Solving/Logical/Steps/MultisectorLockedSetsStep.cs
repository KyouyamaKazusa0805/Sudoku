namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Multi-sector Locked Sets</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells">Indicates the cells used.</param>
internal sealed record MultisectorLockedSetsStep(Conclusion[] Conclusions, View[]? Views, scoped in CellMap Cells) :
	NonnegativeRankStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 9.4M;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.Msls;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.MultisectorLockedSets;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.MultisectorLockedSets;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.HardlyEver;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.Size, A002024(Cells.Count) * .1M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?>? FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { CellsCountStr, CellsStr } }, { "zh", new[] { CellsCountStr, CellsStr } } };

	private string CellsCountStr => Cells.Count.ToString();

	private string CellsStr => Cells.ToString();
}
