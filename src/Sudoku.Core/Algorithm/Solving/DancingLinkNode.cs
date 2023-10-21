using System.Runtime.CompilerServices;

namespace Sudoku.Algorithm.Solving;

/// <summary>
/// Represents a dancing link node.
/// </summary>
public class DancingLinkNode
{
	/// <summary>
	/// Initializes a <see cref="DancingLinkNode"/> instance via the specified ID value and the column node.
	/// </summary>
	/// <param name="id">The ID value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DancingLinkNode(Offset id) => (Id, Column, Left, Right, Up, Down) = (id, null, this, this, this, this);

	/// <summary>
	/// Initializes a <see cref="DancingLinkNode"/> instance via the specified ID value and the column node.
	/// </summary>
	/// <param name="id">The ID value.</param>
	/// <param name="column">The column node.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DancingLinkNode(Offset id, ColumnNode? column) => (Id, Column, Left, Right, Up, Down) = (id, column, this, this, this, this);


	/// <summary>
	/// Indicates the ID of the node.
	/// </summary>
	public Offset Id { get; set; }

	/// <summary>
	/// Indicates the current column node.
	/// </summary>
	public ColumnNode? Column { get; set; }

	/// <summary>
	/// Indicates the pointer that points to the left node.
	/// </summary>
	public DancingLinkNode Left { get; set; }

	/// <summary>
	/// Indicates the pointer that points to the right node.
	/// </summary>
	public DancingLinkNode Right { get; set; }

	/// <summary>
	/// Indicates the pointer that points to the up node.
	/// </summary>
	public DancingLinkNode Up { get; set; }

	/// <summary>
	/// Indicates the pointer that points to the down node.
	/// </summary>
	public DancingLinkNode Down { get; set; }
}
