namespace Sudoku.Rendering.Nodes.Grouped;

/// <summary>
/// Defines a capsule view node.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class CapsuleViewNode(
	ColorIdentifier identifier,
	Cell headCell,
	[PrimaryConstructorParameter, StringMember] bool isHorizontal
) : GroupedViewNode(identifier, headCell, ImmutableArray.Create(headCell + (isHorizontal ? 1 : 9)))
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is CapsuleViewNode comparer && HeadCell == comparer.HeadCell;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CapsuleViewNode Clone() => new(Identifier, HeadCell, IsHorizontal);
}
