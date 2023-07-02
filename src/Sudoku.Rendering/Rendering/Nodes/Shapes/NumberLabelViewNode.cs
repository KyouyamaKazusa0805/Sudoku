namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Defines a number label view node.
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
/// <param name="cell1"><inheritdoc/></param>
/// <param name="cell2"><inheritdoc/></param>
/// <param name="label">
/// <para>Indicates the target value you want to display.</para>
/// <para><i>
/// Please note that the target value is a <see cref="string"/> value. We don't limit you only input numbers <b>on purpose</b>.
/// You can use instances of the current type to display what you want to display.
/// </i></para>
/// </param>
[GetHashCode]
[ToString]
public sealed partial class NumberLabelViewNode(
	ColorIdentifier identifier,
	Cell cell1,
	Cell cell2,
	[PrimaryConstructorParameter, StringMember] string label
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
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is NumberLabelViewNode comparer && Cell1 == comparer.Cell1 && Cell2 == comparer.Cell2;

	/// <inheritdoc/>
	public override NumberLabelViewNode Clone() => new(Identifier, Cell1, Cell2, Label);
}
