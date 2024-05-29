namespace Sudoku.Drawing;

/// <summary>
/// Represents with a preference type that stores the configurations on drawing.
/// </summary>
public sealed class GridCanvasSettings
{
	/// <summary>
	/// Indicates whether the form shows candidates.
	/// </summary>
	public bool ShowCandidates { get; set; } = true;

	/// <summary>
	/// Indicates whether the grid painter will use new algorithm to draw a house (lighter).
	/// </summary>
	public bool ShowLightHouse { get; set; } = false;

	/// <summary>
	/// Indicates whether border bars will fully overlaps the shared grid line while drawing.
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
	/// Indicates the scale that footer text will be drawn. The value unit is unified with
	/// <see cref="ValueScale"/> and <see cref="CandidateScale"/>.
	/// </summary>
	/// <seealso cref="ValueScale"/>
	/// <seealso cref="CandidateScale"/>
	public decimal FooterTextScale { get; set; } = .4M;

	/// <summary>
	/// Indicates the grid line width of the sudoku grid to draw.
	/// </summary>
	public float GridLineWidth { get; set; } = 1.5F;

	/// <summary>
	/// Indicates the block line width of the sudoku grid to draw.
	/// </summary>
	public float BlockLineWidth { get; set; } = 3F;

	/// <summary>
	/// Indicates the padding of figures.
	/// </summary>
	public float FigurePadding { get; set; } = 5F;

	/// <summary>
	/// Indicates the font of given digits to draw.
	/// </summary>
	public string? GivenFontName { get; set; } = "MiSans";

	/// <summary>
	/// Indicates the font of modifiable digits to draw.
	/// </summary>
	public string? ModifiableFontName { get; set; } = "MiSans";

	/// <summary>
	/// Indicates the font of candidate digits to draw.
	/// </summary>
	public string? CandidateFontName { get; set; } = "MiSans";

	/// <summary>
	/// Indicates the font of baba grouping characters to draw.
	/// </summary>
	public string? BabaGroupingFontName { get; set; } = "Times New Roman";

	/// <summary>
	/// Indicates the font of footer text to draw.
	/// </summary>
	public string? FooterTextFontName { get; set; } = "MiSans";

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
	/// Indicates the font style of footer text.
	/// </summary>
	public FontStyle FooterTextFontStyle { get; set; } = FontStyle.Bold;

	/// <summary>
	/// Indicates the given digits to draw.
	/// </summary>
	public Color GivenColor { get; set; } = Color.Black;

	/// <summary>
	/// Indicates the modifiable digits to draw.
	/// </summary>
	public Color ModifiableColor { get; set; } = Color.Blue;

	/// <summary>
	/// Indicates the candidate digits to draw.
	/// </summary>
	public Color CandidateColor { get; set; } = Color.DimGray;

	/// <summary>
	/// Indicates the normal color.
	/// </summary>
	public Color NormalColor { get; set; } = Color.FromArgb(63, 218, 101); // Green

	/// <summary>
	/// Indicates the color that draws for an assignment.
	/// </summary>
	public Color AssignmentColor { set; get; } = Color.FromArgb(63, 218, 101); // Green

	/// <summary>
	/// Indicates the elimination color.
	/// </summary>
	public Color EliminationColor { get; set; } = Color.FromArgb(255, 118, 132); // Red

	/// <summary>
	/// Indicates the cannibalism color.
	/// </summary>
	public Color CannibalismColor { get; set; } = Color.FromArgb(235, 0, 0); // Dark-red

	/// <summary>
	/// Indicates the exo-fin color.
	/// </summary>
	public Color ExofinColor { get; set; } = Color.FromArgb(255, 192, 89); // Orange

	/// <summary>
	/// Indicates the endo-fin color.
	/// </summary>
	public Color EndofinColor { get; set; } = Color.FromArgb(216, 178, 255); // Purple

	/// <summary>
	/// Indicates the chain color.
	/// </summary>
	public Color ChainColor { get; set; } = Color.Red;

	/// <summary>
	/// Indicates the background color of the sudoku grid to draw.
	/// </summary>
	public Color BackgroundColor { get; set; } = Color.White;

	/// <summary>
	/// Indicates the grid line color of the sudoku grid to draw.
	/// </summary>
	public Color GridLineColor { get; set; } = Color.Black;

	/// <summary>
	/// Indicates the block line color of the sudoku grid to draw.
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
	/// Indicates the auxiliary color set.
	/// </summary>
	public ColorPalette AuxiliaryColorSet { get; set; } = [
		Color.FromArgb(255, 192, 89), // Orange
		Color.FromArgb(127, 187, 255), // Sky-blue
		Color.FromArgb(216, 178, 255) // Purple
	];

	/// <summary>
	/// Indicates the almost locked set color set.
	/// </summary>
	public ColorPalette AlmostLockedSetColorSet { get; set; } = [
		Color.FromArgb(220, 212, 252), // Purple
		Color.FromArgb(255, 118, 132), // Red
		Color.FromArgb(206, 251, 237), // Light sky-blue
		Color.FromArgb(215, 255, 215), // Light green
		Color.FromArgb(192, 192, 192) // Gray
	];

	/// <summary>
	/// Indicates the user-defined color palette.
	/// </summary>
	public ColorPalette ColorPalette { get; set; } = [
		Color.FromArgb(63, 218, 101), // Green
		Color.FromArgb(255, 192, 89), // Orange
		Color.FromArgb(127, 187, 255), // Sky-blue
		Color.FromArgb(216, 178, 255), // Purple
		Color.FromArgb(197, 232, 140), // Yellow-green
		Color.FromArgb(255, 203, 203), // Light red
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
}
