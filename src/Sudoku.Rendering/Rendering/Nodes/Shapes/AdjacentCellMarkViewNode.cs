namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Represents a view node that describes a shape that is a mark, displayed between two adjacent cells.
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
/// <param name="cell1">Indicates the first cell used.</param>
/// <param name="cell2">Indicates the second cell used. This cell should be adjacent with <see cref="Cell1"/></param>
public abstract partial class AdjacentCellMarkViewNode(
	ColorIdentifier identifier,
	[PrimaryConstructorParameter, HashCodeMember] Cell cell1,
	[PrimaryConstructorParameter, HashCodeMember] Cell cell2
) : ShapeViewNode(identifier)
{
	/// <summary>
	/// Indicates whether two cells <see cref="Cell1"/> and <see cref="Cell2"/> are adjacent by row.
	/// If <see langword="true"/>, row; otherwise, column.
	/// </summary>
	public bool IsRow => Cell2 - Cell1 == 1;


	[DeconstructionMethod]
	public partial void Deconstruct(out Cell cell1, out Cell cell2);

	[DeconstructionMethod]
	public partial void Deconstruct(out Cell cell1, out Cell cell2, out bool isRow);
}
