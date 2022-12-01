namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a circle view node.
/// </summary>
public sealed class CircleViewNode : FigureViewNode
{
	/// <summary>
	/// Initializes a <see cref="CircleViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identfier.</param>
	/// <param name="cell">The cell.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CircleViewNode(Identifier identifier, int cell) : base(identifier, cell)
	{
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CircleViewNode Clone() => new(Identifier, Cell);
}
