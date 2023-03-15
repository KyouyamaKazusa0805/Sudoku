namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a single-cell mark view node.
/// </summary>
public abstract partial class SingleCellMarkViewNode(Identifier identifier, int cell, Direction directions) : ShapeViewNode(identifier)
{
	/// <summary>
	/// Indicates the cell used.
	/// </summary>
	public int Cell { get; } = cell is >= 0 and < 81 ? cell : throw new ArgumentOutOfRangeException(nameof(cell));

	/// <summary>
	/// Indicates the directions that the current mark points to. <see cref="Direction.None"/> is for default case.
	/// Use <see cref="Direction"/>.<see langword="operator"/> |(<see cref="Direction"/>, <see cref="Direction"/>)
	/// to combine multiple directions.
	/// </summary>
	/// <seealso cref="Direction.None"/>
	public Direction Directions { get; } = directions;

	/// <summary>
	/// Indicates the cell string.
	/// </summary>
	[DebuggerHidden]
	[GeneratedDisplayName(nameof(Cell))]
	private string CellString => CellsMap[Cell].ToString();


	[DeconstructionMethod]
	public partial void Deconstruct(out int cell, out Direction directions);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> GetType() == other?.GetType() && other is SingleCellMarkViewNode comparer
		&& Identifier == comparer.Identifier && Cell == comparer.Cell && Directions == comparer.Directions;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(Cell), nameof(Directions))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(CellString), nameof(Directions))]
	public override partial string ToString();
}
