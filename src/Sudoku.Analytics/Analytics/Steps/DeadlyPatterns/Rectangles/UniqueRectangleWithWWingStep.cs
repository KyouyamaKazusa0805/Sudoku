using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Text;
using Sudoku.Text.Notation;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle with W-Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
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
	Technique code,
	Digit digit1,
	Digit digit2,
	scoped ref readonly CellMap cells,
	bool isAvoidable,
	[DataMember] Digit wDigit,
	[DataMember] scoped ref readonly CellMap connectors,
	[DataMember] scoped ref readonly CellMap endCells,
	int absoluteOffset
) : UniqueRectangleStep(
	conclusions,
	views,
	code,
	digit1,
	digit2,
	in cells,
	isAvoidable,
	absoluteOffset
)
{
	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => [new(ExtraDifficultyCaseNames.Avoidable, IsAvoidable ? .1M : 0)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [D1Str, D2Str, CellsStr, ConnectorsString, EndCellsString, WDigitsString]),
			new(ChineseLanguage, [D1Str, D2Str, CellsStr, ConnectorsString, EndCellsString, WDigitsString])
		];

	private string ConnectorsString => CellNotation.ToCollectionString(Connectors);

	private string EndCellsString => CellNotation.ToCollectionString(EndCells);

	private string WDigitsString => DigitNotation.ToString(WDigit);
}
