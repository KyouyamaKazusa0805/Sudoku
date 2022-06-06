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
	/// The default value is <c>"Tahoma"</c>.
	/// </remarks>
	public string ValueFontName { get; set; } = "Tahoma";

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>"Tahoma"</c>.
	/// </remarks>
	public string CandidateFontName { get; set; } = "Tahoma";

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color OutsideBorderColor { get; set; } = Colors.Black;

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
	/// The default value is <c>#FFFFB9B9</c> (i.e. <see cref="Color"/> {255, 255, 185, 185}).
	/// </remarks>
	[JsonConverter(typeof(ColorJsonConverter))]
	public Color CandidateDeltaColor { get; set; } = Color.FromArgb(255, 255, 185, 185);
}
