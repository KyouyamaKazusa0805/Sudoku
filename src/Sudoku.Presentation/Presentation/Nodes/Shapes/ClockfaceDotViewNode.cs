namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a clockface dot view node.
/// </summary>
public sealed partial class ClockfaceDotViewNode : QuadrupleCellMarkViewNode
{
	/// <summary>
	/// Initializes a <see cref="ClockfaceDotViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="topLeftCell">The top-left cell.</param>
	/// <param name="isClockwise">Indicates whether the dot is marked as clockwise.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ClockfaceDotViewNode(Identifier identifier, int topLeftCell, bool isClockwise) : base(identifier, topLeftCell)
		=> IsClockwise = isClockwise;

	/// <summary>
	/// Initializes a <see cref="ClockfaceDotViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cells">The cells used.</param>
	/// <param name="isClockwise">Indicates whether the dot is marked as clockwise.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ClockfaceDotViewNode(Identifier identifier, scoped in CellMap cells, bool isClockwise) : base(identifier, cells)
		=> IsClockwise = isClockwise;


	/// <summary>
	/// Indicates whether the dot is marked as clockwise. If <see langword="true"/>, clockwise; otherwise, counterclockwise.
	/// </summary>
	public bool IsClockwise { get; }


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is ClockfaceDotViewNode comparer
		&& Identifier == comparer.Identifier && Cells == comparer.Cells && IsClockwise == comparer.IsClockwise;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(Cells), nameof(IsClockwise))]
	public override partial int GetHashCode();

	/// <inheritdoc/>
	public override string ToString()
		=> $$"""{{nameof(ClockfaceDotViewNode)}} { {{nameof(Cells)}} = {{Cells}}, {{nameof(IsClockwise)}} = {{IsClockwise}} }""";

	/// <inheritdoc/>
	public override ClockfaceDotViewNode Clone() => new(Identifier, Cells, IsClockwise);
}
