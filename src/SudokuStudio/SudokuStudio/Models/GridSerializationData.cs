namespace SudokuStudio.Models;

/// <summary>
/// Defines a sudoku grid serialization data.
/// </summary>
public sealed class GridSerializationData
{
	/// <summary>
	/// Indicates the format description.
	/// </summary>
	public string FormatDescription { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the grid string.
	/// </summary>
	public required string GridString { get; set; }
}
