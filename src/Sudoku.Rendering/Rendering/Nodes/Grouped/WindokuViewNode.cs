namespace Sudoku.Rendering.Nodes.Grouped;

/// <summary>
/// Defines a windoku view node.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class WindokuViewNode(ColorIdentifier identifier) : GroupedViewNode(identifier, -1, ImmutableArray<Cell>.Empty)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is WindokuViewNode;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override WindokuViewNode Clone() => new(Identifier);
}
