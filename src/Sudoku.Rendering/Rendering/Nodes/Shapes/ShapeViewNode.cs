namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Defines a shape view node.
/// </summary>
public abstract class ShapeViewNode(ColorIdentifier identifier) : ViewNode(identifier)
{
	/// <summary>
	/// Indicates the mode that the bound view node will be displayed.
	/// The default value is <see cref="RenderingMode.PencilmarkModeOnly"/>, which means only pencilmark mode the node will be displayed.
	/// </summary>
	public RenderingMode RenderingMode { get; init; } = RenderingMode.PencilmarkModeOnly;
}
