#if AUTHOR_FEATURE_CELL_MARKS || AUTHOR_FEATURE_CANDIDATE_MARKS

namespace Sudoku.UI.Models;

/// <summary>
/// Indicates the serializable drawing data of a complete sudoku grid.
/// </summary>
[AutoDeconstruction(nameof(GridRawValue), nameof(ShowCandidates), nameof(CellData), nameof(CandidateData))]
internal sealed partial class SerializableDrawingData
{
	/// <summary>
	/// Indicates the grid raw value. The format is <c>"#"</c>.
	/// </summary>
	[JsonPropertyOrder(0)]
	public required string GridRawValue { get; set; }

	/// <summary>
	/// Indicates whether the sudoku grid currently displays for candidates.
	/// </summary>
	[JsonPropertyOrder(1)]
	public required bool ShowCandidates { get; set; }

#if AUTHOR_FEATURE_CELL_MARKS
	/// <summary>
	/// Indicates the full cell data.
	/// </summary>
	public required List<CellMarkInfo> CellData { get; set; }
#endif

#if AUTHOR_FEATURE_CANDIDATE_MARKS
	/// <summary>
	/// Indicates the full candidate data.
	/// </summary>
	public required List<CandidateMarkInfo> CandidateData { get; set; }
#endif
}

#endif