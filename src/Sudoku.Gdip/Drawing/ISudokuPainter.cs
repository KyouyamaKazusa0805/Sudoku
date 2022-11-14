namespace Sudoku.Drawing;

/// <summary>
/// Defines a sudoku painter instance.
/// </summary>
public interface ISudokuPainter : ISudokuPainterFactory
{
	/// <summary>
	/// The width.
	/// </summary>
	float Width { get; }

	/// <summary>
	/// The height.
	/// </summary>
	float Height { get; }

	/// <summary>
	/// The grid image generator.
	/// </summary>
	protected internal IGridImageGenerator GridImageGenerator { get; }


	/// <summary>
	/// Render the image with the current configuration, and save the image to the local path,
	/// and automatically release the memory while rendering and image creating.
	/// </summary>
	/// <param name="path">The local path.</param>
	/// <exception cref="ArgumentException">
	/// Throws when the specified file format specified in the argument <paramref name="path"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="NotSupportedException">
	/// Throws when the specified file format specified in the argument <paramref name="path"/> is not supported.
	/// </exception>
	sealed void SaveTo(string path)
	{
		switch (Path.GetExtension(path)?.ToLower())
		{
			case null or []:
			{
				throw new ArgumentException("The specified file format is unknown (null), which is not allowed in this method.", nameof(path));
			}
			case ".wmf":
			{
				using var tempBitmap = new Bitmap((int)Width, (int)Height);
				using var tempGraphics = Graphics.FromImage(tempBitmap);
				using var metaFile = new Metafile(path, tempGraphics.GetHdc());
				using var g = Graphics.FromImage(metaFile);
				GridImageGenerator.Render(metaFile, g);

				tempGraphics.ReleaseHdc();

				break;
			}
			case { Length: >= 4 } e and (".jpg" or ".jpeg" or ".png" or ".bmp" or ".gif"):
			{
				using var imageRendered = Render();
				imageRendered.Save(
					path,
					e switch
					{
						".jpg" or ".jpeg" => ImageFormat.Jpeg,
						".png" => ImageFormat.Png,
						".bmp" => ImageFormat.Bmp,
						".gif" => ImageFormat.Gif
					}
				);

				break;
			}
			default:
			{
				throw new NotSupportedException("The specified file format is not supported.");
			}
		}
	}

	/// <summary>
	/// Render the image with the current configuration, and save the image to the local path,
	/// and automatically release the memory while rendering and image creating.
	/// </summary>
	/// <param name="path">The local path.</param>
	/// <returns>
	/// A <see cref="bool"/> result indicating whether the file is successfully saved or not.
	/// All supported formats are:
	/// <list type="bullet">
	/// <item><c>*.jpg</c> and <c>*.jpeg</c></item>
	/// <item><c>*.png</c></item>
	/// <item><c>*.bmp</c></item>
	/// <item><c>*.gif</c></item>
	/// <item><c>*.wmf</c></item>
	/// </list>
	/// Other formats are not supported. This method will return <see langword="false"/> for not being supported.
	/// </returns>
	sealed bool TrySaveTo(string path)
	{
		try
		{
			SaveTo(path);

			return true;
		}
		catch (Exception ex) when (ex is NotSupportedException or ArgumentException)
		{
			return false;
		}
		catch
		{
			throw;
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
	Image Render();

	/// <summary>
	/// Sets the footer text that can be rendered below the picture.
	/// </summary>
	/// <param name="footerText">The footer text.</param>
	/// <returns>The target painter.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	sealed ISudokuPainter WithFooterTextIfNotNull(string? footerText) => footerText is not null ? WithFooterText(footerText) : this;


	/// <summary>
	/// The default singleton instance that you can get.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static sealed ISudokuPainter Create(int canvasDefaultSize, int canvasOffset = 10) => new SudokuPainter(canvasDefaultSize, canvasOffset);
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
		=> _generator = Create(Create(defaultSize, (float)defaultOffset), IPreference.Default, Grid.Empty);


	/// <inheritdoc/>
	public float Width => _generator.Width;

	/// <inheritdoc/>
	public float Height => _generator.Height;

	/// <inheritdoc/>
	IGridImageGenerator ISudokuPainter.GridImageGenerator => _generator;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Image Render() => _generator.Render();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ISudokuPainter WithCanvasSize(int size)
	{
		_generator.Calculator = Create(size);
		return this;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ISudokuPainter WithCanvasOffset(int offset)
	{
		_generator.Calculator = Create(_generator.Width, offset);
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
		_generator.Preferences.GivenFontName = _generator.Preferences.ModifiableFontName = fontName;
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
	public ISudokuPainter WithFooterTextFont(string fontName)
	{
		_generator.Preferences.FooterTextFontName = fontName;
		return this;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ISudokuPainter WithFontScale(decimal fontScale)
	{
		_generator.Preferences.ValueScale = fontScale;
		_generator.Preferences.CandidateScale = fontScale / 3;
		return this;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ISudokuPainter WithFooterText(string footerText, TextAlignmentType alignment)
	{
		_generator.FooterText = footerText;
		_generator.FooterTextAlignment = alignment switch
		{
			TextAlignmentType.Left => StringAlignment.Near,
			TextAlignmentType.Center => StringAlignment.Center,
			TextAlignmentType.Right => StringAlignment.Far,
			_ => throw new ArgumentOutOfRangeException(nameof(alignment))
		};

		return this;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ISudokuPainter WithFooterTextColor(Color color)
	{
		_generator.Preferences.FooterTextColor = color;
		return this;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ISudokuPainter WithConclusions(params Conclusion[] conclusions)
	{
		_generator.Conclusions = conclusions;
		return this;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ISudokuPainter WithNodes(IEnumerable<ViewNode> nodes)
	{
		_generator.View = View.Empty | nodes;
		return this;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ISudokuPainter AddNodes(IEnumerable<ViewNode> nodes)
	{
		_generator.View ??= View.Empty;
		_generator.View.AddRange(nodes);

		return this;
	}

	/// <inheritdoc/>
	public ISudokuPainter RemoveNodes(IEnumerable<ViewNode> nodes)
	{
		if (_generator.View is not { } view)
		{
			goto ReturnThis;
		}

		foreach (var node in nodes)
		{
			view.Remove(node);
		}

	ReturnThis:
		return this;
	}

	/// <inheritdoc/>
	public ISudokuPainter RemoveNodesWhen(Predicate<ViewNode> predicate)
	{
		if (_generator.View is not { } view)
		{
			goto ReturnThis;
		}

		var nodes = view.ToArray();
		foreach (var node in nodes)
		{
			if (predicate(node))
			{
				view.Remove(node);
			}
		}

	ReturnThis:
		return this;
	}
}
