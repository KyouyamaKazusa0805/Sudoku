#if AUTHOR_FEATURE_CANDIDATE_MARKS

namespace Sudoku.UI.Models;

/// <summary>
/// Indicates the candidate mark info that describes a candidate.
/// </summary>
[AutoDeconstruction(nameof(CellIndex), nameof(DigitIndex), nameof(PaletteColorIndex))]
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
	/// Indicates the color index in the palette.
	/// </summary>
	public required int PaletteColorIndex { get; set; }
}

#endif