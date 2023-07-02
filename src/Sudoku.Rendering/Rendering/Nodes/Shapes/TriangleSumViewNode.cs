namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Defines a triangle-sum view node.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class TriangleSumViewNode(ColorIdentifier identifier, Cell cell, Direction directions) :
	SingleCellMarkViewNode(identifier, cell, directions)
{
	/// <summary>
	/// Determines whether the shape is full complement.
	/// </summary>
	public bool IsComplement => Directions is (Direction.TopLeft | Direction.BottomRight) or (Direction.TopRight | Direction.BottomLeft);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is TriangleSumViewNode comparer && Cell == comparer.Cell;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override TriangleSumViewNode Clone() => new(Identifier, Cell, Directions);
}
