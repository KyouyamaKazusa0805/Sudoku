namespace Sudoku.Rendering.Nodes.Grouped;

/// <summary>
/// Defines an oblique line view node.
/// </summary>
public sealed partial class ObliqueLineViewNode(ColorIdentifier identifier, int firstCell, int lastCell) :
	GroupedViewNode(identifier, firstCell, ImmutableArray<int>.Empty)
{
	/// <summary>
	/// Indicates the last cell.
	/// </summary>
	public int TailCell { get; } = lastCell;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is ObliqueLineViewNode comparer && HeadCell == comparer.HeadCell && TailCell == comparer.TailCell;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(HeadCell), nameof(TailCell))]
	public override partial int GetHashCode();

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(HeadCell), nameof(TailCell))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override ObliqueLineViewNode Clone() => new(Identifier, HeadCell, TailCell);
}
