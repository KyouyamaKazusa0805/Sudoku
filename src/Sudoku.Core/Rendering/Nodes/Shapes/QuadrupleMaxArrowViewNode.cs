namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Defines a quadruple max arrow view node.
/// </summary>
public sealed partial class QuadrupleMaxArrowViewNode(Identifier identifier, scoped in CellMap cells, Direction arrowDirection) :
	QuadrupleCellMarkViewNode(identifier, cells)
{
	/// <summary>
	/// Initializes a <see cref="QuadrupleMaxArrowViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="topLeftCell">The top-left cell.</param>
	/// <param name="arrowDirection">The arrow direction.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public QuadrupleMaxArrowViewNode(Identifier identifier, int topLeftCell, Direction arrowDirection) :
		this(identifier, CellsMap[topLeftCell] + (topLeftCell + 1) + (topLeftCell + 9) + (topLeftCell + 10), arrowDirection)
	{
	}


	/// <summary>
	/// Indicates the arrow direction.
	/// </summary>
	public Direction ArrowDirection { get; } =
		arrowDirection is Direction.TopLeft or Direction.TopRight or Direction.BottomLeft or Direction.BottomRight
			? arrowDirection
			: throw new ArgumentException($"Argument '{nameof(arrowDirection)} can only be one direction.'", nameof(arrowDirection));


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is QuadrupleMaxArrowViewNode comparer && Cells == comparer.Cells;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(Cells))]
	public override partial int GetHashCode();

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cells), nameof(ArrowDirection))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override QuadrupleMaxArrowViewNode Clone() => new(Identifier, Cells, ArrowDirection);
}
