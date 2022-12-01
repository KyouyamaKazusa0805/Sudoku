namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a square view node.
/// </summary>
public sealed class SquareViewNode : FigureViewNode
{
	/// <summary>
	/// Initializes a <see cref="SquareViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identfier.</param>
	/// <param name="cell">The cell.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SquareViewNode(Identifier identifier, int cell) : base(identifier, cell)
	{
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override SquareViewNode Clone() => new(Identifier, Cell);
}
