using System.SourceGeneration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Text;
using Sudoku.Text.Notation;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bivalue Oddagon Type 2</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="loopCells"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="extraDigit">Indicates the extra digit used.</param>
public sealed partial class BivalueOddagonType2Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped ref readonly CellMap loopCells,
	Digit digit1,
	Digit digit2,
	[DataMember] Digit extraDigit
) : BivalueOddagonStep(conclusions, views, in loopCells, digit1, digit2)
{
	/// <inheritdoc/>
	public override int Type => 2;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => [new(ExtraDifficultyCaseNames.ExtraDigit, .1M)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [ExtraDigitStr, LoopStr]), new(ChineseLanguage, [LoopStr, ExtraDigitStr])];

	private string ExtraDigitStr => DigitNotation.ToString(ExtraDigit);
}
