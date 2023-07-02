namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Represents with a greater-than sign view node.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class GreaterThanSignViewNode(
	ColorIdentifier identifier,
	Cell cell1,
	Cell cell2,
	[PrimaryConstructorParameter, StringMember] bool isGreaterThan
) : AdjacentCellMarkViewNode(identifier, cell1, cell2)
{
	/// <summary>
	/// Indicates the cell 1 string.
	/// </summary>
	[StringMember(nameof(Cell1))]
	private string Cell1String => CellsMap[Cell1].ToString();

	/// <summary>
	/// Indicates the cell 2 string.
	/// </summary>
	[StringMember(nameof(Cell2))]
	private string Cell2String => CellsMap[Cell2].ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is GreaterThanSignViewNode comparer && Cell1 == comparer.Cell1 && Cell2 == comparer.Cell2;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override GreaterThanSignViewNode Clone() => new(Identifier, Cell1, Cell2, IsGreaterThan);
}
