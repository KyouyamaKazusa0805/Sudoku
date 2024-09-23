namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle with Baba Grouping</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="targetCell">Indicates the target cell.</param>
/// <param name="extraDigit">Indicates the extra digit used.</param>
/// <param name="absoluteOffset"><inheritdoc/></param>
public sealed partial class UniqueRectangleWithBabaGroupingStep(
	Conclusion[] conclusions,
	View[]? views,
	StepGathererOptions options,
	Digit digit1,
	Digit digit2,
	ref readonly CellMap cells,
	[PrimaryConstructorParameter] Cell targetCell,
	[PrimaryConstructorParameter] Digit extraDigit,
	int absoluteOffset
) : UniqueRectangleStep(
	conclusions,
	views,
	options,
	Technique.UniqueRectangleBabaGrouping,
	digit1,
	digit2,
	in cells,
	false,
	absoluteOffset
)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 4;

	/// <inheritdoc/>
	public override Mask DigitsUsed => (Mask)(base.DigitsUsed | (Mask)(1 << ExtraDigit));

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [
			new(SR.EnglishLanguage, [D1Str, D2Str, CellsStr, TargetCellStr, DigitsStr, ExtraDigitStr]),
			new(SR.ChineseLanguage, [D1Str, D2Str, CellsStr, TargetCellStr, DigitsStr, ExtraDigitStr])
		];

	private string TargetCellStr => Options.Converter.CellConverter(in TargetCell.AsCellMap());

	private string ExtraDigitStr => Options.Converter.DigitConverter((Mask)(1 << ExtraDigit));
}