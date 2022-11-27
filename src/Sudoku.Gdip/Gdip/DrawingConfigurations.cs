namespace Sudoku.Gdip;

/// <summary>
/// Represents with a preference type that stores the configurations on drawing.
/// </summary>
public sealed class DrawingConfigurations : ICloneable<DrawingConfigurations>
{
	/// <summary>
	/// Indicates whether the form shows candidates.
	/// </summary>
	public bool ShowCandidates { get; set; } = true;

	/// <summary>
	/// Indicates whether the grid painter will use new algorithm to render a house (lighter).
	/// </summary>
	public bool ShowLightHouse { get; set; } = true;

	/// <summary>
	/// Indicates whether border bars will fully overlaps the shared grid line while rendering.
	/// </summary>
	public bool BorderBarFullyOverlapsGridLine { get; set; } = false;

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
	/// Indicates the border bar width.
	/// </summary>
	public float BorderBarWidth { get; set; } = 6F;

	/// <summary>
	/// Indicates the footer text font size.
	/// </summary>
	public float FooterTextFontSize { get; set; } = 24F;

	/// <summary>
	/// Indicates the greater-than symbol text font size.
	/// </summary>
	public float GreaterThanTextFontSize { get; set; } = 24F;

	/// <summary>
	/// Indicates the XV symbol text font size.
	/// </summary>
	public float XvTextFontSize { get; set; } = 24F;

	/// <summary>
	/// Indicates the number label text font size.
	/// </summary>
	public float NumberLabelFontSize { get; set; } = 24F;

	/// <summary>
	/// Indicates the border width of Kropki dots.
	/// </summary>
	public float KropkiDotBorderWidth { get; set; } = 3F;

	/// <summary>
	/// Indicates the size of Kropki dots.
	/// </summary>
	public float KropkiDotSize { get; set; } = 6F;

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
	/// Indicates the font of greater-than signs.
	/// </summary>
	public string GreaterThanSignFontName { get; set; } = "Consolas";

	/// <summary>
	/// Indicates the font of XV signs.
	/// </summary>
	public string XvSignFontName { get; set; } = "Consolas";

	/// <summary>
	/// Indicates the font of number labels.
	/// </summary>
	public string NumberLabelFontName { get; set; } = "Consolas";

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
	/// Indicates the font style of greater-than signs.
	/// </summary>
	public FontStyle GreaterThanSignFontStyle { get; set; } = FontStyle.Bold;

	/// <summary>
	/// Indicates the font style of XV signs.
	/// </summary>
	public FontStyle XvSignFontStyle { get; set; } = FontStyle.Bold;

	/// <summary>
	/// Indicates the font style of number labels.
	/// </summary>
	public FontStyle NumberLabelFontStyle { get; set; } = FontStyle.Bold;

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DrawingConfigurations Clone() => this.ReflectionClone();

	/// <inheritdoc cref="ReflectionCopying.ReflectionCover{T}(T, T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void CoverBy(DrawingConfigurations @new) => this.ReflectionCover(@new);
}
