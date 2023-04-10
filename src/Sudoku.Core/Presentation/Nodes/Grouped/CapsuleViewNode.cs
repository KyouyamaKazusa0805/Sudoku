namespace Sudoku.Presentation.Nodes.Grouped;

/// <summary>
/// Defines a capsule view node.
/// </summary>
public sealed partial class CapsuleViewNode(Identifier identifier, int headCell, bool isHorizontal) :
	GroupedViewNode(identifier, headCell, ImmutableArray.Create(headCell + (isHorizontal ? 1 : 9)))
{
	/// <summary>
	/// Indicates whether the view node is horizontal.
	/// </summary>
	public bool IsHorizontal { get; } = isHorizontal;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is CapsuleViewNode comparer && HeadCell == comparer.HeadCell;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(HeadCell))]
	public override partial int GetHashCode();

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(HeadCell), nameof(IsHorizontal))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CapsuleViewNode Clone() => new(Identifier, HeadCell, IsHorizontal);
}
