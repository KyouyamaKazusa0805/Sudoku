namespace Sudoku.Rendering.Nodes.Grouped;

/// <summary>
/// Defines a diagonal line pair view node. The node can only contain one in a <see cref="View"/> because it is special.
/// </summary>
public sealed partial class DiagonalLinesViewNode(Identifier identifier) : GroupedViewNode(identifier, -1, ImmutableArray<int>.Empty)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is DiagonalLinesViewNode;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier))]
	public override partial int GetHashCode();

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override DiagonalLinesViewNode Clone() => new(Identifier);
}
