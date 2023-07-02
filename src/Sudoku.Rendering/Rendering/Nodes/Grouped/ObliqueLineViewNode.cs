namespace Sudoku.Rendering.Nodes.Grouped;

/// <summary>
/// Defines an oblique line view node.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class ObliqueLineViewNode(
	ColorIdentifier identifier,
	Cell firstCell,
	[PrimaryConstructorParameter(GeneratedMemberName = "TailCell"), HashCodeMember, StringMember] Cell lastCell
) : GroupedViewNode(identifier, firstCell, ImmutableArray<Cell>.Empty)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is ObliqueLineViewNode comparer && HeadCell == comparer.HeadCell && TailCell == comparer.TailCell;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override ObliqueLineViewNode Clone() => new(Identifier, HeadCell, TailCell);
}
