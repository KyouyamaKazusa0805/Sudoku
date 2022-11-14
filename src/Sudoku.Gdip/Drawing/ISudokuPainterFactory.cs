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
	ISudokuPainter WithCanvasSize(int size);

	/// <summary>
	/// Sets the offset of the canvas.
	/// </summary>
	/// <param name="offset">The new offset of the canvas.</param>
	/// <returns>The target painter.</returns>
	ISudokuPainter WithCanvasOffset(int offset);

	/// <summary>
	/// Sets the grid of the canvas.
	/// </summary>
	/// <param name="grid">The new grid.</param>
	/// <returns>The target painter.</returns>
	ISudokuPainter WithGrid(scoped in Grid grid);

	/// <summary>
	/// Sets the grid of the canvas, with the string representation.
	/// </summary>
	/// <param name="gridCode">The new grid string code.</param>
	/// <returns>The target painter.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	ISudokuPainter WithGridCode(string gridCode) => WithGrid(Grid.Parse(gridCode));

	/// <summary>
	/// Sets whether the candidates in the grid will also be rendered.
	/// </summary>
	/// <param name="renderingCandidates">The <see cref="bool"/> value indicating that.</param>
	/// <returns>The target painter.</returns>
	ISudokuPainter WithRenderingCandidates(bool renderingCandidates);

	/// <summary>
	/// Sets a font name that is used for rendering text of value digits in a sudoku grid.
	/// </summary>
	/// <param name="fontName">The font name.</param>
	/// <returns>The target painter.</returns>
	ISudokuPainter WithValueFont(string fontName);

	/// <summary>
	/// Sets a font scale that is used for rendering text of digits (values and candidates) in a sudoku grid.
	/// </summary>
	/// <param name="fontScale">
	/// <para>Indicates the desired font scale.</para>
	/// <para>
	/// The value is surrounded with 1. If you want to make the rendered digits become greater, you can set the value greater;
	/// otherwise, just set the value less. Generally the value should be less than 1.
	/// However, you can also assign a value greater than 1.
	/// </para>
	/// <para>We recommend you assign the value with the range (0, 1], with the boundary value 1, but not containing 0.</para>
	/// </param>
	/// <returns>The target painter.</returns>
	ISudokuPainter WithFontScale(decimal fontScale);

	/// <summary>
	/// Sets a font name that is used for rendering text of candidate digits in a sudoku grid.
	/// </summary>
	/// <param name="fontName">The font name.</param>
	/// <returns>The target painter.</returns>
	ISudokuPainter WithCandidateFont(string fontName);

	/// <summary>
	/// Sets a font name that is used for rendering footer text.
	/// </summary>
	/// <param name="fontName">The font name.</param>
	/// <returns>The target painter.</returns>
	ISudokuPainter WithFooterTextFont(string fontName);

	/// <summary>
	/// Sets the footer text that can be rendered below the picture.
	/// </summary>
	/// <param name="footerText">The footer text.</param>
	/// <returns>The target painter.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	sealed ISudokuPainter WithFooterText(string footerText) => WithFooterText(footerText, TextAlignmentType.Center);

	/// <summary>
	/// Sets the footer text that can be rendered below the picture, with the specified alignment.
	/// </summary>
	/// <param name="footerText">The footer text.</param>
	/// <param name="alignment">The alignment.</param>
	/// <returns>The target painter.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="alignment"/> is not defined.</exception>
	ISudokuPainter WithFooterText(string footerText, TextAlignmentType alignment);

	/// <summary>
	/// Sets the footer text color that is used by rendering the text.
	/// </summary>
	/// <param name="color">The color to set. We do not recommend you use hard-reading colors such as <see cref="Color.Transparent"/>.</param>
	/// <returns>The target painter.</returns>
	/// <seealso cref="Color.Transparent"/>
	ISudokuPainter WithFooterTextColor(Color color);

	/// <summary>
	/// Sets the conclusions used for rendering.
	/// </summary>
	/// <param name="conclusions">The conclusions.</param>
	/// <returns>The target painter.</returns>
	ISudokuPainter WithConclusions(params Conclusion[] conclusions);

	/// <summary>
	/// Sets the view nodes used for rendering.
	/// </summary>
	/// <param name="nodes">The nodes.</param>
	/// <returns>The target painter.</returns>
	ISudokuPainter WithNodes(IEnumerable<ViewNode> nodes);

	/// <summary>
	/// Append extra nodes.
	/// </summary>
	/// <param name="nodes">Extra nodes.</param>
	/// <returns>The target instance.</returns>
	ISudokuPainter AddNodes(IEnumerable<ViewNode> nodes);

	/// <summary>
	/// Remove nodes.
	/// </summary>
	/// <param name="nodes">Nodes.</param>
	/// <returns>The target painter.</returns>
	ISudokuPainter RemoveNodes(IEnumerable<ViewNode> nodes);

	/// <summary>
	/// Remove nodes if the target node satisfies the specified condition.
	/// </summary>
	/// <param name="predicate">The predicate.</param>
	/// <returns>The target painter.</returns>
	ISudokuPainter RemoveNodesWhen(Predicate<ViewNode> predicate);
}
