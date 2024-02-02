namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Avoidable Rectangle with Hidden Single</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
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
	StepSearcherOptions options,
	Digit digit1,
	Digit digit2,
	scoped ref readonly CellMap cells,
	[PrimaryCosntructorParameter] Cell baseCell,
	[PrimaryCosntructorParameter] Cell targetCell,
	[PrimaryCosntructorParameter] House house,
	int absoluteOffset
) : UniqueRectangleStep(
	conclusions,
	views,
	options,
	(Technique)((int)Technique.AvoidableRectangleHiddenSingleBlock + (int)house.ToHouseType()),
	digit1,
	digit2,
	in cells,
	true,
	absoluteOffset
)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .2M;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [D1Str, D2Str, CellsStr, BaseCellStr, HouseStr, TargetCellStr]),
			new(ChineseLanguage, [D1Str, D2Str, CellsStr, BaseCellStr, HouseStr, TargetCellStr])
		];

	private string BaseCellStr => Options.Converter.CellConverter([BaseCell]);

	private string TargetCellStr => Options.Converter.CellConverter([TargetCell]);

	private string HouseStr => Options.Converter.HouseConverter(1 << House);
}
