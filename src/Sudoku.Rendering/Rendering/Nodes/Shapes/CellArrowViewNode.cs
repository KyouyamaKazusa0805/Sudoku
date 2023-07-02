namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Defines a cell arrow view node.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class CellArrowViewNode(ColorIdentifier identifier, Cell cell, Direction directions) :
	SingleCellMarkViewNode(identifier, cell, directions)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is CellArrowViewNode comparer && Cell == comparer.Cell;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CellArrowViewNode Clone() => new(Identifier, Cell, Directions);
}
