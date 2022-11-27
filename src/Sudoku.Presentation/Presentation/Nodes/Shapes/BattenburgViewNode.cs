namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a battenburg view node.
/// </summary>
public sealed partial class BattenburgViewNode : QuadrupleCellMarkViewNode
{
	/// <summary>
	/// Initializes a <see cref="BattenburgViewNode"/> instance via the values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="topLeftCell">The top-left cell used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BattenburgViewNode(Identifier identifier, int topLeftCell) : base(identifier, topLeftCell)
	{
	}

	/// <summary>
	/// Initializes a <see cref="BattenburgViewNode"/> instance via the values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cells">The cells used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BattenburgViewNode(Identifier identifier, scoped in CellMap cells) : base(identifier, cells)
	{
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is BattenburgViewNode comparer && Identifier == comparer.Identifier && Cells == comparer.Cells;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(Cells))]
	public override partial int GetHashCode();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => $$"""{{nameof(BattenburgViewNode)}} { {{nameof(Cells)}} = {{Cells}} }""";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override BattenburgViewNode Clone() => new(Identifier, Cells);
}
