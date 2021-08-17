namespace Sudoku.Solving;

/// <summary>
/// Defines a reason why the <see cref="IPuzzleSolver"/> is failed to solve a puzzle.
/// </summary>
/// <seealso cref="IPuzzleSolver"/>
[Closed]
public enum FailedReason
{
	/// <summary>
	/// Indicates nothing goes wrong.
	/// </summary>
	Nothing,

	/// <summary>
	/// Indicates the failed reason is that the puzzle doesn't contain a valid solution.
	/// </summary>
	PuzzleHasNoSolution,

	/// <summary>
	/// Indicates the failed reason is that the puzzle contains multiple valid solutions.
	/// </summary>
	PuzzleHasMultipleSolutions,

	/// <summary>
	/// Indicates the failed reason is that the user has cancelled the task.
	/// </summary>
	UserCancelled,

	/// <summary>
	/// Indicates the failed reason is that the solver has encountered an error and couldn't solve,
	/// then an exception thrown.
	/// </summary>
	ExceptionThrown,

	/// <summary>
	/// Indicates the other reason to cause the error.
	/// </summary>
	Unknown
}
