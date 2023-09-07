using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Rendering;
using Sudoku.Text;
using Sudoku.Text.Notation;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle 2D (or 3X)</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="code"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="isAvoidable"><inheritdoc/></param>
/// <param name="xDigit">Indicates the digit X defined in this pattern.</param>
/// <param name="yDigit">Indicates the digit Y defined in this pattern.</param>
/// <param name="xyCell">Indicates a bi-value cell that only contains digit X and Y.</param>
/// <param name="absoluteOffset"><inheritdoc/></param>
public sealed partial class UniqueRectangle2DOr3XStep(
	Conclusion[] conclusions,
	View[]? views,
	Technique code,
	Digit digit1,
	Digit digit2,
	scoped in CellMap cells,
	bool isAvoidable,
	[DataMember] Digit xDigit,
	[DataMember] Digit yDigit,
	[DataMember] Cell xyCell,
	int absoluteOffset
) : UniqueRectangleStep(conclusions, views, code, digit1, digit2, cells, isAvoidable, absoluteOffset)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .2M;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [D1Str, D2Str, CellsStr, XDigitStr, YDigitStr, XYCellsStr]),
			new(ChineseLanguage, [D1Str, D2Str, CellsStr, XDigitStr, YDigitStr, XYCellsStr])
		];

	private string XDigitStr => DigitNotation.ToString(XDigit);

	private string YDigitStr => DigitNotation.ToString(YDigit);

	private string XYCellsStr => CellNotation.ToString(XyCell);
}
