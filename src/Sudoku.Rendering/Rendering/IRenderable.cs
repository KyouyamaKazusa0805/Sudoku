namespace Sudoku.Rendering;

/// <summary>
/// Represents a renderable instance that can be used for rendering, providing with base data structure to be used by drawing APIs.
/// </summary>
public interface IRenderable
{
	/// <summary>
	/// Indicates the conclusions that the step can be eliminated or assigned to.
	/// </summary>
	public abstract Conclusion[] Conclusions { get; }

	/// <summary>
	/// Indicates the views of the step that may be displayed onto the screen using pictures.
	/// </summary>
	public abstract View[]? Views { get; }
}
