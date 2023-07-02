namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Defines a battenburg view node.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class BattenburgViewNode(ColorIdentifier identifier, scoped in CellMap cells) : QuadrupleCellMarkViewNode(identifier, cells)
{
	/// <summary>
	/// Initializes a <see cref="BattenburgViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="topLeftCell">The top-left cell used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BattenburgViewNode(ColorIdentifier identifier, Cell topLeftCell) :
		this(identifier, CellsMap[topLeftCell] + (topLeftCell + 1) + (topLeftCell + 9) + (topLeftCell + 10))
	{
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is BattenburgViewNode comparer && Cells[0] == comparer.Cells[0];

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override BattenburgViewNode Clone() => new(Identifier, Cells);
}
