namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines an embedded skyscraper arrow view node.
/// </summary>
public sealed class EmbeddedSkyscraperArrowViewNode : SingleCellMarkViewNode
{
	/// <summary>
	/// Initializes an <see cref="EmbeddedSkyscraperArrowViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="directions">The directions.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public EmbeddedSkyscraperArrowViewNode(Identifier identifier, int cell, Direction directions) : base(identifier, cell, directions)
	{
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override EmbeddedSkyscraperArrowViewNode Clone() => new(Identifier, Cell, Directions);
}
