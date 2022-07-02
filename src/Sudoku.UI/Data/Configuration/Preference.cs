#if AUTHOR_FEATURE_CELL_MARKS || AUTHOR_FEATURE_CANDIDATE_MARKS
#pragma warning disable IDE1006
#endif

namespace Sudoku.UI.Data.Configuration;

/// <summary>
/// Defines the user preferences in the program.
/// </summary>
public sealed class Preference : IDrawingPreference
{
	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	public bool ShowCandidates { get; set; } = true;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	public bool ShowCandidateBorderLines { get; set; } = false;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	public bool EnableDeltaValuesDisplaying { get; set; } = true;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	public bool DescendingOrderedInfoBarBoard { get; set; } = true;

	/// <summary>
	/// Indicates whether the program will use zero character <c>'0'</c> as the placeholder to describe empty cells
	/// in a sudoku grid that we should copied.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	public bool PlaceholderIsZero { get; set; } = true;

#if AUTHOR_FEATURE_CELL_MARKS || AUTHOR_FEATURE_CANDIDATE_MARKS
	/// <summary>
	/// Indicates whether the old shape should be covered when diffused.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	public bool __CoverOldShapeWhenDiffused { get; set; } = false;
#endif

	/// <summary>
	/// Indicates whether the picture will also be saved when a drawing data file is saved to local.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	public bool AlsoSavePictureWhenSaveDrawingData { get; set; } = true;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>0</c>.
	/// </remarks>
	public double OutsideBorderWidth { get; set; } = 0;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>4</c>.
	/// </remarks>
	public double BlockBorderWidth { get; set; } = 4;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>1</c>.
	/// </remarks>
	public double CellBorderWidth { get; set; } = 1;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>1</c>.
	/// </remarks>
	public double CandidateBorderWidth { get; set; } = 1;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>.8</c>.
	/// </remarks>
	public double ValueFontScale { get; set; } = .8;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>.25</c>.
	/// </remarks>
	public double CandidateFontScale { get; set; } = .25;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>3</c>.
	/// </remarks>
	public double HighlightCellStrokeThickness { get; set; } = 3;

#if AUTHOR_FEATURE_CELL_MARKS
	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>4</c>.
	/// </remarks>
	public double __CrossMarkStrokeThickness { get; set; } = 4;
#endif

#if AUTHOR_FEATURE_CANDIDATE_MARKS
	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>2</c>.
	/// </remarks>
	public double __CandidateMarkStrokeThickness { get; set; } = 2;
#endif

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>"Cascadia Mono"</c> in debug environment; else <c>"Tahoma"</c>.
	/// </remarks>
	public string ValueFontName { get; set; }
#if DEBUG
		= "Cascadia Mono";
#else
		= "Tahoma";
#endif

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>"Cascadia Mono"</c> in debug environment; else <c>"Tahoma"</c>.
	/// </remarks>
	public string CandidateFontName { get; set; }
#if DEBUG
		= "Cascadia Mono";
#else
		= "Tahoma";
#endif

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <see cref="PeerFocusingMode.FocusedCellAndPeerCells"/>.
	/// </remarks>
	public PeerFocusingMode PeerFocusingMode { get; set; } = PeerFocusingMode.FocusedCellAndPeerCells;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	public Color OutsideBorderColor { get; set; } = Colors.Black;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFFFFFF</c> (i.e. <see cref="Colors.White"/>).
	/// </remarks>
	public Color GridBackgroundFillColor { get; set; } = Colors.White;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	public Color BlockBorderColor { get; set; } = Colors.Black;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	public Color CellBorderColor { get; set; } = Colors.Black;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFD3D3D3</c> (i.e. <see cref="Colors.LightGray"/>).
	/// </remarks>
	public Color CandidateBorderColor { get; set; } = Colors.LightGray;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	public Color GivenColor { get; set; } = Colors.Black;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF0000FF</c> (i.e. <see cref="Colors.Blue"/>).
	/// </remarks>
	public Color ModifiableColor { get; set; } = Colors.Blue;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF696969</c> (i.e. <see cref="Colors.DimGray"/>).
	/// </remarks>
	public Color CandidateColor { get; set; } = Colors.DimGray;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFF0000</c> (i.e. <see cref="Colors.Red"/>).
	/// </remarks>
	public Color CellDeltaColor { get; set; } = Colors.Red;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFFB9B9</c>.
	/// </remarks>
	public Color CandidateDeltaColor { get; set; } = Color.FromArgb(255, 255, 185, 185);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	public Color MaskEllipseColor { get; set; } = Colors.Black;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF3FDA65</c>.
	/// </remarks>
	public Color NormalColor { get; set; } = Color.FromArgb(255, 63, 218, 101);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF7FBBFF</c>.
	/// </remarks>
	public Color Auxiliary1Color { get; set; } = Color.FromArgb(255, 127, 187, 255);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFD8B2FF</c>.
	/// </remarks>
	public Color Auxiliary2Color { get; set; } = Color.FromArgb(255, 216, 178, 255);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFFFF96</c>.
	/// </remarks>
	public Color Auxiliary3Color { get; set; } = Color.FromArgb(255, 255, 255, 150);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFF7684</c>.
	/// </remarks>
	public Color EliminationColor { get; set; } = Color.FromArgb(255, 255, 118, 132);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF7FBBFF</c>.
	/// </remarks>
	public Color ExofinColor { get; set; } = Color.FromArgb(255, 127, 187, 255);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFD8B2FF</c>.
	/// </remarks>
	public Color EndofinColor { get; set; } = Color.FromArgb(255, 216, 178, 255);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFEB0000</c>.
	/// </remarks>
	public Color CannibalismColor { get; set; } = Color.FromArgb(255, 235, 0, 0);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFF0000</c>.
	/// </remarks>
	public Color LinkColor { get; set; } = Color.FromArgb(255, 255, 0, 0);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFC5E88C</c>.
	/// </remarks>
	public Color AlmostLockedSet1Color { get; set; } = Color.FromArgb(255, 197, 232, 140);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFFCBCB</c>.
	/// </remarks>
	public Color AlmostLockedSet2Color { get; set; } = Color.FromArgb(255, 255, 203, 203);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFB2DFDF</c>.
	/// </remarks>
	public Color AlmostLockedSet3Color { get; set; } = Color.FromArgb(255, 178, 223, 223);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFCDCA5</c>.
	/// </remarks>
	public Color AlmostLockedSet4Color { get; set; } = Color.FromArgb(255, 252, 220, 165);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFFFF96</c>.
	/// </remarks>
	public Color AlmostLockedSet5Color { get; set; } = Color.FromArgb(255, 255, 255, 150);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF0000FF</c> (i.e. <see cref="Colors.Blue"/>).
	/// </remarks>
	public Color HighlightCellStrokeColor { get; set; } = Colors.Blue;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#300000FF</c> (i.e. <see cref="Colors.Blue"/> with alpha 48).
	/// </remarks>
	public Color FocusedCellColor { get; set; } = Colors.Blue with { A = 48 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#200000FF</c> (i.e. <see cref="Colors.Blue"/> with alpha 32).
	/// </remarks>
	public Color PeersFocusedCellColor { get; set; } = Colors.Blue with { A = 32 };

#if AUTHOR_FEATURE_CELL_MARKS
	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#40000000</c> (i.e. <see cref="Colors.Black"/> with alpha 64).
	/// </remarks>
	public Color __CellRectangleFillColor { get; set; } = Colors.Black with { A = 64 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#40000000</c> (i.e. <see cref="Colors.Black"/> with alpha 64).
	/// </remarks>
	public Color __CellCircleFillColor { get; set; } = Colors.Black with { A = 64 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#40000000</c> (i.e. <see cref="Colors.Black"/> with alpha 64).
	/// </remarks>
	public Color __CrossMarkStrokeColor { get; set; } = Colors.Black with { A = 64 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#40000000</c> (i.e. <see cref="Colors.Black"/> with alpha 64).
	/// </remarks>
	public Color __StarFillColor { get; set; } = Colors.Black with { A = 64 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#40000000</c> (i.e. <see cref="Colors.Black"/> with alpha 64).
	/// </remarks>
	public Color __TriangleFillColor { get; set; } = Colors.Black with { A = 64 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#40000000</c> (i.e. <see cref="Colors.Black"/> with alpha 64).
	/// </remarks>
	public Color __DiamondFillColor { get; set; } = Colors.Black with { A = 64 };
#endif

#if AUTHOR_FEATURE_CANDIDATE_MARKS
	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#80000000</c> (i.e. <see cref="Colors.Black"/> with alpha 128).
	/// </remarks>
	public Color __CandidateMarkStrokeColor { get; set; } = Colors.Black with { A = 128 };
#endif

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is an array of 10 elements:
	/// <list type="number">
	/// <item>#FFFFC059 (Orange)</item>
	/// <item>#FFB1A5F3 (Light purple)</item>
	/// <item>#FFF7A5A7 (Red)</item>
	/// <item>#FF86E8D0 (Sky blue)</item>
	/// <item>#FF86F280 (Light green)</item>
	/// <item>#FFF7DE8F (Light orange)</item>
	/// <item>#FFDCD4FC (Whitey purple)</item>
	/// <item>#FFFFD2D2 (Light red)</item>
	/// <item>#FFCEFBED (Whitey blue)</item>
	/// <item>#FFD7FFD7 (Whitey green)</item>
	/// </list>
	/// All values of this array are referenced from sudoku project
	/// <see href="https://sourceforge.net/projects/hodoku/">Hodoku</see>.
	/// </remarks>
	public Color[] PaletteColors { get; set; } =
	{
		Color.FromArgb(255, 255, 192, 89), // FFFFC059
		Color.FromArgb(255, 177, 165, 243), // FFB1A5F3
		Color.FromArgb(255, 247, 165, 167), // FFF7A5A7
		Color.FromArgb(255, 134, 232, 208), // FF86E8D0
		Color.FromArgb(255, 134, 242, 128), // FF86F280
		Color.FromArgb(255, 247, 222, 143), // FFF7DE8F
		Color.FromArgb(255, 220, 212, 252), // FFDCD4FC
		Color.FromArgb(255, 255, 210, 210), // FFFFD2D2
		Color.FromArgb(255, 206, 251, 237), // FFCEFBED
		Color.FromArgb(255, 215, 255, 215) // FFD7FFD7
	};


	/// <summary>
	/// Covers the config file by the specified preference instance.
	/// </summary>
	/// <param name="preference">
	/// The preference instance. If the value is <see langword="null"/>, no operation will be handled.
	/// </param>
	public void CoverPreferenceBy(Preference? preference)
	{
		if (preference is null)
		{
			return;
		}

		((IDrawingPreference)this).CoverPreferenceBy(preference);
	}
}
