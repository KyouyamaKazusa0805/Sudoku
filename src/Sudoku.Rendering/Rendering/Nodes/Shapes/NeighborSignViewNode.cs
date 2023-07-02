namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Defines a neighbor sign view node.
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
/// <param name="cell"><inheritdoc/></param>
/// <param name="isFourDirections">
/// Indicates whether the sign only records for 4 directions (top-left, top-right, bottom-left and bottom-right).
/// If <see langword="true"/>, 4 directions; otherwise, 8 directions.
/// </param>
[GetHashCode]
[ToString]
public sealed partial class NeighborSignViewNode(
	ColorIdentifier identifier,
	Cell cell,
	[PrimaryConstructorParameter, StringMember] bool isFourDirections
) : SingleCellMarkViewNode(identifier, cell, Direction.None)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is NeighborSignViewNode comparer && Cell == comparer.Cell;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override NeighborSignViewNode Clone() => new(Identifier, Cell, IsFourDirections);
}
