namespace Sudoku.Solving.Algorithms.DancingLinx;

/// <summary>
/// Represents a data node.
/// </summary>
internal class DataNode
{
	/// <summary>
	/// Initializes a <see cref="DataNode"/> instance via the specified ID value and the column node.
	/// </summary>
	/// <param name="id">The ID value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected internal DataNode(int id) => (Id, Column, Left, Right, Up, Down) = (id, null, this, this, this, this);

	/// <summary>
	/// Initializes a <see cref="DataNode"/> instance via the specified ID value and the column node.
	/// </summary>
	/// <param name="id">The ID value.</param>
	/// <param name="column">The column node.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected internal DataNode(int id, ColumnNode? column) => (Id, Column, Left, Right, Up, Down) = (id, column, this, this, this, this);


	/// <summary>
	/// Indicates the ID of the node.
	/// </summary>
	public int Id { get; set; }

	/// <summary>
	/// Indicates the current column node.
	/// </summary>
	public ColumnNode? Column { get; set; }

	/// <summary>
	/// Indicates the pointer that points to the left node.
	/// </summary>
	public DataNode Left { get; set; }

	/// <summary>
	/// Indicates the pointer that points to the right node.
	/// </summary>
	public DataNode Right { get; set; }

	/// <summary>
	/// Indicates the pointer that points to the up node.
	/// </summary>
	public DataNode Up { get; set; }

	/// <summary>
	/// Indicates the pointer that points to the down node.
	/// </summary>
	public DataNode Down { get; set; }
}
