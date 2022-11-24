namespace Sudoku.Gdip;

/// <summary>
/// Represents with a preference type that stores the configurations on drawing.
/// </summary>
public sealed class DrawingConfigurations : ICloneable<DrawingConfigurations>
{
	/// <summary>
	/// Initializes a <see cref="DrawingConfigurations"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private DrawingConfigurations()
	{
	}


	/// <summary>
	/// Indicates the singleton instance.
	/// </summary>
	public static DrawingConfigurations Instance => new();


	/// <summary>
	/// Indicates whether the form shows candidates.
	/// </summary>
	public bool ShowCandidates { get; set; } = true;

	/// <summary>
	/// Indicates whether the grid painter will use new algorithm to render a house (lighter).
	/// </summary>
	public bool ShowLightHouse { get; set; } = true;

	/// <summary>
	/// Indicates the scale of values.
	/// </summary>
	public decimal ValueScale { get; set; } = .9M;

	/// <summary>
	/// Indicates the scale of candidates.
	/// </summary>
	public decimal CandidateScale { get; set; } = .3M;

	/// <summary>
	/// Indicates the grid line width of the sudoku grid to render.
	/// </summary>
	public float GridLineWidth { get; set; } = 1.5F;

	/// <summary>
	/// Indicates the block line width of the sudoku grid to render.
	/// </summary>
	public float BlockLineWidth { get; set; } = 3F;

	/// <summary>
	/// Indicates the footer text font size.
	/// </summary>
	public float FooterTextFontSize { get; set; } = 24;

	/// <summary>
	/// Indicates the font of given digits to render.
	/// </summary>
	public string? GivenFontName { get; set; } = "MiSans";

	/// <summary>
	/// Indicates the font of modifiable digits to render.
	/// </summary>
	public string? ModifiableFontName { get; set; } = "MiSans";

	/// <summary>
	/// Indicates the font of candidate digits to render.
	/// </summary>
	public string? CandidateFontName { get; set; } = "MiSans";

	/// <summary>
	/// Indicates the font of unknown values to render.
	/// </summary>
	public string? UnknownFontName { get; set; } = "Times New Roman";

	/// <summary>
	/// Indicates the font of footer text.
	/// </summary>
	public string FooterTextFontName { get; set; } = "MiSans";

	/// <summary>
	/// Indicates the font style of the givens.
	/// </summary>
	public FontStyle GivenFontStyle { get; set; } = FontStyle.Regular;

	/// <summary>
	/// Indicates the font style of the modifiables.
	/// </summary>
	public FontStyle ModifiableFontStyle { get; set; } = FontStyle.Regular;

	/// <summary>
	/// Indicates the font style of the candidates.
	/// </summary>
	public FontStyle CandidateFontStyle { get; set; } = FontStyle.Regular;

	/// <summary>
	/// Indicates the font style of an unknown.
	/// </summary>
	public FontStyle UnknownFontStyle { get; set; } = FontStyle.Italic | FontStyle.Bold;

	/// <summary>
	/// Indicates the font style of footer text.
	/// </summary>
	public FontStyle FooterTextFontStyle { get; set; } = FontStyle.Bold;

	/// <summary>
	/// Indicates the given digits to render.
	/// </summary>
	public Color GivenColor { get; set; } = Color.Black;

	/// <summary>
	/// Indicates the modifiable digits to render.
	/// </summary>
	public Color ModifiableColor { get; set; } = Color.Blue;

	/// <summary>
	/// Indicates the candidate digits to render.
	/// </summary>
	public Color CandidateColor { get; set; } = Color.DimGray;

	/// <summary>
	/// Indicates the color used for painting for focused cells.
	/// </summary>
	public Color FocusedCellColor { get; set; } = Color.FromArgb(32, Color.Yellow);

	/// <summary>
	/// Indicates the elimination color.
	/// </summary>
	public Color EliminationColor { get; set; } = Color.FromArgb(255, 118, 132);

	/// <summary>
	/// Indicates the cannibalism color.
	/// </summary>
	public Color CannibalismColor { get; set; } = Color.FromArgb(235, 0, 0);

	/// <summary>
	/// Indicates the chain color.
	/// </summary>
	public Color ChainColor { get; set; } = Color.Red;

	/// <summary>
	/// Indicates the background color of the sudoku grid to render.
	/// </summary>
	public Color BackgroundColor { get; set; } = Color.White;

	/// <summary>
	/// Indicates the grid line color of the sudoku grid to render.
	/// </summary>
	public Color GridLineColor { get; set; } = Color.Black;

	/// <summary>
	/// Indicates the block line color of the sudoku grid to render.
	/// </summary>
	public Color BlockLineColor { get; set; } = Color.Black;

	/// <summary>
	/// Indicates the color of the crosshatching outline.
	/// </summary>
	public Color CrosshatchingOutlineColor { get; set; } = Color.FromArgb(192, Color.Black);

	/// <summary>
	/// Indicates the color of the crosshatching inner.
	/// </summary>
	public Color CrosshatchingInnerColor { get; set; } = Color.Transparent;

	/// <summary>
	/// Indicates the color of the unknown identifier color.
	/// </summary>
	public Color UnknownIdentifierColor { get; set; } = Color.FromArgb(192, Color.Red);

	/// <summary>
	/// Indicates the color of the cross sign.
	/// </summary>
	public Color CrossSignColor { get; set; } = Color.FromArgb(192, Color.Black);

	/// <summary>
	/// Indicates footer text color.
	/// </summary>
	public Color FooterTextColor { get; set; } = Color.Black;

	/// <summary>
	/// The color palette. This property stores a list of customized colors to be used as user-defined colors.
	/// </summary>
	public Color[] ColorPalette { get; set; } =
	{
		Color.FromArgb( 63, 218, 101),
		Color.FromArgb(255, 192,  89),
		Color.FromArgb(127, 187, 255),
		Color.FromArgb(216, 178, 255),
		Color.FromArgb(197, 232, 140),
		Color.FromArgb(255, 203, 203),
		Color.FromArgb(178, 223, 223),
		Color.FromArgb(252, 220, 165),
		Color.FromArgb(255, 255, 150),
		Color.FromArgb(247, 222, 143),
		Color.FromArgb(220, 212, 252),
		Color.FromArgb(255, 118, 132),
		Color.FromArgb(206, 251, 237),
		Color.FromArgb(215, 255, 215),
		Color.FromArgb(192, 192, 192)
	};


	/// <inheritdoc/>
	public DrawingConfigurations Clone()
	{
		var instance = Instance;
		foreach (var propertyInfo in typeof(DrawingConfigurations).GetProperties(BindingFlags.Instance | BindingFlags.Public))
		{
			if (propertyInfo is { CanRead: true, CanWrite: true })
			{
				var originalValue = propertyInfo.GetValue(this);
				propertyInfo.SetValue(instance, originalValue);
			}
		}

		return instance;
	}

	/// <summary>
	/// Copies and covers the current instance via the newer instance.
	/// </summary>
	/// <param name="newPreferences">The newer instance to copy.</param>
	public void CoverBy(DrawingConfigurations newPreferences)
	{
		foreach (var propertyInfo in typeof(DrawingConfigurations).GetProperties(BindingFlags.Instance | BindingFlags.Public))
		{
			if (propertyInfo is { CanRead: true, CanWrite: true })
			{
				var originalValue = propertyInfo.GetValue(newPreferences);
				propertyInfo.SetValue(this, originalValue);
			}
		}
	}
}
