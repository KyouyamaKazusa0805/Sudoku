namespace Sudoku.Presentation;

/// <summary>
/// Provides with a visual item.
/// </summary>
public interface IVisual
{
	/// <summary>
	/// Indicates the conclusions that the step can be eliminated or assigned to.
	/// </summary>
	public abstract ConclusionList Conclusions { get; }

	/// <summary>
	/// Indicates the views of the step that may be displayed onto the screen using pictures.
	/// </summary>
	public abstract ViewList Views { get; }
}
