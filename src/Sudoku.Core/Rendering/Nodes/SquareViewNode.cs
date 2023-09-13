using System.Runtime.CompilerServices;

namespace Sudoku.Rendering.Nodes;

/// <summary>
/// Defines a square view node.
/// </summary>
/// <param name="identifier"><inheritdoc cref="IconViewNode(ColorIdentifier, Cell)" path="/param[@name='identifier']"/></param>
/// <param name="cell"><inheritdoc cref="IconViewNode(ColorIdentifier, Cell)" path="/param[@name='cell']"/></param>
public sealed class SquareViewNode(ColorIdentifier identifier, Cell cell) : IconViewNode(identifier, cell)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override SquareViewNode Clone() => new(Identifier, Cell);
}
