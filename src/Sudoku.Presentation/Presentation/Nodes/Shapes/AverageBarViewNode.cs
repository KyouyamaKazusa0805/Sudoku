namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines an average bar view node.
/// </summary>
public sealed partial class AverageBarViewNode(
	Identifier identifier,
	int cell,
	AdjacentCellType type
) : SingleCellMarkViewNode(identifier, cell, Direction.None)
{
	/// <summary>
	/// Indicates the adjacent cell type.
	/// </summary>
	public AdjacentCellType Type { get; } = type;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is AverageBarViewNode comparer
		&& Identifier == comparer.Identifier && Cell == comparer.Cell && Type == comparer.Type;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(Cell), nameof(Type))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Cell), nameof(Type))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override AverageBarViewNode Clone() => new(Identifier, Cell, Type);
}
