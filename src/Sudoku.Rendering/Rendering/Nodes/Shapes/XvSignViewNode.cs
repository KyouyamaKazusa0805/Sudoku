namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Defines a XV sign view node.
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
/// <param name="cell1"><inheritdoc/></param>
/// <param name="cell2"><inheritdoc/></param>
/// <param name="isX">Indicates whether the mark is <c>X</c>. If <see langword="true"/>, <c>X</c>; otherwise, <c>V</c>.</param>
[GetHashCode]
[ToString]
public sealed partial class XvSignViewNode(
	ColorIdentifier identifier,
	Cell cell1,
	Cell cell2,
	[PrimaryConstructorParameter, StringMember] bool isX
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
		=> other is XvSignViewNode comparer && Cell1 == comparer.Cell1 && Cell2 == comparer.Cell2;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override XvSignViewNode Clone() => new(Identifier, Cell1, Cell2, IsX);
}
