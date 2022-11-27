namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a XV sign view node.
/// </summary>
public sealed partial class XvSignViewNode : AdjacentCellMarkViewNode
{
	/// <summary>
	/// Initializes an <see cref="XvSignViewNode"/> instance via identifier and cells.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cell1">The cell 1.</param>
	/// <param name="cell2">The cell 2.</param>
	/// <param name="isX">Indicates whether the mark is <c>X</c>.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public XvSignViewNode(Identifier identifier, int cell1, int cell2, bool isX) : base(identifier, cell1, cell2) => IsX = isX;


	/// <summary>
	/// Indicates whether the mark is <c>X</c>. If <see langword="true"/>, <c>X</c>; otherwise, <c>V</c>.
	/// </summary>
	public bool IsX { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is XvSignViewNode comparer
		&& Identifier == comparer.Identifier
		&& Cell1 == comparer.Cell1 && Cell2 == comparer.Cell2 && IsX == comparer.IsX;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(Cell1), nameof(Cell2), nameof(IsX))]
	public override partial int GetHashCode();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
		=> $$"""{{nameof(XvSignViewNode)}} { {{nameof(Cell1)}} = {{CellsMap[Cell1]}}, {{nameof(Cell2)}} = {{CellsMap[Cell2]}}, {{nameof(IsX)}} = {{IsX}} }""";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override XvSignViewNode Clone() => new(Identifier, Cell1, Cell2, IsX);
}
