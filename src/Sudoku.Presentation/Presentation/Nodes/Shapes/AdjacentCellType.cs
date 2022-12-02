namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines an adjacent cell type.
/// </summary>
[Flags]
public enum AdjacentCellType : byte
{
	/// <summary>
	/// Indicates the default value.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the adjacent cell type is rowish.
	/// </summary>
	Rowish = 1,

	/// <summary>
	/// Indicates the adjacent cell type is columnish.
	/// </summary>
	Columnish = 1 << 1
}
