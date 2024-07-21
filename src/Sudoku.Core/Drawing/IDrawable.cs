namespace Sudoku.Drawing;

/// <summary>
/// Represents a drawable instance that can be used for drawing, providing with base data structure to be used by drawing APIs.
/// </summary>
public interface IDrawable
{
	/// <summary>
	/// Indicates the conclusions that the step can be eliminated or assigned to.
	/// </summary>
	public abstract ReadOnlyMemory<Conclusion> Conclusions { get; }

	/// <summary>
	/// Indicates the views of the step that may be displayed onto the screen using pictures.
	/// </summary>
	public abstract ReadOnlyMemory<View> Views { get; }
}
