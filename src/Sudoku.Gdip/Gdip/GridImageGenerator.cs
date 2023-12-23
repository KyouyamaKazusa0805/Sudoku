namespace Sudoku.Gdip;

/// <summary>
/// Defines and encapsulates a data structure that provides the operations to draw a sudoku puzzle.
/// </summary>
public sealed partial class GridImageGenerator
{
	/// <summary>
	/// The rotate angle (45 degrees). This field is used for rotate the chains if some of them are overlapped.
	/// </summary>
	private const float RotateAngle = MathF.PI / 4;


	/// <summary>
	/// Indicates the <see cref="StringFormat"/> instance that locates the text drawn by
	/// <see cref="Graphics.DrawString(string?, Font, Brush, RectangleF, StringFormat?)"/>,
	/// center the text with both horizontal and vertical.
	/// </summary>
	/// <seealso cref="Graphics.DrawString(string?, Font, Brush, RectangleF, StringFormat?)"/>
	private static readonly StringFormat StringLocating = new() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };


	/// <inheritdoc cref="GridImageGenerator(PointCalculator, DrawingConfigurations)"/>
	/// <summary>
	/// <inheritdoc path="/summary"/>
	/// </summary>
	/// <param name="canvasSize">The size of the drawing canvas.</param>
	/// <param name="padding">The padding.</param>
	[SetsRequiredMembers]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public GridImageGenerator(float canvasSize, float padding) : this(new(canvasSize, padding))
	{
	}

	/// <inheritdoc cref="GridImageGenerator(PointCalculator, DrawingConfigurations)"/>
	[SetsRequiredMembers]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public GridImageGenerator(PointCalculator calculator) : this(calculator, new())
	{
	}

	/// <summary>
	/// Initializes a <see cref="GridImageGenerator"/> instance via the specified values.
	/// </summary>
	/// <param name="calculator">The point calculator that is used for conversion of drawing pixels.</param>
	/// <param name="preferences">The user-defined preferences.</param>
	[SetsRequiredMembers]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public GridImageGenerator(PointCalculator calculator, DrawingConfigurations preferences)
		=> (Calculator, Preferences) = (calculator, preferences);


	/// <summary>
	/// Indicates the drawing width.
	/// </summary>
	public float Width => Calculator.Width;

	/// <summary>
	/// Indicates the drawing height.
	/// </summary>
	public float Height => Calculator.Height;

	/// <summary>
	/// Indicate the footer text. This property is optional, and you can keep this with <see langword="null"/> value
	/// if you don't want to make any footers on a picture.
	/// </summary>
	public string? FooterText { get; set; }

	/// <summary>
	/// Indicates the footer text alignment.
	/// </summary>
	public StringAlignment FooterTextAlignment { get; set; }

	/// <summary>
	/// Indicates the puzzle.
	/// </summary>
	public Grid Puzzle { get; set; } = Grid.Empty;

	/// <summary>
	/// Indicates all conclusions.
	/// </summary>
	public ImmutableArray<Conclusion> Conclusions { get; set; } = ImmutableArray<Conclusion>.Empty;

	/// <summary>
	/// Indicates the view.
	/// </summary>
	public View? View { get; set; }

	/// <summary>
	/// Indicates the <see cref="PointCalculator"/> instance that calculates the pixels to help the inner
	/// methods to handle and draw the picture used for displaying onto the UI projects.
	/// </summary>
	public required PointCalculator Calculator { get; set; }

	/// <summary>
	/// Indicates the <see cref="DrawingConfigurations"/> instance that stores the default preferences
	/// that decides the drawing behavior.
	/// </summary>
	public required DrawingConfigurations Preferences { get; set; }


	/// <summary>
	/// To render the image onto the canvas specified as parameter <paramref name="g"/> of type <see cref="Graphics"/>.
	/// </summary>
	/// <param name="g">The graphics instance as base canvas, offering APIs allowing you doing drawing operations.</param>
	/// <seealso cref="Graphics"/>
	public void RenderTo(Graphics g)
	{
		DrawBackground(g);
		DrawGridAndBlockLines(g);

		g.SmoothingMode = SmoothingMode.HighQuality;
		g.TextRenderingHint = TextRenderingHint.AntiAlias;
		g.InterpolationMode = InterpolationMode.HighQualityBicubic;
		g.CompositingQuality = CompositingQuality.HighQuality;

		DrawView(g);
		DrawEliminations(g);
		DrawValue(g);
		DrawFooterText(g);
	}

	/// <summary>
	/// Render the image, with automatically calculation to get the target <see cref="Image"/> instance, and then return it.
	/// </summary>
	/// <returns>The default-generated <see cref="Image"/> instance.</returns>
	public Image RenderTo()
	{
		using var data = GetFooterTextRenderingData();
		var (_, extraHeight, _) = data;

		// There is a little bug that this method ignores the case when the text is too long.
		// However, I don't want to handle on this case. If the text is too long, it will be overflown, as default case to be kept;
		// otherwise, the picture drawn will be aligned as left, which is not the expected result.
		var bitmap = new Bitmap((int)Width, (int)(FooterText is not null ? Height + extraHeight : Height));

		using var g = Graphics.FromImage(bitmap);
		RenderTo(g);

		return bitmap;
	}

	/// <summary>
	/// Gets the rendering data.
	/// </summary>
	/// <returns>Rendering data.</returns>
	internal TextRenderingData GetFooterTextRenderingData()
	{
		if (this is not { FooterTextAlignment: var footerAlignment, FooterText: var footer, Preferences.FooterTextFont: var fontData })
		{
			throw new();
		}

		using var tempBitmap = new Bitmap((int)Width, (int)Height);
		using var tempGraphics = Graphics.FromImage(tempBitmap);
		var footerFont = fontData.CreateFont();
		var (_, footerHeight) = footer is not null ? tempGraphics.MeasureString(footer, footerFont) : default;
		return new(footerFont, footerHeight, new() { Alignment = footerAlignment });
	}

	/// <summary>
	/// Try to get the result color value.
	/// </summary>
	/// <param name="value">The value of ID.</param>
	/// <param name="result">The result color got.</param>
	/// <returns>The <see cref="bool"/> result.</returns>
	/// <exception cref="InvalidOperationException">Throws when the ID is invalid.</exception>
	private bool GetValueById(int value, out Color result)
	{
		if (Preferences is { ColorPalette: var palette })
		{
			return (result = palette.Length > value ? palette[value] : Color.Transparent) != Color.Transparent;
		}

		result = Color.Transparent;
		return false;
	}

	/// <summary>
	/// Gets the color value.
	/// </summary>
	/// <param name="id">The identifier instance.</param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException">Throws when the specified value is invalid.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Color GetColor(ColorIdentifier id)
		=> id switch
		{
			ColorColorIdentifier(var a, var r, var g, var b) => Color.FromArgb(a, r, g, b),
			PaletteIdColorIdentifier { Value: var value } when GetValueById(value, out var color) => Color.FromArgb(64, color),
			WellKnownColorIdentifier { Kind: var kind } => kind switch
			{
				WellKnownColorIdentifierKind.Normal => Preferences.ColorPalette[0],
				WellKnownColorIdentifierKind.Assignment => Preferences.ColorPalette[0],
				WellKnownColorIdentifierKind.Elimination => Preferences.EliminationColor,
				WellKnownColorIdentifierKind.Exofin => Preferences.ColorPalette[1],
				WellKnownColorIdentifierKind.Endofin => Preferences.ColorPalette[2],
				WellKnownColorIdentifierKind.Cannibalism => Preferences.CannibalismColor,
				WellKnownColorIdentifierKind.Link => Preferences.ChainColor,
				WellKnownColorIdentifierKind.Auxiliary1 => Preferences.ColorPalette[1],
				WellKnownColorIdentifierKind.Auxiliary2 => Preferences.ColorPalette[2],
				WellKnownColorIdentifierKind.Auxiliary3 => Preferences.ColorPalette[3],
				WellKnownColorIdentifierKind.AlmostLockedSet1 => Preferences.ColorPalette[^5],
				WellKnownColorIdentifierKind.AlmostLockedSet2 => Preferences.ColorPalette[^4],
				WellKnownColorIdentifierKind.AlmostLockedSet3 => Preferences.ColorPalette[^3],
				WellKnownColorIdentifierKind.AlmostLockedSet4 => Preferences.ColorPalette[^2],
				WellKnownColorIdentifierKind.AlmostLockedSet5 => Preferences.ColorPalette[^1],
				_ => throw new InvalidOperationException("Such displaying color kind is invalid.")
			},
			_ => throw new InvalidOperationException("Such identifier instance contains invalid value.")
		};


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
		=> new(fontName ?? throw new ArgumentNullException(nameof(size)), size * (float)scale, style);
}
