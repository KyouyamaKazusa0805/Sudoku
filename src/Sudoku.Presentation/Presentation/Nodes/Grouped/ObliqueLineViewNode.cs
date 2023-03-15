namespace Sudoku.Presentation.Nodes.Grouped;

/// <summary>
/// Defines an oblique line view node.
/// </summary>
/// <param name="identifier"><inheritdoc cref="GroupedViewNode(Identifier, int, ImmutableArray{int})" path="/param[@name='identifier']"/></param>
/// <param name="firstCell">The first cell.</param>
/// <param name="lastCell">The last cell.</param>
public sealed partial class ObliqueLineViewNode(
	Identifier identifier,
	int firstCell,
	int lastCell
) : GroupedViewNode(identifier, firstCell, ImmutableArray<int>.Empty)
{
	/// <summary>
	/// Indicates the last cell.
	/// </summary>
	public int TailCell { get; } = lastCell;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is ObliqueLineViewNode comparer
		&& Identifier == comparer.Identifier && HeadCell == comparer.HeadCell && TailCell == comparer.TailCell;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(HeadCell), nameof(TailCell))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(HeadCell), nameof(TailCell))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override ObliqueLineViewNode Clone() => new(Identifier, HeadCell, TailCell);
}
