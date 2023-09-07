using System.SourceGeneration;
using Sudoku.Analytics.Rating;
using Sudoku.Rendering;
using Sudoku.Text;
using Sudoku.Text.Notation;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Extended Rectangle Type 2</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="extraDigit">Indicates the extra digit used.</param>
public sealed partial class ExtendedRectangleType2Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped in CellMap cells,
	Mask digitsMask,
	[DataMember] Digit extraDigit
) : ExtendedRectangleStep(conclusions, views, cells, digitsMask)
{
	/// <inheritdoc/>
	public override int Type => 2;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> [.. base.ExtraDifficultyCases, new(ExtraDifficultyCaseNames.ExtraDigit, .1M)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [DigitsStr, CellsStr, ExtraDigitStr]), new(ChineseLanguage, [DigitsStr, CellsStr, ExtraDigitStr])];

	private string ExtraDigitStr => DigitNotation.ToString(ExtraDigit);
}
