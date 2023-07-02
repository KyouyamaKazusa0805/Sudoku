namespace Sudoku.Rendering.Nodes.Grouped;

/// <summary>
/// Defines a pyramid view node.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class PyramidViewNode(ColorIdentifier identifier) : GroupedViewNode(identifier, -1, ImmutableArray<Cell>.Empty)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is PyramidViewNode;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override PyramidViewNode Clone() => new(Identifier);
}
