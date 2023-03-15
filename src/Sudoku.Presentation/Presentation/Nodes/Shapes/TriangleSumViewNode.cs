namespace Sudoku.Presentation.Nodes.Shapes;

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
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is TriangleSumViewNode comparer
		&& Identifier == comparer.Identifier && Cell == comparer.Cell && Directions == comparer.Directions;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(Cell), nameof(Directions))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cell), nameof(Directions))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override TriangleSumViewNode Clone() => new(Identifier, Cell, Directions);
}
