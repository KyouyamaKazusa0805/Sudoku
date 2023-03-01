namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is an <b>Avoidable Rectangle with Hidden Single</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="BaseCell"></param>
/// <param name="TargetCell"></param>
/// <param name="House"></param>
/// <param name="AbsoluteOffset"><inheritdoc/></param>
internal sealed record AvoidableRectangleWithHiddenSingleStep(
	ConclusionList Conclusions,
	ViewList Views,
	int Digit1,
	int Digit2,
	scoped in CellMap Cells,
	int BaseCell,
	int TargetCell,
	int House,
	int AbsoluteOffset
) : UniqueRectangleStep(
	Conclusions,
	Views,
	(Technique)((int)Technique.AvoidableRectangleHiddenSingleBlock + (int)House.ToHouseType()),
	Digit1,
	Digit2,
	Cells,
	true,
	AbsoluteOffset
)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .2M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.UniqueRectanglePlus;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?>? FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { D1Str, D2Str, CellsStr, BaseCellStr, HouseStr, TargetCellStr } },
			{ "zh", new[] { D1Str, D2Str, CellsStr, BaseCellStr, HouseStr, TargetCellStr } }
		};

	private string BaseCellStr => RxCyNotation.ToCellString(BaseCell);

	private string TargetCellStr => RxCyNotation.ToCellString(TargetCell);

	private string HouseStr => HouseFormatter.Format(1 << House);
}
