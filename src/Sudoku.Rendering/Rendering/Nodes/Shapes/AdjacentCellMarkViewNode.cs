namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Represents a view node that describes a shape that is a mark, displayed between two adjacent cells.
/// </summary>
public abstract partial class AdjacentCellMarkViewNode(ColorIdentifier identifier, int cell1, int cell2) : ShapeViewNode(identifier)
{
	/// <summary>
	/// Indicates whether two cells <see cref="Cell1"/> and <see cref="Cell2"/> are adjacent by row.
	/// If <see langword="true"/>, row; otherwise, column.
	/// </summary>
	public bool IsRow => Cell2 - Cell1 == 1;

	/// <summary>
	/// Indicates the first cell used.
	/// </summary>
	public int Cell1 { get; } = cell1;

	/// <summary>
	/// Indicates the second cell used. This cell should be adjacent with <see cref="Cell1"/>.
	/// </summary>
	public int Cell2 { get; } = cell2;


	[DeconstructionMethod]
	public partial void Deconstruct(out int cell1, out int cell2);

	[DeconstructionMethod]
	public partial void Deconstruct(out int cell1, out int cell2, out bool isRow);
}
