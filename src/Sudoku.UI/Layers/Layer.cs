namespace Sudoku.UI.Layers;

/// <summary>
/// Defines a layer that is used for drawing a sudoku grid.
/// </summary>
public abstract class Layer : IDrawable
{
	/// <summary>
	/// Creates a <see cref="Layer"/> instance.
	/// </summary>
	public Layer()
	{
	}


	/// <summary>
	/// Indicates the offset that makes the blank outside the grid outlines.
	/// </summary>
	public int Offset { get; set; }


	/// <inheritdoc/>
	public abstract void Draw(ICanvas canvas, RectangleF dirtyRect);
}
