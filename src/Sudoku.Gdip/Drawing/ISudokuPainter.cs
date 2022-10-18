namespace Sudoku.Drawing;

/// <summary>
/// Defines a sudoku painter instance.
/// </summary>
public interface ISudokuPainter
{
	/// <summary>
	/// Render the image with the current configuration, and save the image to the local path,
	/// and automatically release the memory while rendering and image creating.
	/// </summary>
	/// <param name="path">The local path.</param>
	/// <returns>
	/// A <see cref="bool"/> result indicating whether the file is successfully saved or not.
	/// All supported formats are:
	/// <list type="bullet">
	/// <item><see cref="ImageFormat.Jpeg"/></item>
	/// <item><see cref="ImageFormat.Png"/></item>
	/// <item><see cref="ImageFormat.Bmp"/></item>
	/// <item><see cref="ImageFormat.Gif"/></item>
	/// <item><see cref="ImageFormat.Wmf"/></item>
	/// </list>
	/// Other formats are not supported. This method will return <see langword="false"/> for not being supported.
	/// </returns>
	public sealed bool TrySaveTo(string path)
	{
		return Path.GetExtension(path) switch
		{
			{ Length: >= 4 } commonExtensions and (".jpg" or ".jpeg" or ".png" or ".bmp" or ".gif" or ".wmf") => s(this, commonExtensions),
			_ => false
		};


		bool s(ISudokuPainter @this, string commonExtensions)
		{
			using var imageRendered = @this.Render();
			try
			{
				imageRendered.Save(
					path,
					commonExtensions switch
					{
						".jpg" or ".jpeg" => ImageFormat.Jpeg,
						".png" => ImageFormat.Png,
						".bmp" => ImageFormat.Bmp,
						".gif" => ImageFormat.Gif,
						".wmf" => ImageFormat.Wmf
					}
				);

				return true;
			}
			catch
			{
				return false;
			}
		}
	}

	/// <summary>
	/// Render the image with the current configuration.
	/// </summary>
	/// <returns>
	/// <para>The <see cref="Image"/> created.</para>
	/// <para>
	/// <b>
	/// Please note that the method will return an <see cref="IDisposable"/> type, you should release it after used.
	/// The recommend pattern is using <see langword="using"/> statement:
	/// </b>
	/// <code><![CDATA[
	/// using var image = Render();
	/// // Then you can do something you want to do ...
	/// ]]></code>
	/// </para>
	/// </returns>
	/// <seealso cref="Image"/>
	/// <seealso cref="IDisposable"/>
	public abstract Image Render();

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
	/// The default singleton instance that you can get.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sealed ISudokuPainter CreatePainter(int canvasDefaultSize, int canvasOffset = 10)
		=> new SudokuPainter(canvasDefaultSize, canvasOffset);
}

/// <summary>
/// The backing type that implements type <see cref="ISudokuPainter"/>.
/// </summary>
/// <seealso cref="ISudokuPainter"/>
file sealed class SudokuPainter : ISudokuPainter
{
	/// <summary>
	/// The inner image generator instance.
	/// </summary>
	private readonly IGridImageGenerator _generator;


	/// <summary>
	/// Initializes a <see cref="SudokuPainter"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SudokuPainter(int defaultSize, int defaultOffset)
		=> _generator = CreateInstance(CreateConverter(defaultSize, defaultOffset), IPreference.Default, Grid.Empty);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Image Render() => _generator.DrawManually();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ISudokuPainter WithCanvasSize(int size)
	{
		_generator.Calculator = CreateConverter(size);
		return this;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ISudokuPainter WithCanvasOffset(int offset)
	{
		_generator.Calculator = CreateConverter(_generator.Width, offset);
		return this;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ISudokuPainter WithGrid(scoped in Grid grid)
	{
		_generator.Puzzle = grid;
		return this;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ISudokuPainter WithRenderingCandidates(bool renderingCandidates)
	{
		_generator.Preferences.ShowCandidates = renderingCandidates;
		return this;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ISudokuPainter WithValueFont(string fontName)
	{
		_generator.Preferences.GivenFontName = fontName;
		_generator.Preferences.ModifiableFontName = fontName;
		return this;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ISudokuPainter WithCandidateFont(string fontName)
	{
		_generator.Preferences.CandidateFontName = fontName;
		return this;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ISudokuPainter WithFontScale(decimal fontScale)
	{
		fontScale *= 1.5M;

		_generator.Preferences.ValueScale = fontScale;
		_generator.Preferences.CandidateScale = fontScale / 3;
		return this;
	}
}
