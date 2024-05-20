#pragma warning disable CS1591, IDE0051, IDE0052

namespace Sudoku.Drawing;

/// <summary>
/// Represents a canvas that allows drawing sudoku-related items onto it.
/// </summary>
public sealed partial class GridCanvas : IDisposable
{
	/// <summary>
	/// Represents a radian value equivalent to 45 degree. This field is used for rotate the chains if overlapped.
	/// </summary>
	private const float RotateAngle = PI / 4;

	/// <summary>
	/// Represents square root of 2. This field is used for rotate the chains.
	/// </summary>
	private const float SqrtOf2 = 1.4142135F;


	/// <summary>
	/// Indicates the footer text.
	/// </summary>
	private readonly string? _footerText;

	/// <summary>
	/// Indicates the backing settings.
	/// </summary>
	private readonly GridCanvasSettings _settings;

	/// <summary>
	/// Indicates the backing point calculator to be used.
	/// </summary>
	private readonly PointCalculator _calculator;

	/// <summary>
	/// Represents a <see cref="StringFormat"/> instance that locates the text drawn, created by method
	/// <see cref="Graphics.DrawString(string?, Font, Brush, RectangleF, StringFormat?)"/>,
	/// with centering the text with both horizontal aligning and vertical aligning.
	/// </summary>
	/// <remarks><inheritdoc cref="_backingBitmap" path="/remarks"/></remarks>
	/// <seealso cref="Graphics.DrawString(string?, Font, Brush, RectangleF, StringFormat?)"/>
	private readonly StringFormat _stringAligner = new() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

	/// <summary>
	/// Indicates the backing bitmap to be used.
	/// </summary>
	/// <remarks>
	/// <b><i>This field sholud be released in <see cref="Dispose"/>.</i></b>
	/// </remarks>
	private readonly Bitmap _backingBitmap;

	/// <summary>
	/// Indicates the backing <see cref="Graphics"/> instance to draw elements.
	/// </summary>
	/// <remarks><inheritdoc cref="_backingBitmap" path="/remarks"/></remarks>
	private readonly Graphics _g;

	/// <summary>
	/// Indicates whether the object had already been disposed before <see cref="Dispose"/> method was called.
	/// If this field holds <see langword="false"/> value, <see cref="Dispose"/> method will throw an
	/// <see cref="ObjectDisposedException"/> to report the error.
	/// </summary>
	/// <seealso cref="Dispose"/>
	/// <seealso cref="ObjectDisposedException"/>
	private bool _isDisposed;


	/// <summary>
	/// Initializes a <see cref="GridCanvas"/> instance via some values.
	/// </summary>
	/// <param name="size">Indicates the picture size to be used.</param>
	/// <param name="padding">Indicaets the padding of the inner grid.</param>
	/// <param name="settings">Indicates settings that can be used by drawing items.</param>
	/// <param name="footerText">Indicates the footer text to be used.</param>
	public GridCanvas(int size, int padding, GridCanvasSettings? settings = null, string? footerText = null)
	{
		_settings = settings ?? new();
		_calculator = new(size, padding);
		_g = CreateGraphics(_footerText = footerText, size, settings, out _backingBitmap);
		Clear();
	}


	/// <inheritdoc/>
	/// <exception cref="ObjectDisposedException">Throws when the object had already been disposed.</exception>
	public void Dispose()
	{
		ObjectDisposedException.ThrowIf(_isDisposed, this);

		// To release fields, by calling 'Dispose' method.
		_stringAligner.Dispose();
		_backingBitmap.Dispose();
		_g.Dispose();

		// Set the field with 'true'.
		_isDisposed = true;
	}

	/// <summary>
	/// Try to save the picture into the local path.
	/// </summary>
	/// <param name="filePath">Indicates the file path.</param>
	/// <exception cref="ArgumentException">Throws when the file doesn't contain any valid file extension.</exception>
	/// <exception cref="NotSupportedException">Throws when the specified file extension is not supported.</exception>
	public void SavePictureTo(string filePath)
	{
		const string error_ContainsNoExtension = "The file path is not supported because the file doesn't contain any valid extension.";
		const string error_NotSupportedExtension = "The file path doesn't have a valid file extension to be output picture.";
		switch ((Path.GetExtension(filePath) is { Length: not 0 } p ? p : null)?.ToLower())
		{
			case null:
			{
				throw new ArgumentException(error_ContainsNoExtension, nameof(filePath));
			}
			case ".wmf":
			{
				using var tempBitmap = new Bitmap((int)_calculator.Width, (int)_calculator.Height);
				using var tempGraphics = Graphics.FromImage(tempBitmap);
				using var metaFile = new Metafile(filePath, tempGraphics.GetHdc());
				using var g = Graphics.FromImage(metaFile);
				g.DrawImage(_backingBitmap, 0, 0);
				tempGraphics.ReleaseHdc();
				break;
			}
			case { Length: >= 4 } e and (".jpg" or ".jpeg" or ".png" or ".bmp" or ".gif"):
			{
				_backingBitmap.Save(
					filePath,
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
				throw new NotSupportedException(error_NotSupportedExtension);
			}
		}
	}

	/// <summary>
	/// Try to get the result color value.
	/// </summary>
	/// <param name="paletteColorIndex">The value of ID.</param>
	/// <param name="result">The result color got.</param>
	/// <returns>The <see cref="bool"/> result.</returns>
	/// <exception cref="InvalidOperationException">Throws when the ID is invalid.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool TryGetPaletteColorByIndex(int paletteColorIndex, out Color result)
	{
		var palette = _settings.ColorPalette;
		return (result = palette.Count > paletteColorIndex ? palette[paletteColorIndex] : Color.Transparent) != Color.Transparent;
	}

	/// <summary>
	/// Gets the color value.
	/// </summary>
	/// <param name="id">The identifier instance.</param>
	/// <returns></returns>
	/// <exception cref="ArgumentException">Throws when the specified value is invalid.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Color GetColor(ColorIdentifier id)
		=> id switch
		{
			ColorColorIdentifier(var a, var r, var g, var b) => Color.FromArgb(a, r, g, b),
			PaletteIdColorIdentifier { Value: var value } when TryGetPaletteColorByIndex(value, out var color) => Color.FromArgb(64, color),
			WellKnownColorIdentifier { Kind: var kind } => kind switch
			{
				WellKnownColorIdentifierKind.Normal => _settings.NormalColor,
				WellKnownColorIdentifierKind.Assignment => _settings.AssignmentColor,
				WellKnownColorIdentifierKind.Elimination => _settings.EliminationColor,
				WellKnownColorIdentifierKind.Cannibalism => _settings.CannibalismColor,
				WellKnownColorIdentifierKind.Exofin => _settings.ExofinColor,
				WellKnownColorIdentifierKind.Endofin => _settings.EndofinColor,
				WellKnownColorIdentifierKind.Link => _settings.ChainColor,
				WellKnownColorIdentifierKind.Auxiliary1 => _settings.AuxiliaryColorSet[0],
				WellKnownColorIdentifierKind.Auxiliary2 => _settings.AuxiliaryColorSet[1],
				WellKnownColorIdentifierKind.Auxiliary3 => _settings.AuxiliaryColorSet[2],
				WellKnownColorIdentifierKind.AlmostLockedSet1 => _settings.AlmostLockedSetColorSet[0],
				WellKnownColorIdentifierKind.AlmostLockedSet2 => _settings.AlmostLockedSetColorSet[1],
				WellKnownColorIdentifierKind.AlmostLockedSet3 => _settings.AlmostLockedSetColorSet[2],
				WellKnownColorIdentifierKind.AlmostLockedSet4 => _settings.AlmostLockedSetColorSet[3],
				WellKnownColorIdentifierKind.AlmostLockedSet5 => _settings.AlmostLockedSetColorSet[4],
				_ => throw new ArgumentException("The specified identifier paletteColorIndex is invalid.", nameof(id))
			},
			_ => throw new ArgumentException("The specified identifier paletteColorIndex is invalid.", nameof(id))
		};


	/// <summary>
	/// Try to get extra height that footer text will use.
	/// </summary>
	/// <param name="footerText">Indicates the footer text.</param>
	/// <param name="size">Indicates the size to be used.</param>
	/// <param name="settings">Indicates the settings to be used.</param>
	/// <returns>Indicates the measured extra height.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float MeasureFooterTextExtraHeight(string footerText, int size, GridCanvasSettings? settings)
	{
		using var tempBitmap = new Bitmap(size, size);
		using var tempGraphics = Graphics.FromImage(tempBitmap);
		using var font = GetFooterTextFont(size, settings);
		return tempGraphics.MeasureString(footerText, font).Height;
	}

	/// <summary>
	/// Get the font via the specified name, size and the scale.
	/// </summary>
	/// <param name="size">The size that decides the default font size.</param>
	/// <param name="settings">Indicates the settings.</param>
	/// <returns>The font.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Font GetFooterTextFont(float size, GridCanvasSettings? settings)
		=> new((settings ??= new()).FooterTextFontName.Unwrap(), size * (float)settings.FooterTextScale, settings.FooterTextFontStyle);

	/// <summary>
	/// Get the font via the specified name, size and the scale.
	/// </summary>
	/// <param name="fontName">The font name that decides the font to use and presentation.</param>
	/// <param name="size">The size that decides the default font size.</param>
	/// <param name="scale">The scale that decides the result font size.</param>
	/// <param name="style">The style that decides the font style of the text in the picture.</param>
	/// <returns>The font.</returns>
	/// <exception cref="ArgumentNullException">Throws when <paramref name="fontName"/> is <see langword="null"/>.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Font GetFont(string? fontName, float size, decimal scale, FontStyle style)
		=> new(fontName.Unwrap(), size * (float)scale, style);

	/// <summary>
	/// Creates a <see cref="Graphics"/> instance via values.
	/// </summary>
	/// <param name="footerText">Indicates the footer text.</param>
	/// <param name="size">Indicates the size.</param>
	/// <param name="settings">Indicates the settings.</param>
	/// <param name="bitmap">Indicates the bitmap instance that returns after <see cref="Graphics"/> instance is created.</param>
	/// <returns>A <see cref="Graphics"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Graphics CreateGraphics(string? footerText, int size, GridCanvasSettings? settings, out Bitmap bitmap)
	{
		var g = Graphics.FromImage(bitmap = new(size, (int)(footerText is null ? size : size + MeasureFooterTextExtraHeight(footerText, size, settings))));
		g.SmoothingMode = SmoothingMode.HighQuality;
		g.TextRenderingHint = TextRenderingHint.AntiAlias;
		g.InterpolationMode = InterpolationMode.HighQualityBicubic;
		g.CompositingQuality = CompositingQuality.HighQuality;
		return g;
	}


	public partial void Clear();
	public partial void DrawBackground();
	public partial void DrawBorderLines();
	public partial void DrawFooterText();
	public partial void DrawGrid(ref readonly Grid grid);
	public partial void DrawCellViewNodes(ReadOnlySpan<CellViewNode> nodes);
	public partial void DrawCandidateViewNodes(ReadOnlySpan<CandidateViewNode> nodes, ReadOnlySpan<Conclusion> conclusions);
	public partial void DrawHouseViewNodes(ReadOnlySpan<HouseViewNode> nodes);
	public partial void DrawLinkViewNodes(ReadOnlySpan<LinkViewNode> nodes, ReadOnlySpan<Conclusion> conclusions);
	public partial void DrawChuteViewNodes(ReadOnlySpan<ChuteViewNode> nodes);
	public partial void DrawBabaGroupingViewNodes(ReadOnlySpan<BabaGroupViewNode> nodes);
	public partial void DrawEliminations(ReadOnlySpan<Conclusion> conclusions, ReadOnlySpan<CandidateViewNode> nodes);
	public partial void DrawIconViewNodes(ReadOnlySpan<IconViewNode> nodes);
}
