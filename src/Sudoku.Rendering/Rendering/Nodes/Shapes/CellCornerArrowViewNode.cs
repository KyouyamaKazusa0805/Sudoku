namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Defines a cell corner arrow view node.
/// </summary>
public sealed partial class CellCornerArrowViewNode(ColorIdentifier identifier, Cell cell, Direction directions) :
	SingleCellMarkViewNode(identifier, cell, directions)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is CellCornerArrowViewNode comparer && Cell == comparer.Cell;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(Cell))]
	public override partial int GetHashCode();

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cell), nameof(Directions))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CellCornerArrowViewNode Clone() => new(Identifier, Cell, Directions);
}
