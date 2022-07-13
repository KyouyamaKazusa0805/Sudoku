namespace Sudoku.Presentation;

/// <summary>
/// Provides with a displayable item.
/// </summary>
public interface IDisplayable
{
	/// <summary>
	/// Indicates the conclusions that the step can be eliminated or assigned to.
	/// </summary>
	public abstract ImmutableArray<Conclusion> Conclusions { get; }

	/// <summary>
	/// Indicates the views of the step that may be displayed onto the screen using pictures.
	/// </summary>
	public abstract ImmutableArray<View> Views { get; }
}
