namespace Sudoku.Drawing;

/// <summary>
/// Defines a sudoku painter instance.
/// </summary>
public interface ISudokuPainter : ISudokuPainterFactory
{
	/// <summary>
	/// The grid image generator.
	/// </summary>
	protected internal abstract GridImageGenerator GridImageGenerator { get; }


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
	public sealed void SaveTo(string path)
	{
		switch (Path.GetExtension(path)?.ToLower())
		{
			case null or []:
			{
				throw new ArgumentException(ResourceDictionary.Get("ArgCannotBeNull"), nameof(path));
			}
			case ".wmf":
			{
				using var tempBitmap = new Bitmap((int)GridImageGenerator.Width, (int)GridImageGenerator.Height);
				using var tempGraphics = Graphics.FromImage(tempBitmap);
				using var metaFile = new Metafile(path, tempGraphics.GetHdc());
				using var g = Graphics.FromImage(metaFile);
				GridImageGenerator.RenderTo(g);

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
				throw new NotSupportedException(ResourceDictionary.ExceptionMessage("FileFormatNotSupported"));
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
	public sealed bool TrySaveTo(string path)
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
	public abstract Image Render();

	/// <summary>
	/// Sets the footer text that can be rendered below the picture.
	/// </summary>
	/// <param name="footerText">The footer text.</param>
	/// <returns>The target painter.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed ISudokuPainter WithFooterTextIfNotNull(string? footerText) => footerText is not null ? WithFooterText(footerText) : this;


	/// <summary>
	/// The default singleton instance that you can get.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sealed ISudokuPainter Create(int canvasDefaultSize, int canvasOffset = 10) => new SudokuPainter(canvasDefaultSize, canvasOffset);

	/// <summary>
	/// Create an instance using the specified <see cref="SudokuPainterPropertySetter"/> method,
	/// and a default base <see cref="ISudokuPainter"/> instance.
	/// </summary>
	/// <param name="base">The base instance.</param>
	/// <param name="propertySetters">The property setter method.</param>
	/// <returns>Created <see cref="ISudokuPainter"/> instance.</returns>
	public static sealed ISudokuPainter Create(ISudokuPainter @base, SudokuPainterPropertySetter propertySetters)
	{
		foreach (var method in propertySetters.GetInvocations())
		{
			@base = method(@base);
		}

		return @base;
	}
}

/// <summary>
/// The backing type that implements type <see cref="ISudokuPainter"/>.
/// </summary>
/// <param name="defaultSize">The default size.</param>
/// <param name="defaultOffset">The default outside offset.</param>
/// <seealso cref="ISudokuPainter"/>
file sealed class SudokuPainter(int defaultSize, int defaultOffset) : ISudokuPainter
{
	/// <inheritdoc/>
	public GridImageGenerator GridImageGenerator { get; } = new(new(defaultSize, defaultOffset));


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Image Render() => GridImageGenerator.RenderTo();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ISudokuPainter WithSize(int size)
	{
		GridImageGenerator.Calculator = new(size);
		return this;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ISudokuPainter WithPadding(int padding)
	{
		GridImageGenerator.Calculator = new(GridImageGenerator.Width, padding);
		return this;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ISudokuPainter WithGrid(ref readonly Grid grid)
	{
		GridImageGenerator.Puzzle = grid;
		return this;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ISudokuPainter WithPreferenceSettings(Action<DrawingConfigurations> action)
	{
		action(GridImageGenerator.Preferences);
		return this;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ISudokuPainter WithFooterText(string footerText, TextAlignmentType alignment)
	{
		GridImageGenerator.FooterText = footerText;
		GridImageGenerator.FooterTextAlignment = alignment switch
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
	public ISudokuPainter WithConclusions(ImmutableArray<Conclusion> conclusions)
	{
		GridImageGenerator.Conclusions = conclusions;
		return this;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ISudokuPainter WithNodes(IEnumerable<ViewNode> nodes)
	{
		GridImageGenerator.View = [.. nodes];
		return this;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ISudokuPainter AddNodes(ReadOnlySpan<ViewNode> nodes)
	{
		var view = GridImageGenerator.View;
		if (view is null)
		{
			GridImageGenerator.View = [.. nodes];
		}
		else
		{
			view.AddRange(nodes);
		}

		return this;
	}

	/// <inheritdoc/>
	public ISudokuPainter RemoveNodes(IEnumerable<ViewNode> nodes)
	{
		if (GridImageGenerator.View is not { } view)
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
	public ISudokuPainter RemoveNodesWhen(Func<ViewNode, bool> predicate)
	{
		if (GridImageGenerator.View is not { } view)
		{
			goto ReturnThis;
		}

		foreach (var node in view)
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
