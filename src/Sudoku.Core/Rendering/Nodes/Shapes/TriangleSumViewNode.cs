namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Defines a triangle-sum view node.
/// </summary>
public sealed partial class TriangleSumViewNode(Identifier identifier, int cell, Direction directions) :
	SingleCellMarkViewNode(identifier, cell, directions)
{
	/// <summary>
	/// Determines whether the shape is full complement.
	/// </summary>
	public bool IsComplement => Directions is (Direction.TopLeft | Direction.BottomRight) or (Direction.TopRight | Direction.BottomLeft);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is TriangleSumViewNode comparer && Cell == comparer.Cell;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(Cell))]
	public override partial int GetHashCode();

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cell), nameof(Directions))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override TriangleSumViewNode Clone() => new(Identifier, Cell, Directions);
}
