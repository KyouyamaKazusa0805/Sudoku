namespace Sudoku.Presentation.Nodes.Grouped;

/// <summary>
/// Defines a pyramid view node.
/// </summary>
/// <param name="identifier"><inheritdoc cref="GroupedViewNode(Identifier, int, ImmutableArray{int})" path="/param[@name='identifier']"/></param>
public sealed partial class PyramidViewNode(Identifier identifier) : GroupedViewNode(identifier, -1, ImmutableArray<int>.Empty)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is PyramidViewNode comparer && Identifier == comparer.Identifier;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override PyramidViewNode Clone() => new(Identifier);
}
