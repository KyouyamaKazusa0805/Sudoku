namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Domino Loop</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells">Indicates the cells used.</param>
internal sealed partial record DominoLoopStep(ConclusionList Conclusions, ViewList Views, scoped in CellMap Cells) :
	NonnegativeRankStep(Conclusions, Views),
	IStepWithRank
{
	/// <inheritdoc/>
	public override decimal Difficulty => 9.6M;

	/// <inheritdoc/>
	public int Rank => 0;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags
		=> base.TechniqueTags | TechniqueTags.LongChaining | TechniqueTags.RankTheory;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.DominoLoop;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.DominoLoop;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.OnlyForSpecialPuzzles;

	[ResourceTextFormatter]
	private partial string CellsCountStr() => Cells.Count.ToString();

	[ResourceTextFormatter]
	private partial string CellsStr() => Cells.ToString();
}
