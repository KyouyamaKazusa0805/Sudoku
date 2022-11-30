namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a star view node used by star product.
/// </summary>
public sealed partial class StarProductStarViewNode : SingleCellMarkViewNode
{
	/// <summary>
	/// Initializes a <see cref="StarProductStarViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="directions">The directions.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public StarProductStarViewNode(Identifier identifier, int cell, Direction directions) : base(identifier, cell, directions)
	{
		var lengthToValidate = directions.GetAllFlags()?.Length ?? 0;

		Argument.ThrowIfFalse(lengthToValidate != 1, $"Argument '{nameof(directions)}' must hold only one flag.");
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is StarProductStarViewNode comparer
		&& Identifier == comparer.Identifier && Cell == comparer.Cell && Directions == comparer.Directions;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(Cell), nameof(Directions))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cell), nameof(Directions))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override StarProductStarViewNode Clone() => new(Identifier, Cell, Directions);
}
