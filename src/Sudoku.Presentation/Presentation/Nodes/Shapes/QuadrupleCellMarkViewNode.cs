namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a quadruple cell mark view node that lies on a 2x2 squared cells.
/// </summary>
public abstract class QuadrupleCellMarkViewNode : ShapeViewNode
{
	/// <summary>
	/// Assigns properties with target values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cells">The cells.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected QuadrupleCellMarkViewNode(Identifier identifier, scoped in CellMap cells) : base(identifier) => Cells = cells;


	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	public CellMap Cells { get; }
}
