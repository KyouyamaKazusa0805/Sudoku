namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle with Baba Grouping</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="targetCell">Indicates the target cell.</param>
/// <param name="extraDigit">Indicates the extra digit used.</param>
/// <param name="absoluteOffset"><inheritdoc/></param>
public sealed partial class UniqueRectangleWithBabaGroupingStep(
	Conclusion[] conclusions,
	View[]? views,
	Digit digit1,
	Digit digit2,
	scoped in CellMap cells,
	[DataMember] Cell targetCell,
	[DataMember] Digit extraDigit,
	int absoluteOffset
) : UniqueRectangleStep(
	conclusions,
	views,
	Technique.UniqueRectangleBabaGrouping,
	digit1,
	digit2,
	cells,
	false,
	absoluteOffset
)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .4M;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [D1Str, D2Str, CellsStr, TargetCellStr, DigitsStr, ExtraDigitStr]),
			new(ChineseLanguage, [D1Str, D2Str, CellsStr, TargetCellStr, DigitsStr, ExtraDigitStr])
		];

	private string TargetCellStr => CellNotation.ToString(TargetCell);

	private string DigitsStr
		=> string.Join(GetString("OrKeywordWithSpaces")!, (from digit in ((Mask)(1 << Digit1 | 1 << Digit2)).GetAllSets() select DigitNotation.ToString(digit)).ToArray());

	private string ExtraDigitStr => DigitNotation.ToString(ExtraDigit);
}
