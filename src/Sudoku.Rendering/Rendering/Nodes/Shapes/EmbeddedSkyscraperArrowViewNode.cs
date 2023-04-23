namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Defines an embedded skyscraper arrow view node.
/// </summary>
public sealed class EmbeddedSkyscraperArrowViewNode(ColorIdentifier identifier, Cell cell, Direction directions) :
	SingleCellMarkViewNode(identifier, cell, directions)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is EmbeddedSkyscraperArrowViewNode comparer && Cell == comparer.Cell;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override EmbeddedSkyscraperArrowViewNode Clone() => new(Identifier, Cell, Directions);
}
