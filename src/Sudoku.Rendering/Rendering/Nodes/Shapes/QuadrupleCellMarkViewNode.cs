namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Defines a quadruple cell mark view node that lies on a 2x2 squared cells.
/// </summary>
public abstract class QuadrupleCellMarkViewNode(ColorIdentifier identifier, scoped in CellMap cells) : ShapeViewNode(identifier)
{
	/// <summary>
	/// Assigns properties with target values.
	/// </summary>
	/// <param name="identifier"><inheritdoc cref="ShapeViewNode(ColorIdentifier)" path="/param[@name='identifier']"/></param>
	/// <param name="topLeftCell">The top-left cell.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected QuadrupleCellMarkViewNode(ColorIdentifier identifier, Cell topLeftCell) :
		this(identifier, CellsMap[topLeftCell] + (topLeftCell + 1) + (topLeftCell + 9) + (topLeftCell + 10))
	{
	}


	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	[HashCodeMember, StringMember]
	public CellMap Cells { get; } =
		cells.Count == 4 ? cells : throw new ArgumentException($"The argument '{nameof(cells)}' must hold 4 cells.", nameof(cells));
}
