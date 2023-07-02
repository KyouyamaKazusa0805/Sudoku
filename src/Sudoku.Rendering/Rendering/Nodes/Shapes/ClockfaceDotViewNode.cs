namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Defines a clockface dot view node.
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="isClockwise">
/// Indicates whether the dot is marked as clockwise. If <see langword="true"/>, clockwise; otherwise, counterclockwise.
/// </param>
[GetHashCode]
[ToString]
public sealed partial class ClockfaceDotViewNode(
	ColorIdentifier identifier,
	scoped in CellMap cells,
	[PrimaryConstructorParameter, StringMember] bool isClockwise
) : QuadrupleCellMarkViewNode(identifier, cells)
{
	/// <summary>
	/// Initializes a <see cref="ClockfaceDotViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="topLeftCell">The top-left cell.</param>
	/// <param name="isClockwise">Indicates whether the dot is marked as clockwise.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ClockfaceDotViewNode(ColorIdentifier identifier, Cell topLeftCell, bool isClockwise) :
		this(identifier, CellsMap[topLeftCell] + (topLeftCell + 1) + (topLeftCell + 9) + (topLeftCell + 10), isClockwise)
	{
	}


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is ClockfaceDotViewNode comparer && Cells == comparer.Cells;

	/// <inheritdoc/>
	public override ClockfaceDotViewNode Clone() => new(Identifier, Cells, IsClockwise);
}
