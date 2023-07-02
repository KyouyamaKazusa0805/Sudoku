namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Defines a star view node used by star product.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class StarProductStarViewNode(ColorIdentifier identifier, Cell cell, Direction directions) :
	SingleCellMarkViewNode(identifier, cell, directions)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is StarProductStarViewNode comparer && Cell == comparer.Cell;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override StarProductStarViewNode Clone() => new(Identifier, Cell, Directions);
}
