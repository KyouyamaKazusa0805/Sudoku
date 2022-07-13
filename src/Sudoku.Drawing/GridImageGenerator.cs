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
public partial record class GridImageGenerator(
	IPointCalculator Calculator, IPreference Preferences, scoped in Grid Puzzle) :
	IGridImageGenerator
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
	public View? View { get; set; }

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

		DrawView(g, TextOffset);
		DrawFocusedCells(g);
		DrawEliminations(g, TextOffset);
		DrawValue(g);

		return bitmap;
	}

	/// <summary>
	/// Gets the color value.
	/// </summary>
	/// <param name="id">The identifier instance.</param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException">Throws when the specified value is invalid.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Color GetColor(Identifier id)
		=> id switch
		{
			{ Mode: IdentifierColorMode.Raw, A: var alpha, R: var red, G: var green, B: var blue }
				=> Color.FromArgb(alpha, red, green, blue),
			{ Mode: IdentifierColorMode.Id } when Preferences.TryGetColor(id, out var color)
				=> Color.FromArgb(64, color),
			{ Mode: IdentifierColorMode.Named, NamedKind: var namedKind }
				=> namedKind switch
				{
					DisplayColorKind.Normal => Preferences.Color1,
					DisplayColorKind.Elimination => Preferences.EliminationColor,
					DisplayColorKind.Exofin => Preferences.Color2,
					DisplayColorKind.Endofin => Preferences.Color3,
					DisplayColorKind.Cannibalism => Preferences.CannibalismColor,
					DisplayColorKind.Link => Preferences.ChainColor,
					_ => throw new InvalidOperationException("Such displaying color kind is invalid.")
				},
			_ => throw new InvalidOperationException("Such identifier instance contains invalid value.")
		};


	partial void DrawGridAndBlockLines(Graphics g);
	partial void DrawBackground(Graphics g);
	partial void DrawValue(Graphics g);
	partial void DrawFocusedCells(Graphics g);
	partial void DrawView(Graphics g, float offset);
	partial void DrawEliminations(Graphics g, float offset);
	partial void DrawCells(Graphics g);
	partial void DrawCandidates(Graphics g, float offset);
	partial void DrawRegions(Graphics g, float offset);
	partial void DrawLinks(Graphics g, float offset);
	partial void DrawDirectLines(Graphics g, float offset);
	partial void DrawUnknownValue(Graphics g);
}
