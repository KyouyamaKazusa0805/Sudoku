namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a neighbor sign view node.
/// </summary>
public sealed partial class NeighborSignViewNode : SingleCellMarkViewNode
{
	/// <summary>
	/// Initializes a <see cref="NeighborSignViewNode"/> instance via specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="isFourDirections">Indicates whether the sign only records for 4 directions.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public NeighborSignViewNode(Identifier identifier, int cell, bool isFourDirections) : base(identifier, cell, Direction.None)
		=> IsFourDirections = isFourDirections;


	/// <summary>
	/// Indicates whether the sign only records for 4 directions (top-left, top-right, bottom-left and bottom-right).
	/// If <see langword="true"/>, 4 directions; otherwise, 8 directions.
	/// </summary>
	public bool IsFourDirections { get; }

	/// <summary>
	/// Indicates the cell string.
	/// </summary>
	[DebuggerHidden]
	[GeneratedDisplayName(nameof(Cell))]
	private string CellString => CellsMap[Cell].ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is NeighborSignViewNode comparer
		&& Identifier == comparer.Identifier && Cell == comparer.Cell && IsFourDirections == comparer.IsFourDirections;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(Cell), nameof(IsFourDirections))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(CellString), nameof(IsFourDirections))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override NeighborSignViewNode Clone() => new(Identifier, Cell, IsFourDirections);
}
