namespace Sudoku.Rendering.Nodes.Grouped;

/// <summary>
/// Defines a windoku view node.
/// </summary>
public sealed partial class WindokuViewNode(Identifier identifier) : GroupedViewNode(identifier, -1, ImmutableArray<int>.Empty)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is WindokuViewNode;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier))]
	public override partial int GetHashCode();

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override WindokuViewNode Clone() => new(Identifier);
}
