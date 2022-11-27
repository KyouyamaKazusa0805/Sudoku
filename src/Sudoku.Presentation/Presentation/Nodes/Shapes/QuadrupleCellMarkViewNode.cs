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
	/// <param name="topLeftCell">The top-left cell.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected QuadrupleCellMarkViewNode(Identifier identifier, int topLeftCell) :
		this(identifier, CellsMap[topLeftCell] + (topLeftCell + 1) + (topLeftCell + 9) + (topLeftCell + 10))
	{
	}

	/// <summary>
	/// Assigns properties with target values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cells">The cells.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected QuadrupleCellMarkViewNode(Identifier identifier, scoped in CellMap cells) : base(identifier)
	{
		Argument.ThrowIfFalse(cells.Count == 4, $"The argument '{nameof(cells)}' must hold 4 cells.");

		Cells = cells;
	}


	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	public CellMap Cells { get; }
}
