namespace Sudoku.Rendering.Nodes.Grouped;

/// <summary>
/// Defines a lever view node.
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
/// <param name="headCell"><inheritdoc/></param>
/// <param name="tailCell">Indicates the tail cell.</param>
/// <param name="centerCell">Indicates the center cell.</param>
[GetHashCode]
[ToString]
public sealed partial class LeverViewNode(
	ColorIdentifier identifier,
	Cell headCell,
	[PrimaryConstructorParameter, HashCodeMember, StringMember] Cell tailCell,
	[PrimaryConstructorParameter, HashCodeMember, StringMember] Cell centerCell
) : GroupedViewNode(identifier, headCell, ImmutableArray<Cell>.Empty)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is LeverViewNode comparer
		&& HeadCell == comparer.HeadCell && TailCell == comparer.TailCell && CenterCell == comparer.CenterCell;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override LeverViewNode Clone() => new(Identifier, HeadCell, TailCell, CenterCell);
}
