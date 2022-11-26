namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Represents with a Kropki dot view node.
/// </summary>
public sealed partial class KropkiDotViewNode : ViewNode
{
	/// <summary>
	/// Initializes a <see cref="BorderBarViewNode"/> instance via the identifier and two adjacent cells.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cell1">The cell 1.</param>
	/// <param name="cell2">The cell 2.</param>
	/// <param name="isSolid">Indicates whether the dot is solid.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public KropkiDotViewNode(Identifier identifier, int cell1, int cell2, bool isSolid) : base(identifier)
	{
		Argument.ThrowIfFalse(cell1 is >= 0 and < 81, "The argument must be between 0 and 80.");
		Argument.ThrowIfFalse(cell2 is >= 0 and < 81, "The argument must be between 0 and 80.");
		Argument.ThrowIfFalse(cell1 < cell2, $"The argument '{nameof(cell1)}' must be lower than another argument '{nameof(cell2)}'.");
		Argument.ThrowIfFalse(cell2 - cell1 is 1 or 9, $"Two cells '{nameof(cell1)}' and '{nameof(cell2)}' must be adjacent with each other.");

		(Cell1, Cell2, IsSolid) = (cell1, cell2, isSolid);
	}


	/// <summary>
	/// Indicates whether two cells <see cref="Cell1"/> and <see cref="Cell2"/> are adjacent by row.
	/// If <see langword="true"/>, row; otherwise, column.
	/// </summary>
	public bool IsRow => Cell2 - Cell1 == 1;

	/// <summary>
	/// Indicates whether the dot will be displayed as solid one.
	/// </summary>
	public bool IsSolid { get; }

	/// <inheritdoc cref="BorderBarViewNode.Cell1"/>
	public int Cell1 { get; }

	/// <inheritdoc cref="BorderBarViewNode.Cell2"/>
	public int Cell2 { get; }

	/// <inheritdoc/>
	protected override string TypeIdentifier => nameof(KropkiDotViewNode);


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
		=> $$"""{{TypeIdentifier}} { {{nameof(Cell1)}} = {{CellsMap[Cell1]}}, {{nameof(Cell2)}} = {{CellsMap[Cell2]}}, {{nameof(IsSolid)}} = {{IsSolid}} }""";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override KropkiDotViewNode Clone() => new(Identifier, Cell1, Cell2, IsSolid);
}
