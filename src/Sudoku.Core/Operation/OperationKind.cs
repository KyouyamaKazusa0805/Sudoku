namespace Sudoku.Operation;

/// <summary>
/// Provides with a kind of difference of candidates.
/// </summary>
public enum OperationKind
{
	/// <summary>
	/// Indicates the difference kind is none.
	/// </summary>
	None,

	/// <summary>
	/// Indicates the difference kind is assignment.
	/// </summary>
	Assignment,

	/// <summary>
	/// Indicates the difference kind is appending candidates.
	/// </summary>
	Appending,

	/// <summary>
	/// Indicates the difference kind is elimination.
	/// </summary>
	Elimination,

	/// <summary>
	/// Indicates the difference kind is unknown.
	/// </summary>
	Unknown
}
