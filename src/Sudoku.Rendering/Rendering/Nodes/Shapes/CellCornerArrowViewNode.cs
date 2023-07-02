namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Defines a cell corner arrow view node.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class CellCornerArrowViewNode(ColorIdentifier identifier, Cell cell, Direction directions) :
	SingleCellMarkViewNode(identifier, cell, directions)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is CellCornerArrowViewNode comparer && Cell == comparer.Cell;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CellCornerArrowViewNode Clone() => new(Identifier, Cell, Directions);
}
