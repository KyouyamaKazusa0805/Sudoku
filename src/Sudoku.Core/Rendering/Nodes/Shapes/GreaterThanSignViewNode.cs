namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Represents with a greater-than sign view node.
/// </summary>
public sealed partial class GreaterThanSignViewNode(Identifier identifier, int cell1, int cell2, bool isGreaterThan) :
	AdjacentCellMarkViewNode(identifier, cell1, cell2)
{
	/// <summary>
	/// Indicates whether the current sign is a greater-than sign.
	/// </summary>
	public bool IsGreaterThan { get; } = isGreaterThan;

	/// <summary>
	/// Indicates the cell 1 string.
	/// </summary>
	[GeneratedDisplayName(nameof(Cell1))]
	private string Cell1String => CellsMap[Cell1].ToString();

	/// <summary>
	/// Indicates the cell 2 string.
	/// </summary>
	[GeneratedDisplayName(nameof(Cell2))]
	private string Cell2String => CellsMap[Cell2].ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is GreaterThanSignViewNode comparer && Cell1 == comparer.Cell1 && Cell2 == comparer.Cell2;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(Cell1), nameof(Cell2))]
	public override partial int GetHashCode();

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cell1String), nameof(Cell2String), nameof(IsGreaterThan))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override GreaterThanSignViewNode Clone() => new(Identifier, Cell1, Cell2, IsGreaterThan);
}
