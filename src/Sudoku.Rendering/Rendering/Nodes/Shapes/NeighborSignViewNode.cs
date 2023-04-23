namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Defines a neighbor sign view node.
/// </summary>
public sealed partial class NeighborSignViewNode(ColorIdentifier identifier, Cell cell, bool isFourDirections) :
	SingleCellMarkViewNode(identifier, cell, Direction.None)
{
	/// <summary>
	/// Indicates whether the sign only records for 4 directions (top-left, top-right, bottom-left and bottom-right).
	/// If <see langword="true"/>, 4 directions; otherwise, 8 directions.
	/// </summary>
	public bool IsFourDirections { get; } = isFourDirections;

	/// <summary>
	/// Indicates the cell string.
	/// </summary>
	[GeneratedDisplayName(nameof(Cell))]
	private string CellString => CellsMap[Cell].ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is NeighborSignViewNode comparer && Cell == comparer.Cell;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(Cell))]
	public override partial int GetHashCode();

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(CellString), nameof(IsFourDirections))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override NeighborSignViewNode Clone() => new(Identifier, Cell, IsFourDirections);
}
