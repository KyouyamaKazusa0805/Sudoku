namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Defines a triangle view node that is used in a cell.
/// </summary>
public sealed class CellCornerTriangleViewNode(ColorIdentifier identifier, Cell cell, Direction directions) :
	SingleCellMarkViewNode(identifier, cell, directions)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is CellCornerTriangleViewNode comparer && Cell == comparer.Cell;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CellCornerTriangleViewNode Clone() => new(Identifier, Cell, Directions);
}
