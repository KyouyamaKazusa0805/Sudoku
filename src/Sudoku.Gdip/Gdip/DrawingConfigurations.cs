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
	public bool ShowLightHouse { get; set; } = false;

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
	/// Indicates the padding of figures.
	/// </summary>
	public float FigurePadding { get; set; } = 5F;

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
	/// Indicates the font of baba grouping characters to render.
	/// </summary>
	public string? BabaGroupingFontName { get; set; } = "Times New Roman";

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
	/// Indicates the font style of a baba group.
	/// </summary>
	public FontStyle BabaGroupCharacterFontStyle { get; set; } = FontStyle.Italic | FontStyle.Bold;

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
	/// Indicates the color of the baba grouping identifier color.
	/// </summary>
	public Color BabaGroupingCharacterColor { get; set; } = Color.FromArgb(192, Color.Red);

	/// <summary>
	/// Indicates footer text color.
	/// </summary>
	public Color FooterTextColor { get; set; } = Color.Black;

	/// <summary>
	/// The color palette. This property stores a list of customized colors to be used as user-defined colors.
	/// </summary>
	public Color[] ColorPalette { get; set; } = [
		Color.FromArgb(63, 218, 101), // Green (normal)
		Color.FromArgb(255, 192, 89), // Orange (auxiliary)
		Color.FromArgb(127, 187, 255), // Sky-blue (exo-fin)
		Color.FromArgb(216, 178, 255), // Purple (endo-fin)
		Color.FromArgb(197, 232, 140), // Yellow-green
		Color.FromArgb(255, 203, 203), // Light red (eliminations)
		Color.FromArgb(178, 223, 223), // Blue green
		Color.FromArgb(252, 220, 165), // Light orange
		Color.FromArgb(255, 255, 150), // Yellow
		Color.FromArgb(247, 222, 143), // Golden yellow
		Color.FromArgb(220, 212, 252), // Purple
		Color.FromArgb(255, 118, 132), // Red
		Color.FromArgb(206, 251, 237), // Light sky-blue
		Color.FromArgb(215, 255, 215), // Light green
		Color.FromArgb(192, 192, 192) // Gray
	];

	/// <summary>
	/// Indicates the font of footer text.
	/// </summary>
	public FontData FooterTextFont { get; set; } = new("MiSans", 24F, FontStyle.Bold);
}
