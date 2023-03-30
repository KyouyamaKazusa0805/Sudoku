namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a cell corner arrow view node.
/// </summary>
public sealed partial class CellCornerArrowViewNode(Identifier identifier, int cell, Direction directions) :
	SingleCellMarkViewNode(identifier, cell, directions)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is CellCornerArrowViewNode comparer && Cell == comparer.Cell;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(Cell))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cell), nameof(Directions))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CellCornerArrowViewNode Clone() => new(Identifier, Cell, Directions);
}
