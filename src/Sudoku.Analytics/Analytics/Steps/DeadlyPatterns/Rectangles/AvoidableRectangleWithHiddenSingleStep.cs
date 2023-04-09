namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Avoidable Rectangle with Hidden Single</b> technique.
/// </summary>
public sealed class AvoidableRectangleWithHiddenSingleStep(
	Conclusion[] conclusions,
	View[]? views,
	int digit1,
	int digit2,
	scoped in CellMap cells,
	int baseCell,
	int targetCell,
	int house,
	int absoluteOffset
) : UniqueRectangleStep(
	conclusions,
	views,
	(Technique)((int)Technique.AvoidableRectangleHiddenSingleBlock + (int)house.ToHouseType()),
	digit1,
	digit2,
	cells,
	true,
	absoluteOffset
)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .2M;

	/// <summary>
	/// Indicates the house where the pattern lies.
	/// </summary>
	public int House { get; } = house;

	/// <summary>
	/// Indicates the base cell used.
	/// </summary>
	public int BaseCell { get; } = baseCell;

	/// <summary>
	/// Indicates the target cell used.
	/// </summary>
	public int TargetCell { get; } = targetCell;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

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
