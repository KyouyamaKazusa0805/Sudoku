namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Represents with a greater-than sign view node.
/// </summary>
public sealed partial class GreaterThanSignViewNode : AdjacentCellMarkViewNode
{
	/// <summary>
	/// Initializes a <see cref="GreaterThanSignViewNode"/> instance via the identifier and two adjacent cells.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cell1">The cell 1.</param>
	/// <param name="cell2">The cell 2.</param>
	/// <param name="isGreaterThan">Indicates whether the current sign is a greater-than sign.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public GreaterThanSignViewNode(Identifier identifier, int cell1, int cell2, bool isGreaterThan) : base(identifier, cell1, cell2)
		=> IsGreaterThan = isGreaterThan;


	/// <summary>
	/// Indicates whether the current sign is a greater-than sign.
	/// </summary>
	public bool IsGreaterThan { get; }

	/// <summary>
	/// Indicates the cell 1 string.
	/// </summary>
	[DebuggerHidden]
	[GeneratedDisplayName(nameof(Cell1))]
	private string Cell1String => CellsMap[Cell1].ToString();

	/// <summary>
	/// Indicates the cell 2 string.
	/// </summary>
	[DebuggerHidden]
	[GeneratedDisplayName(nameof(Cell2))]
	private string Cell2String => CellsMap[Cell2].ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is GreaterThanSignViewNode comparer
		&& Identifier == comparer.Identifier && Cell1 == comparer.Cell1 && Cell2 == comparer.Cell2 && IsGreaterThan == comparer.IsGreaterThan;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(TypeIdentifier), nameof(Cell1), nameof(Cell2), nameof(IsGreaterThan))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cell1String), nameof(Cell2String), nameof(IsGreaterThan))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override GreaterThanSignViewNode Clone() => new(Identifier, Cell1, Cell2, IsGreaterThan);
}
