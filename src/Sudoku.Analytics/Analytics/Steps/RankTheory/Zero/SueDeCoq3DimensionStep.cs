namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>3-dimensional Sue de Coq</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="rowDigitsMask">Indicates the digits mask that describes which digits are used in this pattern in a row.</param>
/// <param name="columnDigitsMask">Indicates the digits mask that describes which digits are used in this pattern in a column.</param>
/// <param name="blockDigitsMask">Indicates the digits mask that describes which digits are used in this pattern in a block.</param>
/// <param name="rowCells">Indicates the cells used in this pattern in a row.</param>
/// <param name="columnCells">Indicates the cells used in this pattern in a column.</param>
/// <param name="blockCells">Indicates the cells used in this pattern in a block.</param>
public sealed partial class SueDeCoq3DimensionStep(
	Conclusion[] conclusions,
	View[]? views,
	[PrimaryConstructorParameter] Mask rowDigitsMask,
	[PrimaryConstructorParameter] Mask columnDigitsMask,
	[PrimaryConstructorParameter] Mask blockDigitsMask,
	[PrimaryConstructorParameter] scoped in CellMap rowCells,
	[PrimaryConstructorParameter] scoped in CellMap columnCells,
	[PrimaryConstructorParameter] scoped in CellMap blockCells
) : ZeroRankStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 5.5M;

	/// <inheritdoc/>
	public override Technique Code => Technique.SueDeCoq3Dimension;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [Cells1Str, Digits1Str, Cells2Str, Digits2Str, Cells3Str, Digits3Str]),
			new(ChineseLanguage, [Cells1Str, Digits1Str, Cells2Str, Digits2Str, Cells3Str, Digits3Str])
		];

	private string Cells1Str => RowCells.ToString();

	private string Digits1Str => DigitMaskFormatter.Format(RowDigitsMask, FormattingMode.Normal);

	private string Cells2Str => ColumnCells.ToString();

	private string Digits2Str => DigitMaskFormatter.Format(ColumnDigitsMask, FormattingMode.Normal);

	private string Cells3Str => BlockCells.ToString();

	private string Digits3Str => DigitMaskFormatter.Format(BlockDigitsMask, FormattingMode.Normal);
}
