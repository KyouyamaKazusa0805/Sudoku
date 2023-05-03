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

	/// <summary>
	/// Indicates the data that represents the conclusions and view nodes.
	/// </summary>
	public UserDefinedRenderable? RenderableData { get; set; }
}
