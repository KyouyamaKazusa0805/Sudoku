namespace Sudoku.Drawing;

/// <summary>
/// Defines the factory methods that can append custom configurations for <see cref="ISudokuPainter"/> instances.
/// </summary>
/// <seealso cref="ISudokuPainter"/>
public interface ISudokuPainterFactory
{
	/// <summary>
	/// Sets the size of the canvas.
	/// </summary>
	/// <param name="size">The new size of the canvas.</param>
	/// <returns>The target painter.</returns>
	public abstract ISudokuPainter WithCanvasSize(int size);

	/// <summary>
	/// Sets the offset of the canvas.
	/// </summary>
	/// <param name="offset">The new offset of the canvas.</param>
	/// <returns>The target painter.</returns>
	public abstract ISudokuPainter WithCanvasOffset(int offset);

	/// <summary>
	/// Sets the grid of the canvas.
	/// </summary>
	/// <param name="grid">The new grid.</param>
	/// <returns>The target painter.</returns>
	public abstract ISudokuPainter WithGrid(scoped in Grid grid);

	/// <summary>
	/// Sets the grid of the canvas, with the string representation.
	/// </summary>
	/// <param name="gridCode">The new grid string code.</param>
	/// <returns>The target painter.</returns>
	public sealed ISudokuPainter WithGridCode(string gridCode) => WithGrid(Grid.Parse(gridCode));

	/// <summary>
	/// Sets whether the candidates in the grid will also be rendered.
	/// </summary>
	/// <param name="renderingCandidates">The <see cref="bool"/> value indicating that.</param>
	/// <returns>The target painter.</returns>
	public abstract ISudokuPainter WithRenderingCandidates(bool renderingCandidates);

	/// <summary>
	/// Sets a font name that is used for rendering text of value digits in a sudoku grid.
	/// </summary>
	/// <param name="fontName">The font name.</param>
	/// <returns>The target painter.</returns>
	public abstract ISudokuPainter WithValueFont(string fontName);

	/// <summary>
	/// Sets a font scale that is used for rendering text of digits (values and candidates) in a sudoku grid.
	/// </summary>
	/// <param name="fontScale">The font scale.</param>
	/// <returns>The target painter.</returns>
	public abstract ISudokuPainter WithFontScale(decimal fontScale);

	/// <summary>
	/// Sets a font name that is used for rendering text of candidate digits in a sudoku grid.
	/// </summary>
	/// <param name="fontName">The font name.</param>
	/// <returns>The target painter.</returns>
	public abstract ISudokuPainter WithCandidateFont(string fontName);

	/// <summary>
	/// Sets the conclusions used for rendering.
	/// </summary>
	/// <param name="conclusions">The conclusions.</param>
	/// <returns>The target painter.</returns>
	public abstract ISudokuPainter WithConclusions(params Conclusion[] conclusions);

	/// <summary>
	/// Sets the view nodes used for rendering.
	/// </summary>
	/// <param name="nodes">The nodes.</param>
	/// <returns>The target painter.</returns>
	public abstract ISudokuPainter WithNodes(params ViewNode[] nodes);
}
