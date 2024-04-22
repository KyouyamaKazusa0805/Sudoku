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
	public abstract ISudokuPainter WithSize(int size);

	/// <summary>
	/// Sets the offset of the canvas.
	/// </summary>
	/// <param name="padding">The padding of the canvas.</param>
	/// <returns>The target painter.</returns>
	public abstract ISudokuPainter WithPadding(int padding);

	/// <summary>
	/// Sets the grid of the canvas.
	/// </summary>
	/// <param name="grid">The new grid.</param>
	/// <returns>The target painter.</returns>
	public abstract ISudokuPainter WithGrid(ref readonly Grid grid);

	/// <summary>
	/// Sets the grid of the canvas, with the string representation.
	/// </summary>
	/// <param name="gridCode">The new grid string code.</param>
	/// <returns>The target painter.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed ISudokuPainter WithGridCode(string gridCode) => WithGrid(Grid.Parse(gridCode));

	/// <summary>
	/// Sets whether the candidates in the grid will also be rendered.
	/// </summary>
	/// <param name="renderingCandidates">The <see cref="bool"/> value indicating that.</param>
	/// <returns>The target painter.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed ISudokuPainter WithRenderingCandidates(bool renderingCandidates)
		=> WithPreferenceSettings(pref => pref.ShowCandidates = renderingCandidates);

	/// <summary>
	/// Sets a font name that is used for rendering text of value digits in a sudoku grid.
	/// </summary>
	/// <param name="fontName">The font name.</param>
	/// <returns>The target painter.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed ISudokuPainter WithValueFont(string fontName)
		=> WithPreferenceSettings(pref => pref.GivenFontName = pref.ModifiableFontName = fontName);

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed ISudokuPainter WithFontScale(decimal fontScale) => WithFontScale(fontScale, fontScale / 3);

	/// <summary>
	/// <inheritdoc cref="WithFontScale(decimal)" path="/summary"/>
	/// </summary>
	/// <param name="valueFontScale">
	/// <para>Indicates the desired font scale that is applied to values (given and modifiable values).</para>
	/// <para><inheritdoc cref="WithFontScale(decimal)" path="//param[@name='fontScale']/para[2]"/></para>
	/// <para><inheritdoc cref="WithFontScale(decimal)" path="//param[@name='fontScale']/para[3]"/></para>
	/// </param>
	/// <param name="candidateFontScale">
	/// <para>Indicates the desired font scale that is applied to candidates.</para>
	/// <para><inheritdoc cref="WithFontScale(decimal)" path="//param[@name='fontScale']/para[2]"/></para>
	/// <para><inheritdoc cref="WithFontScale(decimal)" path="//param[@name='fontScale']/para[3]"/></para>
	/// </param>
	/// <returns>
	/// <inheritdoc cref="WithFontScale(decimal)" path="/returns"/>
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed ISudokuPainter WithFontScale(decimal valueFontScale, decimal candidateFontScale)
		=> WithPreferenceSettings(pref => { pref.ValueScale = valueFontScale; pref.CandidateScale = candidateFontScale; });

	/// <summary>
	/// Sets a font name that is used for rendering text of candidate digits in a sudoku grid.
	/// </summary>
	/// <param name="fontName">The font name.</param>
	/// <returns>The target painter.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed ISudokuPainter WithCandidateFont(string fontName) => WithPreferenceSettings(pref => pref.CandidateFontName = fontName);

	/// <summary>
	/// Sets the preference to the target value.
	/// </summary>
	/// <param name="action">The action to set preference values.</param>
	/// <returns>The target painter.</returns>
	public abstract ISudokuPainter WithPreferenceSettings(Action<DrawingConfigurations> action);

	/// <summary>
	/// Sets the footer text that can be rendered below the picture.
	/// </summary>
	/// <param name="footerText">The footer text.</param>
	/// <returns>The target painter.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed ISudokuPainter WithFooterText(string footerText) => WithFooterText(footerText, TextAlignmentType.Center);

	/// <summary>
	/// Sets the footer text that can be rendered below the picture, with the specified alignment.
	/// </summary>
	/// <param name="footerText">The footer text.</param>
	/// <param name="alignment">The alignment.</param>
	/// <returns>The target painter.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="alignment"/> is not defined.</exception>
	public abstract ISudokuPainter WithFooterText(string footerText, TextAlignmentType alignment);

	/// <summary>
	/// Sets the conclusions used for rendering.
	/// </summary>
	/// <param name="conclusions">The conclusions.</param>
	/// <returns>The target painter.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed ISudokuPainter WithConclusions(params Conclusion[] conclusions) => WithConclusions(ImmutableArray.Create(conclusions));

	/// <summary>
	/// Sets the conclusions used for rendering.
	/// </summary>
	/// <param name="conclusions">The conclusions.</param>
	/// <returns>The target painter.</returns>
	public abstract ISudokuPainter WithConclusions(ImmutableArray<Conclusion> conclusions);

	/// <summary>
	/// Sets the view nodes used for rendering.
	/// </summary>
	/// <param name="nodes">The nodes.</param>
	/// <returns>The target painter.</returns>
	public abstract ISudokuPainter WithNodes(IEnumerable<ViewNode> nodes);

	/// <summary>
	/// Append extra nodes.
	/// </summary>
	/// <param name="nodes">Extra nodes.</param>
	/// <returns>The target painter.</returns>
	public abstract ISudokuPainter AddNodes(ReadOnlySpan<ViewNode> nodes);

	/// <summary>
	/// Remove nodes.
	/// </summary>
	/// <param name="nodes">Nodes.</param>
	/// <returns>The target painter.</returns>
	public abstract ISudokuPainter RemoveNodes(IEnumerable<ViewNode> nodes);

	/// <summary>
	/// Remove nodes if the target node satisfies the specified condition.
	/// </summary>
	/// <param name="predicate">The predicate.</param>
	/// <returns>The target painter.</returns>
	public abstract ISudokuPainter RemoveNodesWhen(Func<ViewNode, bool> predicate);
}
