#if AUTHOR_FEATURE_CANDIDATE_MARKS

namespace Sudoku.UI.Models;

/// <summary>
/// Indicates the candidate mark info that describes a candidate.
/// </summary>
[AutoDeconstruction(nameof(CellIndex), nameof(DigitIndex), nameof(Color))]
internal partial struct CandidateMarkInfo
{
	/// <summary>
	/// Indicates the cell index.
	/// </summary>
	public required int CellIndex { get; set; }

	/// <summary>
	/// Indicates the digit index.
	/// </summary>
	public required int DigitIndex { get; set; }

	/// <summary>
	/// Indicates the color value.
	/// </summary>
	[JsonConverter(typeof(ColorJsonConverter))]
	public required Color Color { get; set; }
}

#endif