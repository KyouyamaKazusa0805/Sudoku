namespace Sudoku.Concepts.Notations;

/// <summary>
/// Represents a notation.
/// </summary>
public enum Notation : byte
{
	/// <summary>
	/// Indicates the RxCy notation.
	/// </summary>
	RxCy,

	/// <summary>
	/// Indicates the K9 notation.
	/// </summary>
	K9,

	/// <summary>
	/// Indicates the notation that is used by the program called Hodoku, as the eliminations.
	/// </summary>
	HodokuElimination
}
