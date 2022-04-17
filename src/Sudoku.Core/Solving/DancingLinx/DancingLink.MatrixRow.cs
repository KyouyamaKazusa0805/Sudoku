namespace Sudoku.Solving.DancingLinx;

partial class DancingLink
{
	/// <summary>
	/// Represents a matrix row.
	/// </summary>
	/// <param name="Cell">Indicates the node that represents the current cell.</param>
	/// <param name="Row">Indicates the nodes at the current row.</param>
	/// <param name="Column">Indicates the nodes at the current column.</param>
	/// <param name="Block">Indicates the nodes at the current block.</param>
	private record struct MatrixRow(DataNode Cell, DataNode Row, DataNode Column, DataNode Block);
}
