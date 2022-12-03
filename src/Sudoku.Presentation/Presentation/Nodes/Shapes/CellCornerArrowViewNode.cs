namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a cell corner arrow view node.
/// </summary>
public sealed partial class CellCornerArrowViewNode : SingleCellMarkViewNode
{
	/// <summary>
	/// Initializes a <see cref="CellCornerArrowViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="directions">The directions.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CellCornerArrowViewNode(Identifier identifier, int cell, Direction directions) : base(identifier, cell, directions)
	{
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is CellCornerArrowViewNode comparer
		&& Identifier == comparer.Identifier && Cell == comparer.Cell && Directions == comparer.Directions;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(Cell), nameof(Directions))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cell), nameof(Directions))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CellCornerArrowViewNode Clone() => new(Identifier, Cell, Directions);
}
