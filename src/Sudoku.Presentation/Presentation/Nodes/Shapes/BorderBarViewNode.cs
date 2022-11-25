namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a view node that displays as a bar on shared border line of two adjacent cells.
/// </summary>
public sealed partial class BorderBarViewNode : ViewNode
{
	/// <summary>
	/// Initializes a <see cref="BorderBarViewNode"/> instance via the identifier and two adjacent cells.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cell1">The cell 1.</param>
	/// <param name="cell2">The cell 2.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BorderBarViewNode(Identifier identifier, int cell1, int cell2) : base(identifier)
	{
		Argument.ThrowIfFalse(cell1 is >= 0 and < 81, "The argument must be between 0 and 80.");
		Argument.ThrowIfFalse(cell2 is >= 0 and < 81, "The argument must be between 0 and 80.");
		Argument.ThrowIfFalse(cell1 < cell2, $"The argument '{nameof(cell1)}' must be lower than another argument '{nameof(cell2)}'.");
		Argument.ThrowIfFalse(cell2 - cell1 is 1 or 9, $"Two cells '{nameof(cell1)}' and '{nameof(cell2)}' must be adjacent with each other.");

		(Cell1, Cell2) = (cell1, cell2);
	}


	/// <summary>
	/// Indicates whether two cells <see cref="Cell1"/> and <see cref="Cell2"/> are adjacent by row.
	/// If <see langword="true"/>, row; otherwise, column.
	/// </summary>
	public bool IsRow => Cell2 - Cell1 == 1;

	/// <summary>
	/// Indicates the first cell used.
	/// </summary>
	public int Cell1 { get; }

	/// <summary>
	/// Indicates the second cell used.
	/// </summary>
	public int Cell2 { get; }

	/// <inheritdoc/>
	protected override string TypeIdentifier => nameof(BorderBarViewNode);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is BorderBarViewNode comparer && Identifier == comparer.Identifier && Cell1 == comparer.Cell1 && Cell2 == comparer.Cell2;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(Cell1), nameof(Cell2))]
	public override partial int GetHashCode();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
		=> $$"""{{nameof(BorderBarViewNode)}} { {{nameof(Identifier)}} = {{Identifier}}, {{nameof(Cell1)}} = {{CellsMap[Cell1]}}, {{nameof(Cell2)}} = {{CellsMap[Cell2]}} }""";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override BorderBarViewNode Clone() => new(Identifier, Cell1, Cell2);
}
