namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>3-dimensional Sue de Coq</b> technique.
/// </summary>
public sealed class SueDeCoq3DimensionStep(
	Conclusion[] conclusions,
	View[]? views,
	Mask rowDigitsMask,
	Mask columnDigitsMask,
	Mask blockDigitsMask,
	scoped in CellMap rowCells,
	scoped in CellMap columnCells,
	scoped in CellMap blockCells
) : ZeroRankStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 5.5M;

	/// <summary>
	/// Indicates the digits mask that describes which digits are used in this pattern in a row.
	/// </summary>
	public Mask RowDigitsMask { get; } = rowDigitsMask;

	/// <summary>
	/// Indicates the digits mask that describes which digits are used in this pattern in a column.
	/// </summary>
	public Mask ColumnDigitsMask { get; } = columnDigitsMask;

	/// <summary>
	/// Indicates the digits mask that describes which digits are used in this pattern in a block.
	/// </summary>
	public Mask BlockDigitsMask { get; } = blockDigitsMask;

	/// <inheritdoc/>
	public override Technique Code => Technique.SueDeCoq3Dimension;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <summary>
	/// Indicates the cells used in this pattern in a row.
	/// </summary>
	public CellMap RowCells { get; } = rowCells;

	/// <summary>
	/// Indicates the cells used in this pattern in a column.
	/// </summary>
	public CellMap ColumnCells { get; } = columnCells;

	/// <summary>
	/// Indicates the cells used in this pattern in a block.
	/// </summary>
	public CellMap BlockCells { get; } = blockCells;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { Cells1Str, Digits1Str, Cells2Str, Digits2Str, Cells3Str, Digits3Str } },
			{ "zh", new[] { Cells1Str, Digits1Str, Cells2Str, Digits2Str, Cells3Str, Digits3Str } }
		};

	private string Cells1Str => RowCells.ToString();

	private string Digits1Str => DigitMaskFormatter.Format(RowDigitsMask, FormattingMode.Normal);

	private string Cells2Str => ColumnCells.ToString();

	private string Digits2Str => DigitMaskFormatter.Format(ColumnDigitsMask, FormattingMode.Normal);

	private string Cells3Str => BlockCells.ToString();

	private string Digits3Str => DigitMaskFormatter.Format(BlockDigitsMask, FormattingMode.Normal);
}
