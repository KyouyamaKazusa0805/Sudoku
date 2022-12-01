namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a triangle view node.
/// </summary>
public sealed class TriangleViewNode : FigureViewNode
{
	/// <summary>
	/// Initializes a <see cref="TriangleViewNode"/> instance via the specified identfier value.
	/// </summary>
	/// <param name="identifier">The identfier.</param>
	/// <param name="cell">The cell.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TriangleViewNode(Identifier identifier, int cell) : base(identifier, cell)
	{
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override TriangleViewNode Clone() => new(Identifier, Cell);
}
