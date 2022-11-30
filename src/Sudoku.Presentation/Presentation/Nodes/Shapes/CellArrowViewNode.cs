namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a cell arrow view node.
/// </summary>
public sealed partial class CellArrowViewNode : SingleCellMarkViewNode
{
	/// <summary>
	/// Initializes a <see cref="CellArrowViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="directions">The directions.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CellArrowViewNode(int cell, Direction directions) : base(DisplayColorKind.Normal, cell, directions)
		=> Argument.ThrowIfFalse(IsPow2((byte)directions), $"Argument '{nameof(directions)}' must hold only one flag.");


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is CellArrowViewNode comparer
		&& Identifier == comparer.Identifier && Cell == comparer.Cell && Directions == comparer.Directions;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(Cell), nameof(Directions))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cell), nameof(Directions))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CellArrowViewNode Clone() => new(Cell, Directions);
}
