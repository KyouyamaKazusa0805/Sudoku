namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a XV sign view node.
/// </summary>
public sealed partial class XvSignViewNode(Identifier identifier, int cell1, int cell2, bool isX) :
	AdjacentCellMarkViewNode(identifier, cell1, cell2)
{
	/// <summary>
	/// Indicates whether the mark is <c>X</c>. If <see langword="true"/>, <c>X</c>; otherwise, <c>V</c>.
	/// </summary>
	public bool IsX { get; } = isX;

	/// <summary>
	/// Indicates the cell 1 string.
	/// </summary>
	[GeneratedDisplayName(nameof(Cell1))]
	private string Cell1String => CellsMap[Cell1].ToString();

	/// <summary>
	/// Indicates the cell 2 string.
	/// </summary>
	[GeneratedDisplayName(nameof(Cell2))]
	private string Cell2String => CellsMap[Cell2].ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is XvSignViewNode comparer && Cell1 == comparer.Cell1 && Cell2 == comparer.Cell2;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(Cell1), nameof(Cell2))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cell1String), nameof(Cell2String), nameof(IsX))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override XvSignViewNode Clone() => new(Identifier, Cell1, Cell2, IsX);
}
