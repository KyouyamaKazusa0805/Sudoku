namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Defines a view node that displays as a bar on shared border line of two adjacent cells.
/// </summary>
public sealed partial class BorderBarViewNode(ColorIdentifier identifier, Cell cell1, Cell cell2) :
	AdjacentCellMarkViewNode(identifier, cell1, cell2)
{
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
		=> other is BorderBarViewNode comparer && Cell1 == comparer.Cell1 && Cell2 == comparer.Cell2;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(Cell1), nameof(Cell2))]
	public override partial int GetHashCode();

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cell1String), nameof(Cell2String))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override BorderBarViewNode Clone() => new(Identifier, Cell1, Cell2);
}
