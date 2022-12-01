namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a diamond view node.
/// </summary>
public sealed class DiamondViewNode : FigureViewNode
{
	/// <summary>
	/// Initializes a <see cref="DiamondViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identfier.</param>
	/// <param name="cell">The cell.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DiamondViewNode(Identifier identifier, int cell) : base(identifier, cell)
	{
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override DiamondViewNode Clone() => new(Identifier, Cell);
}
