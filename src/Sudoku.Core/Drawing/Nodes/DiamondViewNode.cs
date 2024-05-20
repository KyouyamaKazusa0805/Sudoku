namespace Sudoku.Drawing.Nodes;

/// <summary>
/// Defines a diamond view node.
/// </summary>
/// <param name="identifier"><inheritdoc cref="IconViewNode(ColorIdentifier, Cell)" path="/param[@name='identifier']"/></param>
/// <param name="cell"><inheritdoc cref="IconViewNode(ColorIdentifier, Cell)" path="/param[@name='cell']"/></param>
public sealed class DiamondViewNode(ColorIdentifier identifier, Cell cell) : IconViewNode(identifier, cell)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override DiamondViewNode Clone() => new(Identifier, Cell);
}
