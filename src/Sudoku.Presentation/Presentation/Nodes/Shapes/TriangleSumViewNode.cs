namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a triangle-sum view node.
/// </summary>
public sealed partial class TriangleSumViewNode : SingleCellMarkViewNode
{
	/// <summary>
	/// Initializes a <see cref="TriangleSumViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="directions">The directions.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TriangleSumViewNode(Identifier identifier, int cell, Direction directions) : base(identifier, cell, directions)
	{
		Argument.ThrowIfFalse(directions.GetAllFlags()?.Any(directionValidator) ?? true, "The direction value is invalid.");


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool directionValidator(Direction e)
			=> e is not (Direction.TopCenter or Direction.BottomCenter or Direction.MiddleLeft or Direction.MiddleRight);
	}


	/// <summary>
	/// Determines whether the shape is full complement.
	/// </summary>
	public bool IsComplement => Directions is (Direction.TopLeft | Direction.BottomRight) or (Direction.TopRight | Direction.BottomLeft);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is TriangleSumViewNode comparer
		&& Identifier == comparer.Identifier && Cell == comparer.Cell && Directions == comparer.Directions;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(Cell), nameof(Directions))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cell), nameof(Directions))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override TriangleSumViewNode Clone() => new(Identifier, Cell, Directions);
}
