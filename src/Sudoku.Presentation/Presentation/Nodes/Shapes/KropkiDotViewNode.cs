namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Represents with a Kropki dot view node.
/// </summary>
public sealed partial class KropkiDotViewNode : AdjacentCellMarkViewNode
{
	/// <summary>
	/// Initializes a <see cref="KropkiDotViewNode"/> instance via the identifier and two adjacent cells.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cell1">The cell 1.</param>
	/// <param name="cell2">The cell 2.</param>
	/// <param name="isSolid">Indicates whether the dot is solid.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public KropkiDotViewNode(Identifier identifier, int cell1, int cell2, bool isSolid) : base(identifier, cell1, cell2) => IsSolid = isSolid;


	/// <summary>
	/// Indicates whether the dot will be displayed as solid one.
	/// </summary>
	public bool IsSolid { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is KropkiDotViewNode comparer
		&& Identifier == comparer.Identifier
		&& Cell1 == comparer.Cell1 && Cell2 == comparer.Cell2 && IsSolid == comparer.IsSolid;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(Identifier), nameof(Cell1), nameof(Cell2), nameof(IsSolid))]
	public override partial int GetHashCode();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
		=> $$"""{{nameof(KropkiDotViewNode)}} { {{nameof(Cell1)}} = {{CellsMap[Cell1]}}, {{nameof(Cell2)}} = {{CellsMap[Cell2]}}, {{nameof(IsSolid)}} = {{IsSolid}} }""";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override KropkiDotViewNode Clone() => new(Identifier, Cell1, Cell2, IsSolid);
}
