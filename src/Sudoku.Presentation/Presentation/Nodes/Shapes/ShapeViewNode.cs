namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a shape view node.
/// </summary>
public abstract class ShapeViewNode : ViewNode
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected ShapeViewNode(Identifier identifier) : base(identifier)
	{
	}


	/// <inheritdoc/>
	protected sealed override string TypeIdentifier => GetType().Name;
}
