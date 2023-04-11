namespace Sudoku.Rendering.Nodes;

/// <summary>
/// Defines a square view node.
/// </summary>
/// <param name="identifier"><inheritdoc cref="IconViewNode(Identifier, int)" path="/param[@name='identifier']"/></param>
/// <param name="cell"><inheritdoc cref="IconViewNode(Identifier, int)" path="/param[@name='cell']"/></param>
public sealed class SquareViewNode(Identifier identifier, int cell) : IconViewNode(identifier, cell)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override SquareViewNode Clone() => new(Identifier, Cell);
}
