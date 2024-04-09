#if UNIQUE_RECTANGLE_W_WING
namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle with W-Wing</b> technique.
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
public sealed partial class UniqueRectangleWithWWingStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Technique code,
	Digit digit1,
	Digit digit2,
	scoped ref readonly CellMap cells,
	bool isAvoidable,
	[PrimaryConstructorParameter] Digit wDigit,
	[PrimaryConstructorParameter] scoped ref readonly CellMap connectors,
	[PrimaryConstructorParameter] scoped ref readonly CellMap endCells,
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
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [D1Str, D2Str, CellsStr, ConnectorsString, EndCellsString, WDigitsString]),
			new(ChineseLanguage, [D1Str, D2Str, CellsStr, ConnectorsString, EndCellsString, WDigitsString])
		];

	/// <inheritdoc/>
	public override FactorCollection Factors => [new RectangleIsAvoidableFactor()];

	private string ConnectorsString => Options.Converter.CellConverter(Connectors);

	private string EndCellsString => Options.Converter.CellConverter(EndCells);

	private string WDigitsString => Options.Converter.DigitConverter((Mask)(1 << WDigit));
}
#endif
