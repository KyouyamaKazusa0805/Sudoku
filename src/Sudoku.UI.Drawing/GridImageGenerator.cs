namespace Sudoku.UI.Drawing;

/// <summary>
/// Defines and encapsulates a data structure that provides the operations to draw a sudoku puzzle.
/// </summary>
public sealed partial class GridImageGenerator
{
	/// <summary>
	/// The square root of 2.
	/// </summary>
	public const double SqrtOf2 = 1.4142135623730951;

	/// <summary>
	/// The rotate angle (45 degrees). This field is used for rotate the chains if some of them are overlapped.
	/// </summary>
	public const double RotateAngle = PI / 4;

	/// <summary>
	/// The text offset that corrects the pixel of the text output.
	/// </summary>
	public const double TextOffset = 6;


	/// <summary>
	/// Indicates the drawing width.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Throws when the property <see cref="Calculator"/> is <see langword="null"/>.
	/// </exception>
	public double Width =>
		Calculator?.Width ?? throw new InvalidOperationException($"The value can be get if and only if {nameof(Calculator)} isn't null.");

	/// <summary>
	/// Indicates the drawing height.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Throws when the property <see cref="Calculator"/> is <see langword="null"/>.
	/// </exception>
	public double Height =>
		Calculator?.Height ?? throw new InvalidOperationException($"The value can be get if and only if {nameof(Calculator)} isn't null.");

	/// <summary>
	/// Indicates the focused cells.
	/// </summary>
	public Cells FocusedCells { get; set; }

	/// <summary>
	/// Indicates the puzzle.
	/// </summary>
	public SudokuGrid Puzzle { get; set; }/* = SudokuGrid.Empty;*/

	/// <summary>
	/// Indicates the view.
	/// </summary>
	public PresentationData View { get; set; }

	/// <summary>
	/// Indicates all conclusions.
	/// </summary>
	public IEnumerable<Conclusion>? Conclusions { get; set; }

	/// <summary>
	/// Indicates the <see cref="PointCalculator"/> instance that calculates the pixels to help the inner
	/// methods to handle and draw the picture used for displaying onto the UI projects.
	/// </summary>
	[DisallowNull]
	public PointCalculator? Calculator { get; set; }

	/// <summary>
	/// Indicates the <see cref="Preference"/> instance that stores the default preferences
	/// that decides the drawing behavior.
	/// </summary>
	[DisallowNull]
	public Preference? Preferences { get; set; }


	/// <summary>
	/// Creates the <see cref="Shape"/> controls via the current data structure.
	/// </summary>
	/// <param name="baseCanvas">
	/// The <see cref="Canvas"/> control instance to store those children controls.
	/// </param>
	public void DrawOnto(Canvas baseCanvas)
	{
		var grid = new Grid();
		for (int i = 0; i < PointCalculator.AnchorsCount; i++)
		{
			grid.RowDefinitions.Add(new());
			grid.ColumnDefinitions.Add(new());
		}

		DrawBackground(grid);
		DrawGridAndBlockLines(grid);

		DrawView(grid);
		DrawFocusedCells(grid);
		DrawEliminations(grid);
		DrawValue(grid);

		baseCanvas.Children.Add(grid);
	}


	/// <summary>
	/// Get the font via the specified name, size and the scale.
	/// </summary>
	/// <param name="fontName">The font name that decides the font to use and presentation.</param>
	/// <param name="size">The size that decides the default font size.</param>
	/// <param name="scale">The scale that decides the result font size.</param>
	/// <returns>The font and its size calculated.</returns>
	/// <exception cref="ArgumentNullException">
	/// Throws when <paramref name="fontName"/> is <see langword="null"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static (FontFamily Font, double FontSize) GetFont(string? fontName, double size, decimal scale) =>
		(new(fontName ?? throw new ArgumentNullException(nameof(size))), size * (double)scale);


	partial void DrawGridAndBlockLines(Grid g);
	partial void DrawBackground(Grid g);
	partial void DrawValue(Grid g);
	partial void DrawFocusedCells(Grid g);
	partial void DrawView(Grid g);
	partial void DrawEliminations(Grid g);
	partial void DrawCells(Grid g);
	partial void DrawCandidates(Grid g);
	partial void DrawRegions(Grid g);
	partial void DrawLinks(Grid g);
	partial void DrawDirectLines(Grid g);
	partial void DrawUnknownValue(Grid g);
}
