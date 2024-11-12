namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle W-Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="code"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="isAvoidable"><inheritdoc/></param>
/// <param name="wDigit">Indicates the digit W.</param>
/// <param name="connectors">Indicates the connectors.</param>
/// <param name="endCells">Indicates the end cells.</param>
/// <param name="absoluteOffset"><inheritdoc/></param>
public sealed partial class UniqueRectangleWWingStep(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	Technique code,
	Digit digit1,
	Digit digit2,
	ref readonly CellMap cells,
	bool isAvoidable,
	[Property] Digit wDigit,
	[Property] ref readonly CellMap connectors,
	[Property] ref readonly CellMap endCells,
	int absoluteOffset
) : UniqueRectangleStep(
	conclusions,
	views,
	options,
	code,
	digit1,
	digit2,
	in cells,
	isAvoidable,
	absoluteOffset
)
{
	/// <inheritdoc/>
	public override Mask DigitsUsed => (Mask)(base.DigitsUsed | (Mask)(1 << WDigit));

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [
			new(SR.EnglishLanguage, [D1Str, D2Str, CellsStr, ConnectorsString, EndCellsString, WDigitsString]),
			new(SR.ChineseLanguage, [D1Str, D2Str, CellsStr, ConnectorsString, EndCellsString, WDigitsString])
		];

	/// <inheritdoc/>
	public override FactorArray Factors
		=> [
			Factor.Create(
				"Factor_RectangleIsAvoidableFactor",
				[nameof(IsAvoidable)],
				GetType(),
				static args => (bool)args![0]! ? 1 : 0
			)
		];

	private string ConnectorsString => Options.Converter.CellConverter(Connectors);

	private string EndCellsString => Options.Converter.CellConverter(EndCells);

	private string WDigitsString => Options.Converter.DigitConverter((Mask)(1 << WDigit));
}
