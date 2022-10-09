namespace Sudoku.Text.Notations;

/// <summary>
/// Represents a notation kind that describe a candidate list.
/// </summary>
public enum CandidateNotation
{
	/// <summary>
	/// Indicates the notation kind is <b>RxCy</b>.
	/// </summary>
	RxCy,

	/// <summary>
	/// Indicates the notation kind is <b>K9</b>.
	/// </summary>
	K9,

	/// <summary>
	/// Indicates the notation kind is <b>Susser Elimination Format</b>.
	/// </summary>
	SusserElimination
}
