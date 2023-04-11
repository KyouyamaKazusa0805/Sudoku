namespace Sudoku.Rendering.Nodes;

/// <summary>
/// Defines a triangle view node.
/// </summary>
/// <param name="identifier"><inheritdoc cref="IconViewNode(Identifier, int)" path="/param[@name='identifier']"/></param>
/// <param name="cell"><inheritdoc cref="IconViewNode(Identifier, int)" path="/param[@name='cell']"/></param>
public sealed class TriangleViewNode(Identifier identifier, int cell) : IconViewNode(identifier, cell)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override TriangleViewNode Clone() => new(Identifier, Cell);
}
