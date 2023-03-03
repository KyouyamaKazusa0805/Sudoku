namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Domino Loop</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells">Indicates the cells used.</param>
internal sealed record DominoLoopStep(Conclusion[] Conclusions, View[]? Views, scoped in CellMap Cells) : NonnegativeRankStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 9.6M;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.LongChaining | TechniqueTags.RankTheory;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.DominoLoop;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.DominoLoop;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.OnlyForSpecialPuzzles;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { CellsCountStr, CellsStr } }, { "zh", new[] { CellsCountStr, CellsStr } } };

	private string CellsCountStr => Cells.Count.ToString();

	private string CellsStr => Cells.ToString();
}
