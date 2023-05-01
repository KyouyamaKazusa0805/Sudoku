namespace SudokuStudio.Configuration;

/// <summary>
/// Defines a sudoku grid serialization data.
/// </summary>
public sealed class GridSerializationData
{
	/// <summary>
	/// Indicates the grid string.
	/// </summary>
	public required string GridString { get; set; }
}
