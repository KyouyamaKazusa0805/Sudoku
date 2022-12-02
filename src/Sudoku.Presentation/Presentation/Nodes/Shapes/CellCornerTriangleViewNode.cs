namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a triangle view node that is used in a cell.
/// </summary>
public sealed class CellCornerTriangleViewNode : SingleCellMarkViewNode
{
	/// <summary>
	/// Initializes a <see cref="CellCornerTriangleViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="directions">The directions.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CellCornerTriangleViewNode(Identifier identifier, int cell, Direction directions) : base(identifier, cell, directions)
	{
		var condition = directions is Direction.TopLeft or Direction.TopRight or Direction.BottomLeft or Direction.BottomRight;
		Argument.ThrowIfFalse(condition, $"Argument '{nameof(directions)}' must point to cell corner.");
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CellCornerTriangleViewNode Clone() => new(Identifier, Cell, Directions);
}
