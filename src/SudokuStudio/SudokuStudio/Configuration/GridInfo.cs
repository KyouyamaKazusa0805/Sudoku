namespace SudokuStudio.Configuration;

/// <summary>
/// Represents a type that defines the details of a grid.
/// </summary>
public sealed class GridInfo
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
