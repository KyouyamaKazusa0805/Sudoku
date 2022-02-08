namespace Sudoku.UI;

/// <summary>
/// Indicates the tags that applies to the controls in a <see cref="Canvas"/> to display a sudoku grid.
/// </summary>
internal static class SudokuCanvasTags
{
	/// <summary>
	/// Indicates the border lines.
	/// </summary>
	public const string BorderLines = nameof(BorderLines);

	/// <summary>
	/// Indicates the outside border lines.
	/// </summary>
	public const string OutsideBorderLines = nameof(OutsideBorderLines);

	/// <summary>
	/// Indicates the block border lines.
	/// </summary>
	public const string BlockBorderLines = nameof(BlockBorderLines);

	/// <summary>
	/// Indicates the cell border lines.
	/// </summary>
	public const string CellBorderLines = nameof(CellBorderLines);

	/// <summary>
	/// Indicates the candidate border lines.
	/// </summary>
	public const string CandidateBorderLines = nameof(CandidateBorderLines);

	/// <summary>
	/// Indicates the horizontal border lines.
	/// </summary>
	public const string HorizontalBorderLines = nameof(HorizontalBorderLines);

	/// <summary>
	/// Indicates the vertical border lines.
	/// </summary>
	public const string VerticalBorderLines = nameof(VerticalBorderLines);
}
