namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>3-dimensional Sue de Coq</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="rowDigitsMask">Indicates the digits mask that describes which digits are used in this pattern in a row.</param>
/// <param name="columnDigitsMask">Indicates the digits mask that describes which digits are used in this pattern in a column.</param>
/// <param name="blockDigitsMask">Indicates the digits mask that describes which digits are used in this pattern in a block.</param>
/// <param name="rowCells">Indicates the cells used in this pattern in a row.</param>
/// <param name="columnCells">Indicates the cells used in this pattern in a column.</param>
/// <param name="blockCells">Indicates the cells used in this pattern in a block.</param>
public sealed partial class SueDeCoq3DimensionStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] Mask rowDigitsMask,
	[PrimaryConstructorParameter] Mask columnDigitsMask,
	[PrimaryConstructorParameter] Mask blockDigitsMask,
	[PrimaryConstructorParameter] scoped ref readonly CellMap rowCells,
	[PrimaryConstructorParameter] scoped ref readonly CellMap columnCells,
	[PrimaryConstructorParameter] scoped ref readonly CellMap blockCells
) : LockedSetStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 55;

	/// <inheritdoc/>
	public override Technique Code => Technique.SueDeCoq3Dimension;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [Cells1Str, Digits1Str, Cells2Str, Digits2Str, Cells3Str, Digits3Str]),
			new(ChineseLanguage, [Cells1Str, Digits1Str, Cells2Str, Digits2Str, Cells3Str, Digits3Str])
		];

	private string Cells1Str => Options.Converter.CellConverter(RowCells);

	private string Digits1Str => Options.Converter.DigitConverter(RowDigitsMask);

	private string Cells2Str => Options.Converter.CellConverter(ColumnCells);

	private string Digits2Str => Options.Converter.DigitConverter(ColumnDigitsMask);

	private string Cells3Str => Options.Converter.CellConverter(BlockCells);

	private string Digits3Str => Options.Converter.DigitConverter(BlockDigitsMask);
}
