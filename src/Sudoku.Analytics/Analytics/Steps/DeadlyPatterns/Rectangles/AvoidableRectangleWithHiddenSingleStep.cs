namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Avoidable Rectangle with Hidden Single</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="baseCell">Indicates the base cell used.</param>
/// <param name="targetCell">Indicates the target cell used.</param>
/// <param name="house">Indicates the house where the pattern lies.</param>
/// <param name="absoluteOffset"><inheritdoc/></param>
public sealed partial class AvoidableRectangleWithHiddenSingleStep(
	Conclusion[] conclusions,
	View[]? views,
	int digit1,
	int digit2,
	scoped in CellMap cells,
	[PrimaryConstructorParameter] int baseCell,
	[PrimaryConstructorParameter] int targetCell,
	[PrimaryConstructorParameter] int house,
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

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { D1Str, D2Str, CellsStr, BaseCellStr, HouseStr, TargetCellStr } },
			{ "zh", new[] { D1Str, D2Str, CellsStr, BaseCellStr, HouseStr, TargetCellStr } }
		};

	private string BaseCellStr => RxCyNotation.ToCellString(BaseCell);

	private string TargetCellStr => RxCyNotation.ToCellString(TargetCell);

	private string HouseStr => HouseFormatter.Format(1 << House);
}
