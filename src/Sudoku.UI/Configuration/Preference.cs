namespace Sudoku.UI.Configuration;

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

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	public bool AllowFocusing { get; set; } = true;

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

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>4</c>.
	/// </remarks>
	public double AuthorDefined_CrossMarkStrokeThickness { get; set; } = 4;

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
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color OutsideBorderColor { get; set; } = Colors.Black;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFFFFFF</c> (i.e. <see cref="Colors.White"/>).
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color GridBackgroundFillColor { get; set; } = Colors.White;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color BlockBorderColor { get; set; } = Colors.Black;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color CellBorderColor { get; set; } = Colors.Black;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFD3D3D3</c> (i.e. <see cref="Colors.LightGray"/>).
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color CandidateBorderColor { get; set; } = Colors.LightGray;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color GivenColor { get; set; } = Colors.Black;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF0000FF</c> (i.e. <see cref="Colors.Blue"/>).
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color ModifiableColor { get; set; } = Colors.Blue;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF696969</c> (i.e. <see cref="Colors.DimGray"/>).
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color CandidateColor { get; set; } = Colors.DimGray;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFF0000</c> (i.e. <see cref="Colors.Red"/>).
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color CellDeltaColor { get; set; } = Colors.Red;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFFB9B9</c>.
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color CandidateDeltaColor { get; set; } = Color.FromArgb(255, 255, 185, 185);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color MaskEllipseColor { get; set; } = Colors.Black;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFFC059</c>.
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color PaletteColor1 { get; set; } = Color.FromArgb(255, 255, 192, 89);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFB1A5F3</c>.
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color PaletteColor2 { get; set; } = Color.FromArgb(255, 177, 165, 243);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFF7A5A7</c>.
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color PaletteColor3 { get; set; } = Color.FromArgb(255, 247, 165, 167);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF86E8D0</c>.
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color PaletteColor4 { get; set; } = Color.FromArgb(255, 134, 232, 208);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF86F280</c>.
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color PaletteColor5 { get; set; } = Color.FromArgb(255, 134, 242, 128);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFF7DE8F</c>.
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color PaletteColor6 { get; set; } = Color.FromArgb(255, 247, 222, 143);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFDCD4FC</c>.
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color PaletteColor7 { get; set; } = Color.FromArgb(255, 220, 212, 252);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFFD2D2</c>.
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color PaletteColor8 { get; set; } = Color.FromArgb(255, 255, 210, 210);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFCEFBED</c>.
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color PaletteColor9 { get; set; } = Color.FromArgb(255, 206, 251, 237);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFD7FFD7</c>.
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color PaletteColor10 { get; set; } = Color.FromArgb(255, 215, 255, 215);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF3FDA65</c>.
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color NormalColor { get; set; } = Color.FromArgb(255, 63, 218, 101);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF7FBBFF</c>.
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color Auxiliary1Color { get; set; } = Color.FromArgb(255, 127, 187, 255);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFD8B2FF</c>.
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color Auxiliary2Color { get; set; } = Color.FromArgb(255, 216, 178, 255);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFFFF96</c>.
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color Auxiliary3Color { get; set; } = Color.FromArgb(255, 255, 255, 150);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFF7684</c>.
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color EliminationColor { get; set; } = Color.FromArgb(255, 255, 118, 132);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF7FBBFF</c>.
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color ExofinColor { get; set; } = Color.FromArgb(255, 127, 187, 255);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFD8B2FF</c>.
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color EndofinColor { get; set; } = Color.FromArgb(255, 216, 178, 255);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFEB0000</c>.
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color CannibalismColor { get; set; } = Color.FromArgb(255, 235, 0, 0);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFF0000</c>.
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color LinkColor { get; set; } = Color.FromArgb(255, 255, 0, 0);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFC5E88C</c>.
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color AlmostLockedSet1Color { get; set; } = Color.FromArgb(255, 197, 232, 140);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFFCBCB</c>.
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color AlmostLockedSet2Color { get; set; } = Color.FromArgb(255, 255, 203, 203);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFB2DFDF</c>.
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color AlmostLockedSet3Color { get; set; } = Color.FromArgb(255, 178, 223, 223);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFCDCA5</c>.
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color AlmostLockedSet4Color { get; set; } = Color.FromArgb(255, 252, 220, 165);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFFFF96</c>.
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color AlmostLockedSet5Color { get; set; } = Color.FromArgb(255, 255, 255, 150);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF0000FF</c> (i.e. <see cref="Colors.Blue"/>).
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color HighlightCellStrokeColor { get; set; } = Colors.Blue;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#200000FF</c> (i.e. <see cref="Colors.Blue"/> with alpha 48).
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color FocusedCellColor { get; set; } = Colors.Blue with { A = 48 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#400000FF</c> (i.e. <see cref="Colors.Blue"/> with alpha 32).
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color PeersFocusedCellColor { get; set; } = Colors.Blue with { A = 32 };

#if AUTHOR_FEATURE_CELL_MARKS
	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#80000000</c> (i.e. <see cref="Colors.Black"/> with alpha 64).
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color AuthorDefined_CellRectangleFillColor { get; set; } = Colors.Black with { A = 64 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#80000000</c> (i.e. <see cref="Colors.Black"/> with alpha 64).
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color AuthorDefined_CellCircleFillColor { get; set; } = Colors.Black with { A = 64 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#80000000</c> (i.e. <see cref="Colors.Black"/> with alpha 64).
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color AuthorDefined_CrossMarkStrokeColor { get; set; } = Colors.Black with { A = 64 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#80000000</c> (i.e. <see cref="Colors.Black"/> with alpha 64).
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color AuthorDefined_StarFillColor { get; set; } = Colors.Black with { A = 64 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#80000000</c> (i.e. <see cref="Colors.Black"/> with alpha 64).
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color AuthorDefined_TriangleFillColor { get; set; } = Colors.Black with { A = 64 };
#endif
}
