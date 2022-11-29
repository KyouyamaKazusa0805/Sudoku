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

	/// <summary>
	/// Indicates the cell 1 string.
	/// </summary>
	[DebuggerHidden]
	[GeneratedDisplayName(nameof(Cell1))]
	private string Cell1String => CellsMap[Cell1].ToString();

	/// <summary>
	/// Indicates the cell 2 string.
	/// </summary>
	[DebuggerHidden]
	[GeneratedDisplayName(nameof(Cell2))]
	private string Cell2String => CellsMap[Cell2].ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is KropkiDotViewNode comparer
		&& Identifier == comparer.Identifier
		&& Cell1 == comparer.Cell1 && Cell2 == comparer.Cell2 && IsSolid == comparer.IsSolid;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(Identifier), nameof(Cell1), nameof(Cell2), nameof(IsSolid))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cell1String), nameof(Cell2String), nameof(IsSolid))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override KropkiDotViewNode Clone() => new(Identifier, Cell1, Cell2, IsSolid);
}
