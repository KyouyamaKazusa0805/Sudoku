namespace Sudoku.Presentation.Nodes.Grouped;

/// <summary>
/// Defines a diagonal line pair view node. The node can only contain one in a <see cref="View"/> because it is special.
/// </summary>
/// <param name="identifier"><inheritdoc cref="GroupedViewNode(Identifier, int, ImmutableArray{int})" path="/param[@name='identifier']"/></param>
public sealed class DiagonalLinesViewNode(Identifier identifier) : GroupedViewNode(identifier, -1, ImmutableArray<int>.Empty)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override DiagonalLinesViewNode Clone() => new(Identifier);
}
