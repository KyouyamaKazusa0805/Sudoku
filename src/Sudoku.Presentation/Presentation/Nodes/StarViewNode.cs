namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a star view node.
/// </summary>
public sealed class StarViewNode : FigureViewNode
{
	/// <summary>
	/// Initializes a <see cref="StarViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cell">The cell.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public StarViewNode(Identifier identifier, int cell) : base(identifier, cell)
	{
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override StarViewNode Clone() => new(Identifier, Cell);
}
