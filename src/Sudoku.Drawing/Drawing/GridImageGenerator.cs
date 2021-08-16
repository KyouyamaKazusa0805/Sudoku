using static Sudoku.Drawing.IGridImageGenerator;

namespace Sudoku.Drawing;

/// <summary>
/// Defines and encapsulates a data structure that provides the operations to draw a sudoku puzzle.
/// </summary>
/// <param name="Calculator">
/// Indicates the <see cref="IPointCalculator"/> instance that calculates the pixels to help the inner
/// methods to handle and draw the picture used for displaying onto the UI projects.
/// </param>
/// <param name="Preferences">
/// Indicates the <see cref="IPreference"/> instance that stores the default preferences
/// that decides the drawing behavior.
/// </param>
/// <param name="Puzzle">Indicates the puzzle.</param>
public partial record GridImageGenerator(IPointCalculator Calculator, IPreference Preferences, in Grid Puzzle) : IGridImageGenerator
{
	/// <inheritdoc/>
	public float Width
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Calculator.Width;
	}

	/// <inheritdoc/>
	public float Height
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Calculator.Height;
	}

	/// <inheritdoc/>
	public Cells FocusedCells { get; set; }

	/// <inheritdoc/>
	public PresentationData View { get; set; }

	/// <inheritdoc/>
	public PresentationData CustomView { get; set; }

	/// <inheritdoc/>
	public IEnumerable<Conclusion>? Conclusions { get; set; }


	/// <inheritdoc/>
	public Image DrawManually()
	{
		var result = new Bitmap((int)Width, (int)Height);
		using var g = Graphics.FromImage(result);
		return Draw(result, g);
	}

	/// <summary>
	/// To draw the image.
	/// </summary>
	/// <param name="bitmap">The bitmap result.</param>
	/// <param name="g">The graphics instance.</param>
	/// <returns>
	/// The return value is the same as the parameter <paramref name="bitmap"/> when
	/// this parameter is not <see langword="null"/>.
	/// </returns>
	public Image Draw([AllowNull] Image bitmap, Graphics g)
	{
		bitmap ??= new Bitmap((int)Width, (int)Height);

		DrawBackground(g);
		DrawGridAndBlockLines(g);

		g.SmoothingMode = SmoothingMode.AntiAlias;
		g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
		g.InterpolationMode = InterpolationMode.HighQualityBicubic;
		g.CompositingQuality = CompositingQuality.HighQuality;

		DrawView(g, View, TextOffset);
		DrawView(g, CustomView, TextOffset);
		DrawFocusedCells(g);
		DrawEliminations(g, TextOffset);
		DrawValue(g);

		return bitmap;
	}


#pragma warning disable IDE0055
	partial void DrawGridAndBlockLines(Graphics g);
	partial void DrawBackground       (Graphics g);
	partial void DrawValue            (Graphics g);
	partial void DrawFocusedCells     (Graphics g);
	partial void DrawView             (Graphics g,                          in PresentationData view,         float offset);
	partial void DrawEliminations     (Graphics g,                                                            float offset);
	partial void DrawCells            (Graphics g, IEnumerable<(         int, ColorIdentifier)> cells                     );
	partial void DrawCandidates       (Graphics g, IEnumerable<(         int, ColorIdentifier)> candidates,   float offset);
	partial void DrawRegions          (Graphics g, IEnumerable<(         int, ColorIdentifier)> regions,      float offset);
	partial void DrawLinks            (Graphics g, IEnumerable<(        Link, ColorIdentifier)> links,        float offset);
	partial void DrawDirectLines      (Graphics g, IEnumerable<(  Crosshatch, ColorIdentifier)> directLines,  float offset);
	partial void DrawUnknownValue     (Graphics g, IEnumerable<(UnknownValue, ColorIdentifier)> unknownValues             );
#pragma warning restore IDE0055
}
