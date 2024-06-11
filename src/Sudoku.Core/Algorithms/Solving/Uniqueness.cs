namespace Sudoku.Algorithms.Solving;

/// <summary>
/// Represents a flag describing the number of solutions to a puzzle.
/// </summary>
/// <remarks><include file="../../global-doc-comments.xml" path="/g/flags-attribute"/></remarks>
[Flags]
public enum Uniqueness
{
	/// <summary>
	/// The placeholder of this type.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the puzzle has no valid solution.
	/// </summary>
	Bad = 1 << 0,

	/// <summary>
	/// Indicates the puzzle has a unique solution.
	/// </summary>
	Unique = 1 << 1,

	/// <summary>
	/// Indicates the puzzle has multiple solutions.
	/// </summary>
	Multiple = 1 << 2
}
