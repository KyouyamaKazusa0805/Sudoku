namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines an embedded skyscraper arrow view node.
/// </summary>
public sealed class EmbeddedSkyscraperArrowViewNode(Identifier identifier, int cell, Direction directions) :
	SingleCellMarkViewNode(identifier, cell, directions)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override EmbeddedSkyscraperArrowViewNode Clone() => new(Identifier, Cell, Directions);
}
