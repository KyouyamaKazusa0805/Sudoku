namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Represents a view node that describes a shape that is a mark, displayed between two adjacent cells.
/// </summary>
public abstract partial class AdjacentCellMarkViewNode : ShapeViewNode
{
	/// <summary>
	/// Assigns <see cref="Identifier"/> instance and two adjacent cells' indices into the type.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cell1">The cell 1.</param>
	/// <param name="cell2">The cell 2.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected AdjacentCellMarkViewNode(Identifier identifier, int cell1, int cell2) : base(identifier) => (Cell1, Cell2) = (cell1, cell2);


	/// <summary>
	/// Indicates whether two cells <see cref="Cell1"/> and <see cref="Cell2"/> are adjacent by row.
	/// If <see langword="true"/>, row; otherwise, column.
	/// </summary>
	public bool IsRow => Cell2 - Cell1 == 1;

	/// <summary>
	/// Indicates the first cell used.
	/// </summary>
	public int Cell1 { get; }

	/// <summary>
	/// Indicates the second cell used. This cell should be adjacent with <see cref="Cell1"/>.
	/// </summary>
	public int Cell2 { get; }

	[GeneratedDeconstruction]
	public partial void Deconstruct(out int cell1, out int cell2);

	[GeneratedDeconstruction]
	public partial void Deconstruct(out int cell1, out int cell2, out bool isRow);
}
