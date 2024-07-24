namespace Sudoku.Concepts;

/// <summary>
/// Represents a cell state.
/// </summary>
/// <remarks><include file="../../global-doc-comments.xml" path="/g/flags-attribute"/></remarks>
[Flags]
public enum CellState : byte
{
	/// <summary>
	/// Indicates that the cell is empty.
	/// </summary>
	Empty = 1 << 0,

	/// <summary>
	/// Indicates the current cell has been filled a value that is not given from initial grid.
	/// </summary>
	Modifiable = 1 << 1,

	/// <summary>
	/// Indicates the current cell has been filled a value that cannot be modified because it exists in initial grid.
	/// </summary>
	Given = 1 << 2
}
