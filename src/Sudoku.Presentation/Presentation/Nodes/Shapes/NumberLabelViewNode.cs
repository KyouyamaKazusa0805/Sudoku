namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a number label view node.
/// </summary>
public sealed partial class NumberLabelViewNode : AdjacentCellMarkViewNode
{
	/// <summary>
	/// Initializes a <see cref="NumberLabelViewNode"/> instance.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cell1">The cell 1.</param>
	/// <param name="cell2">The cell 2.</param>
	/// <param name="label">Indicates the label you want to display.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public NumberLabelViewNode(Identifier identifier, int cell1, int cell2, string label) : base(identifier, cell1, cell2) => Label = label;


	/// <summary>
	/// Indicates the target value you want to display.
	/// </summary>
	/// <remarks><i>
	/// Please note that the target value is a <see cref="string"/> value. We don't limit you only input numbers <b>on purpose</b>.
	/// You can use instances of the current type to display what you want to display.
	/// </i></remarks>
	public string Label { get; }


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is NumberLabelViewNode comparer
		&& Identifier == comparer.Identifier
		&& Cell1 == comparer.Cell1 && Cell2 == comparer.Cell2 && Label == comparer.Label;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(Cell1), nameof(Cell2), nameof(Label))]
	public override partial int GetHashCode();

	/// <inheritdoc/>
	public override string ToString()
		=> $$"""{{nameof(NumberLabelViewNode)}} { {{nameof(Cell1)}} = {{CellsMap[Cell1]}}, {{nameof(Cell2)}} = {{CellsMap[Cell2]}}, {{nameof(Label)}} = "{{Label}}" }""";

	/// <inheritdoc/>
	public override NumberLabelViewNode Clone() => new(Identifier, Cell1, Cell2, Label);
}
