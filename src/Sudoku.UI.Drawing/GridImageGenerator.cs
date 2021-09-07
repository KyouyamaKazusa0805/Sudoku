namespace Sudoku.UI.Drawing;

/// <summary>
/// Defines and encapsulates a data structure that provides the operations to draw a sudoku puzzle.
/// </summary>
/// <param name="Calculator">
/// Indicates the <see cref="PointCalculator"/> instance that calculates the pixels to help the inner
/// methods to handle and draw the picture used for displaying onto the UI projects.
/// </param>
/// <param name="Preferences">
/// Indicates the <see cref="Preference"/> instance that stores the default preferences
/// that decides the drawing behavior.
/// </param>
public sealed partial record GridImageGenerator(PointCalculator Calculator, Preference Preferences)
{
	/// <summary>
	/// The square root of 2.
	/// </summary>
	public const double SqrtOf2 = 1.4142135623730951;

	/// <summary>
	/// The rotate angle (45 degrees). This field is used for rotate the chains if some of them are overlapped.
	/// </summary>
	public const double RotateAngle = Math.PI / 4;

	/// <summary>
	/// The text offset that corrects the pixel of the text output.
	/// </summary>
	public const double TextOffset = 6;


	/// <summary>
	/// Indicates the drawing width.
	/// </summary>
	public double Width
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Calculator.Width;
	}

	/// <summary>
	/// Indicates the drawing height.
	/// </summary>
	public double Height
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Calculator.Height;
	}

	/// <summary>
	/// Indicates the focused cells.
	/// </summary>
	public Cells FocusedCells { get; set; }

	/// <summary>
	/// Indicates the puzzle.
	/// </summary>
	public SudokuGrid Puzzle { get; set; } = SudokuGrid.Empty;

	/// <summary>
	/// Indicates the view.
	/// </summary>
	public PresentationData View { get; set; }

	/// <summary>
	/// Indicates all conclusions.
	/// </summary>
	public IEnumerable<Conclusion>? Conclusions { get; set; }


	/// <inheritdoc/>
	public Canvas Draw()
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

		var canvas = new Canvas();
		canvas.Children.Add(grid);
		return canvas;
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
