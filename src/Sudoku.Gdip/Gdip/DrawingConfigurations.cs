namespace Sudoku.Gdip;

/// <summary>
/// Represents with a preference type that stores the configurations on drawing.
/// </summary>
public sealed class DrawingConfigurations
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
	/// Indicates the border width of Kropki dots.
	/// </summary>
	public float KropkiDotBorderWidth { get; set; } = 3F;

	/// <summary>
	/// Indicates the border width of clockface dots.
	/// </summary>
	public float ClockfaceDotBorderWidth { get; set; } = 3F;

	/// <summary>
	/// Indicates the width of neighbor signs.
	/// </summary>
	public float NeighborSignsWidth { get; set; } = 3F;

	/// <summary>
	/// Indicates the width of wheel.
	/// </summary>
	public float WheelWidth { get; set; } = 6F;

	/// <summary>
	/// Indicates the size of Kropki dots.
	/// </summary>
	public float KropkiDotSize { get; set; } = 6F;

	/// <summary>
	/// Indicates the size of battenburg.
	/// </summary>
	public float BattenburgSize { get; set; } = 6F;

	/// <summary>
	/// Indicates the size of clockface dot.
	/// </summary>
	public float ClockfaceDotSize { get; set; } = 6F;

	/// <summary>
	/// Indicates the size of quadruple max arrows.
	/// </summary>
	public float QuadrupleMaxArrowSize { get; set; } = 6F;

	/// <summary>
	/// Indicates the size of cell corner triangles.
	/// </summary>
	public float CellCornerTriangleSize { get; set; } = 6F;

	/// <summary>
	/// Indicates the width of average bars.
	/// </summary>
	public float AverageBarWidth { get; set; } = 6F;

	/// <summary>
	/// Indicates the width of cell corner arrows.
	/// </summary>
	public float CellCornerArrowWidth { get; set; } = 6F;

	/// <summary>
	/// Indicates the padding of neighbor signs.
	/// </summary>
	public float NeighborSignCellPadding { get; set; } = 5F;

	/// <summary>
	/// Indicates the padding of triangle sums.
	/// </summary>
	public float TriangleSumCellPadding { get; set; } = 5F;

	/// <summary>
	/// Indicates the padding of figures.
	/// </summary>
	public float FigurePadding { get; set; } = 5F;

	/// <summary>
	/// Indicates the padding of cell corner triangles.
	/// </summary>
	public float CellCornerTriangleCellPadding { get; set; } = 5F;

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
	/// Indicates wheel text color.
	/// </summary>
	public Color WheelTextColor { get; set; } = Color.Black;

	/// <summary>
	/// Indicates pencilmark text color.
	/// </summary>
	public Color PencilmarkTextColor { get; set; } = Color.DimGray;

	/// <summary>
	/// Indicates cell arrow color.
	/// </summary>
	public Color CellArrowColor { get; set; } = Color.FromArgb(128, Color.Black);

	/// <summary>
	/// The color palette. This property stores a list of customized colors to be used as user-defined colors.
	/// </summary>
	public Color[] ColorPalette { get; set; } =
	{
		Color.FromArgb( 63, 218, 101), // Green (normal)
		Color.FromArgb(255, 192,  89), // Orange (auxiliary)
		Color.FromArgb(127, 187, 255), // Skyblue (exo-fin)
		Color.FromArgb(216, 178, 255), // Purple (endo-fin)
		Color.FromArgb(197, 232, 140), // Yellowgreen
		Color.FromArgb(255, 203, 203), // Light red (eliminations)
		Color.FromArgb(178, 223, 223), // Blue green
		Color.FromArgb(252, 220, 165), // Light orange
		Color.FromArgb(255, 255, 150), // Yellow
		Color.FromArgb(247, 222, 143), // Golden yellow
		Color.FromArgb(220, 212, 252), // Purple
		Color.FromArgb(255, 118, 132), // Red
		Color.FromArgb(206, 251, 237), // Light skyblue
		Color.FromArgb(215, 255, 215), // Light green
		Color.FromArgb(192, 192, 192) // Gray
	};

	/// <summary>
	/// Indicates the font of footer text.
	/// </summary>
	public FontData FooterTextFont { get; set; } = new("MiSans", 24F, FontStyle.Bold);

	/// <summary>
	/// Indicates the font of greater-than signs.
	/// </summary>
	public FontData GreaterThanSignFont { get; set; } = new("Consolas", 24F, FontStyle.Bold);

	/// <summary>
	/// Indicates the font of XV signs.
	/// </summary>
	public FontData XvSignFont { get; set; } = new("Consolas", 24F, FontStyle.Bold);

	/// <summary>
	/// Indicates the font of number labels.
	/// </summary>
	public FontData NumberLabelFont { get; set; } = new("Consolas", 24F, FontStyle.Bold);

	/// <summary>
	/// Indicates the font of quadruple hint.
	/// </summary>
	public FontData QuadrupleHintFont { get; set; } = new("MiSans", 24F, FontStyle.Regular);

	/// <summary>
	/// Indicates the font of wheel text.
	/// </summary>
	public FontData WheelFont { get; set; } = new("MiSans", 12F, FontStyle.Regular);

	/// <summary>
	/// Indicates the font of pencilmarks.
	/// </summary>
	public FontData PencilmarkFont { get; set; } = new("Segoe UI", 12F, FontStyle.Regular);

	/// <summary>
	/// Indicates the font of star product star.
	/// </summary>
	public FontData StarProductStarFont { get; set; } = new("Times New Roman", 12F, FontStyle.Regular);

	/// <summary>
	/// Indicates the font of embedded skyscraper arrow.
	/// </summary>
	public FontData EmbeddedSkyscraperArrowFont { get; set; } = new("Times New Roman", 12F, FontStyle.Bold);
}
