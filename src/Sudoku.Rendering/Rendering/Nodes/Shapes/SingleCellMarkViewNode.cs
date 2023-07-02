namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Defines a single-cell mark view node.
/// </summary>
[GetHashCode]
[ToString]
public abstract partial class SingleCellMarkViewNode(ColorIdentifier identifier, Cell cell, Direction directions) : ShapeViewNode(identifier)
{
	/// <summary>
	/// Indicates the cell used.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public Cell Cell { get; } = cell is >= 0 and < 81 ? cell : throw new ArgumentOutOfRangeException(nameof(cell));

	/// <summary>
	/// Indicates the directions that the current mark points to. <see cref="Direction.None"/> is for default case.
	/// Use <see cref="Direction"/>.<see langword="operator"/> |(<see cref="Direction"/>, <see cref="Direction"/>)
	/// to combine multiple directions.
	/// </summary>
	/// <seealso cref="Direction.None"/>
	[StringMember]
	public Direction Directions { get; } = directions;

	/// <summary>
	/// Indicates the cell string.
	/// </summary>
	[StringMember(nameof(Cell))]
	protected string CellString => CellsMap[Cell].ToString();


	[DeconstructionMethod]
	public partial void Deconstruct(out Cell cell, out Direction directions);
}
