namespace Sudoku.Presentation.Nodes.Grouped;

/// <summary>
/// Defines a diagonal line pair view node. The node can only contain one in a <see cref="View"/> because it is special.
/// </summary>
public sealed class DiagonalLinesViewNode : GroupedViewNode
{
	/// <summary>
	/// Initializes a <see cref="GroupedViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DiagonalLinesViewNode(Identifier identifier) : base(identifier, -1, ImmutableArray<int>.Empty)
	{
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override DiagonalLinesViewNode Clone() => new(Identifier);
}
