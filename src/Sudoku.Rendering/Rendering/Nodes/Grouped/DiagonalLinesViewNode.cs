namespace Sudoku.Rendering.Nodes.Grouped;

/// <summary>
/// Defines a diagonal line pair view node. The node can only contain one in a <see cref="View"/> because it is special.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class DiagonalLinesViewNode(ColorIdentifier identifier) : GroupedViewNode(identifier, -1, ImmutableArray<Cell>.Empty)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is DiagonalLinesViewNode;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override DiagonalLinesViewNode Clone() => new(Identifier);
}
