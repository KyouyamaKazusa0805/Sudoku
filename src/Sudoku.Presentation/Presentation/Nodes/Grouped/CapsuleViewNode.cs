namespace Sudoku.Presentation.Nodes.Grouped;

/// <summary>
/// Defines a capsule view node.
/// </summary>
public sealed partial class CapsuleViewNode(Identifier identifier, int headCell, AdjacentCellType adjacentType) :
	GroupedViewNode(
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
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is CapsuleViewNode comparer && HeadCell == comparer.HeadCell;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(HeadCell))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(HeadCell), nameof(AdjacentType))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CapsuleViewNode Clone() => new(Identifier, HeadCell, AdjacentType);
}
