namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a single-cell mark view node.
/// </summary>
public abstract partial class SingleCellMarkViewNode : ShapeViewNode
{
	/// <summary>
	/// Assigns the values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cell">The cell used.</param>
	/// <param name="directions">The directions used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected SingleCellMarkViewNode(Identifier identifier, int cell, Direction directions) : base(identifier)
	{
		Argument.ThrowIfFalse(cell is >= 0 and < 81, $"The argument must be between 0 and 80.");

		(Cell, Directions) = (cell, directions);
	}


	/// <summary>
	/// Indicates the cell used.
	/// </summary>
	public int Cell { get; }

	/// <summary>
	/// Indicates the directions that the current mark points to. <see cref="Direction.None"/> is for default case.
	/// Use <see cref="Direction"/>.<see langword="operator"/> |(<see cref="Direction"/>, <see cref="Direction"/>)
	/// to combine multiple directions.
	/// </summary>
	/// <seealso cref="Direction.None"/>
	public Direction Directions { get; }


	[GeneratedDeconstruction]
	public partial void Deconstruct(out int cell, out Direction directions);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> GetType() == other?.GetType()
		&& other is SingleCellMarkViewNode comparer && Cell == comparer.Cell && Directions == comparer.Directions;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(Cell), nameof(Directions))]
	public override partial int GetHashCode();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
		=> $$"""{{GetType().Name}} { {{nameof(Cell)}} = {{CellsMap[Cell]}}, {{nameof(Directions)}} = {{Directions}} }""";
}
