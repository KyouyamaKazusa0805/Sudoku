namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a heart view node.
/// </summary>
public sealed class HeartViewNode : FigureViewNode
{
	/// <summary>
	/// Initializes a <see cref="HeartViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cell">The cell.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public HeartViewNode(Identifier identifier, int cell) : base(identifier, cell)
	{
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override HeartViewNode Clone() => new(Identifier, Cell);
}
