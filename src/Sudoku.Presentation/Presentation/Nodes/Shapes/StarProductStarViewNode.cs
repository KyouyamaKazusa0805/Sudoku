namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a star view node used by star product.
/// </summary>
public sealed partial class StarProductStarViewNode(Identifier identifier, int cell, Direction directions) :
	SingleCellMarkViewNode(identifier, cell, directions)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is StarProductStarViewNode comparer && Cell == comparer.Cell;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(Cell))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cell), nameof(Directions))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override StarProductStarViewNode Clone() => new(Identifier, Cell, Directions);
}
