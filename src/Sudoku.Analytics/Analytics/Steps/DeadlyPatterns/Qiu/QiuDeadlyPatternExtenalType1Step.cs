using System.SourceGeneration;
using Sudoku.Rendering;
using Sudoku.Text;
using Sudoku.Text.Notation;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern External Type 1</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="is2LinesWith2Cells"><inheritdoc/></param>
/// <param name="houses"><inheritdoc/></param>
/// <param name="corner1"><inheritdoc/></param>
/// <param name="corner2"><inheritdoc/></param>
/// <param name="targetCell">Indicates the target cell.</param>
/// <param name="targetDigits">Indicates the target digits.</param>
public sealed partial class QiuDeadlyPatternExtenalType1Step(
	Conclusion[] conclusions,
	View[]? views,
	bool is2LinesWith2Cells,
	HouseMask houses,
	Cell? corner1,
	Cell? corner2,
	[DataMember] Cell targetCell,
	[DataMember] Mask targetDigits
) : QiuDeadlyPatternExternalTypeStep(conclusions, views, is2LinesWith2Cells, houses, corner1, corner2)
{
	/// <inheritdoc/>
	public override int Type => 1;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [PatternStr, DigitsStr, CellStr]), new(ChineseLanguage, [PatternStr, CellStr, DigitsStr])];

	private string CellStr => CellNotation.ToString(TargetCell);

	private string DigitsStr => DigitNotation.ToString(TargetDigits);
}
