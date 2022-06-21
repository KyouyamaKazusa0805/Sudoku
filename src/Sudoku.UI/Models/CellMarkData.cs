namespace Sudoku.UI.Models;

/// <summary>
/// Indicates the cell mark data of a complete sudoku grid.
/// </summary>
internal sealed class CellMarkData
{
	/// <summary>
	/// Indicates the grid raw value. The format is <c>"#"</c>.
	/// </summary>
	public required string GridRawValue { get; set; }

	/// <summary>
	/// Indicates whether the sudoku grid currently displays for candidates.
	/// </summary>
	public required bool ShowCandidates { get; set; }

	/// <summary>
	/// Indicates the full data.
	/// </summary>
	public required List<CellMarkInfo> Data { get; set; }
}
