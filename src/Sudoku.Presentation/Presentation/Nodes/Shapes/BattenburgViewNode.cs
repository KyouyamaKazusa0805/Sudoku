namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a battenburg view node.
/// </summary>
public sealed partial class BattenburgViewNode(Identifier identifier, scoped in CellMap cells) : QuadrupleCellMarkViewNode(identifier, cells)
{
	/// <summary>
	/// Initializes a <see cref="BattenburgViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="topLeftCell">The top-left cell used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BattenburgViewNode(Identifier identifier, int topLeftCell) :
		this(identifier, CellsMap[topLeftCell] + (topLeftCell + 1) + (topLeftCell + 9) + (topLeftCell + 10))
	{
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is BattenburgViewNode comparer && Identifier == comparer.Identifier && Cells == comparer.Cells;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(Cells))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cells))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override BattenburgViewNode Clone() => new(Identifier, Cells);
}
