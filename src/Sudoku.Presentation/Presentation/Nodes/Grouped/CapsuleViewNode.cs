namespace Sudoku.Presentation.Nodes.Grouped;

/// <summary>
/// Defines a capsule view node.
/// </summary>
/// <param name="identifier"><inheritdoc cref="GroupedViewNode(Identifier, int, ImmutableArray{int})" path="/param[@name='identifier']"/></param>
/// <param name="headCell"><inheritdoc cref="GroupedViewNode(Identifier, int, ImmutableArray{int})" path="/param[@name='headCell']"/></param>
/// <param name="adjacentType">The adjacent cells type.</param>
public sealed partial class CapsuleViewNode(
	Identifier identifier,
	int headCell,
	AdjacentCellType adjacentType
) : GroupedViewNode(
	identifier,
	headCell,
	adjacentType switch
	{
		AdjacentCellType.Rowish => ImmutableArray.Create(headCell + 1),
		AdjacentCellType.Columnish => ImmutableArray.Create(headCell + 9),
		_ => throw new NotSupportedException("Other adjacent cell types are not supported.")
	}
)
{
	/// <summary>
	/// Indicates the adjacent cell type.
	/// </summary>
	public AdjacentCellType AdjacentType { get; } = adjacentType;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is CapsuleViewNode comparer
		&& Identifier == comparer.Identifier && HeadCell == comparer.HeadCell && AdjacentType == comparer.AdjacentType;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(HeadCell), nameof(AdjacentType))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(HeadCell), nameof(AdjacentType))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CapsuleViewNode Clone() => new(Identifier, HeadCell, AdjacentType);
}
