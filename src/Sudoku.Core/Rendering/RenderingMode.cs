namespace Sudoku.Rendering;

/// <summary>
/// Represents a kind of the rendering mode.
/// </summary>
public enum RenderingMode
{
	/// <summary>
	/// Indicates never displays the current node.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	None,

	/// <summary>
	/// Indicates the view node is only displayed in direct mode. In the direct mode, candidates will be never displayed.
	/// </summary>
	DirectModeOnly,

	/// <summary>
	/// Indicates the view node is only displayed in pencilmark mode. In the pencilmark mode, candidates will be displayed.
	/// </summary>
	PencilmarkModeOnly,

	/// <summary>
	/// Indicates the view node will be displayed in both direct and pencilmark mode.
	/// </summary>
	BothDirectAndPencilmark
}
