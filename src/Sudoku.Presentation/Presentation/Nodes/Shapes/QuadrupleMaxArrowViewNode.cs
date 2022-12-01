namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a quadruple max arrow view node.
/// </summary>
public sealed partial class QuadrupleMaxArrowViewNode : QuadrupleCellMarkViewNode
{
	/// <summary>
	/// Initializes a <see cref="QuadrupleMaxArrowViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="topLeftCell">The top-left cell.</param>
	/// <param name="arrowDirection">The arrow direction.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public QuadrupleMaxArrowViewNode(Identifier identifier, int topLeftCell, Direction arrowDirection) : base(identifier, topLeftCell)
	{
		Argument.ThrowIfFalse(
			arrowDirection is Direction.TopLeft or Direction.TopRight or Direction.BottomLeft or Direction.BottomRight,
			$"Argument '{nameof(arrowDirection)} can only be one direction.'"
		);

		ArrowDirection = arrowDirection;
	}

	/// <summary>
	/// Initializes a <see cref="QuadrupleMaxArrowViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cells">The cells used.</param>
	/// <param name="arrowDirection">The arrow direction.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public QuadrupleMaxArrowViewNode(Identifier identifier, scoped in CellMap cells, Direction arrowDirection) : base(identifier, cells)
	{
		Argument.ThrowIfFalse(
			arrowDirection is Direction.TopLeft or Direction.TopRight or Direction.BottomLeft or Direction.BottomRight,
			$"Argument '{nameof(arrowDirection)} can only be one direction.'"
		);

		ArrowDirection = arrowDirection;
	}


	/// <summary>
	/// Indicates the arrow direction.
	/// </summary>
	public Direction ArrowDirection { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is QuadrupleMaxArrowViewNode comparer
		&& Identifier == comparer.Identifier && Cells == comparer.Cells && ArrowDirection == comparer.ArrowDirection;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(Cells), nameof(ArrowDirection))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cells), nameof(ArrowDirection))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override QuadrupleMaxArrowViewNode Clone() => new(Identifier, Cells, ArrowDirection);
}
