namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle External XY-Wing</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="GuardianCells">Indicates the cells that the guardians lie in.</param>
/// <param name="CellPair">Indicates the cell pair.</param>
/// <param name="IsIncomplete">Indicates whether the rectangle is incomplete.</param>
/// <param name="IsAvoidable">Indicates whether the structure is based on avoidable rectangle.</param>
/// <param name="AbsoluteOffset"><inheritdoc/></param>
internal sealed record UniqueRectangleExternalXyWingStep(
	ConclusionList Conclusions,
	ViewList Views,
	int Digit1,
	int Digit2,
	scoped in CellMap Cells,
	scoped in CellMap GuardianCells,
	scoped in CellMap CellPair,
	bool IsIncomplete,
	bool IsAvoidable,
	int AbsoluteOffset
) : UniqueRectangleStep(
	Conclusions,
	Views,
	IsAvoidable ? Technique.AvoidableRectangleExternalXyWing : Technique.UniqueRectangleExternalXyWing,
	Digit1,
	Digit2,
	Cells,
	IsAvoidable,
	AbsoluteOffset
)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .2M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[]
		{
			new(ExtraDifficultyCaseNames.Guardian, A004526(GuardianCells.Count) * .1M),
			new(ExtraDifficultyCaseNames.Avoidable, IsAvoidable ? .1M : 0),
			new(ExtraDifficultyCaseNames.Incompleteness, IsIncomplete ? .1M : 0)
		};


	[ResourceTextFormatter]
	internal string GuardianCellsStr() => GuardianCells.ToString();

	[ResourceTextFormatter]
	internal string CellPairStr() => CellPair.ToString();
}
