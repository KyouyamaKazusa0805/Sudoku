using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Text;
using static Sudoku.Analytics.Strings.StringsAccessor;

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
	[DataMember] Mask rowDigitsMask,
	[DataMember] Mask columnDigitsMask,
	[DataMember] Mask blockDigitsMask,
	[DataMember] scoped ref readonly CellMap rowCells,
	[DataMember] scoped ref readonly CellMap columnCells,
	[DataMember] scoped ref readonly CellMap blockCells
) : ZeroRankStep(conclusions, views, options)
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

	private string Cells1Str => Options.CoordinateConverter.CellConverter(RowCells);

	private string Digits1Str => Options.CoordinateConverter.DigitConverter(RowDigitsMask);

	private string Cells2Str => Options.CoordinateConverter.CellConverter(ColumnCells);

	private string Digits2Str => Options.CoordinateConverter.DigitConverter(ColumnDigitsMask);

	private string Cells3Str => Options.CoordinateConverter.CellConverter(BlockCells);

	private string Digits3Str => Options.CoordinateConverter.DigitConverter(BlockDigitsMask);
}
