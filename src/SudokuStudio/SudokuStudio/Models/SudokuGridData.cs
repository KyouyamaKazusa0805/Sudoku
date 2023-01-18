namespace SudokuStudio.Models;

/// <summary>
/// Defines a sudoku grid data.
/// </summary>
internal sealed class SudokuGridData
{
	/// <summary>
	/// Indicates the format description.
	/// </summary>
	public required string FormatDescription { get; set; }

	/// <summary>
	/// Indicates the grid string.
	/// </summary>
	public required string GridString { get; set; }
}
