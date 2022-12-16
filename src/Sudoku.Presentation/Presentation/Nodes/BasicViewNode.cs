namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a basic view node type that provides with basic displaying elements from a grid.
/// </summary>
public abstract class BasicViewNode : ViewNode
{
	/// <summary>
	/// Assigns the property <see cref="ViewNode.Identifier"/> with the specified value.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected BasicViewNode(Identifier identifier) : base(identifier)
	{
	}


	/// <inheritdoc/>
	protected sealed override string TypeIdentifier => GetType().Name;
}
