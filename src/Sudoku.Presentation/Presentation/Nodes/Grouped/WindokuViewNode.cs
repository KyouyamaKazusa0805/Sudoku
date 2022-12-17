namespace Sudoku.Presentation.Nodes.Grouped;

/// <summary>
/// Defines a windoku view node.
/// </summary>
public sealed partial class WindokuViewNode : GroupedViewNode
{
	/// <summary>
	/// Initializes a <see cref="WindokuViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public WindokuViewNode(Identifier identifier) : base(identifier, -1, ImmutableArray<int>.Empty)
	{
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is WindokuViewNode comparer && Identifier == comparer.Identifier;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override WindokuViewNode Clone() => new(Identifier);
}
