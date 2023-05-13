namespace Sudoku.Rendering.Nodes;

/// <summary>
/// Defines a basic view node type that provides with basic displaying elements from a grid.
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
public abstract class BasicViewNode(ColorIdentifier identifier) : ViewNode(identifier)
{
	/// <summary>
	/// Indicates the mode that the bound view node will be displayed.
	/// The default value is <see cref="RenderingMode.PencilmarkModeOnly"/>, which means only pencilmark mode the node will be displayed.
	/// </summary>
	public RenderingMode RenderingMode { get; init; } = RenderingMode.PencilmarkModeOnly;
}
