namespace Sudoku.Platforms.QQ.Configurations;

/// <summary>
/// Defines a type that stores a user-defined configurations.
/// </summary>
public sealed class UserDefinedConfigurations
{
	/// <summary>
	/// Indicates how many members will be displayed onto the ranking page. The more members to display, the longer the output text will be.
	/// </summary>
	/// <remarks>
	/// The default value is <c>15</c>.
	/// </remarks>
	[JsonPropertyName("rankingDisplayUsersCount")]
	public int RankingDisplayUsersCount { get; set; } = 15;

	/// <summary>
	/// Indicates the size of the rendering canvas used by <see cref="ISudokuPainter"/> to create <see cref="Bitmap"/> instances.
	/// </summary>
	/// <remarks>
	/// The default value is <c>1000</c>.
	/// </remarks>
	[JsonPropertyName("canvasSize")]
	public int CanvasSize { get; set; } = 1000;

	/// <summary>
	/// Indicates the padding of rendering canvas used by <see cref="ISudokuPainter"/> to create <see cref="Bitmap"/> instances.
	/// </summary>
	/// <remarks>
	/// The default value is <c>20</c>.
	/// </remarks>
	[JsonPropertyName("canvasPadding")]
	public int CanvasPadding { get; set; } = 20;

	/// <summary>
	/// Indicates the font scale used by <see cref="ISudokuPainter"/> to render given, modifiable values and candidates.
	/// </summary>
	/// <remarks>
	/// The default value is <c>new(1.0M, 0.4M)</c>.
	/// </remarks>
	[JsonPropertyName("fontScale")]
	public FontScale FontScale { get; set; } = new(1.0M, .4M);

	/// <summary>
	/// Indicates whether <see cref="ISudokuPainter"/> instance prints candidates.
	/// </summary>
	/// <remarks>
	/// The default value is <c><see cref="CandidatePrintingOptions.PrintIfPuzzleIndirect"/></c>.
	/// </remarks>
	/// <seealso cref="ISudokuPainter"/>
	/// <seealso cref="CandidatePrintingOptions"/>
	[JsonPropertyName("candidatePrinting")]
	public CandidatePrintingOptions CandidatePrinting { get; set; } = CandidatePrintingOptions.PrintIfPuzzleIndirect;
}
