namespace Sudoku.Solving;

/// <summary>
/// Defines a reason why the <see cref="IPuzzleSolver"/> is failed to solve a puzzle.
/// </summary>
/// <seealso cref="IPuzzleSolver"/>
public enum FailedReason : byte
{
	/// <summary>
	/// Indicates nothing goes wrong.
	/// </summary>
	Nothing,

	/// <summary>
	/// Indicates the failed reason is that the puzzle doesn't contain a valid unique solution.
	/// Different with <see cref="PuzzleHasMultipleSolutions"/> and <see cref="PuzzleHasNoSolution"/>,
	/// this field will include more generic cases. If the puzzle doesn't pass the pre-process operation
	/// before solving, we should use this field.
	/// </summary>
	/// <seealso cref="PuzzleHasNoSolution"/>
	/// <seealso cref="PuzzleHasMultipleSolutions"/>
	PuzzleIsInvalid,

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
	/// Indicates the failed reason is that the solver doesn't implemented some function
	/// that causes the operation can't be finished.
	/// </summary>
	NotImplemented,

	/// <summary>
	/// Indicates the failed reason is that the solver has encountered an error and couldn't solve,
	/// then an exception thrown.
	/// </summary>
	ExceptionThrown,

	/// <summary>
	/// Indicates the failed reason is that the solver has found a wrong step that cause the grid become invalid.
	/// </summary>
	WrongStep,

	/// <summary>
	/// Indicates the failed reason is that the puzzle is too hard to solve. The solver gave up.
	/// </summary>
	PuzzleIsTooHard,

	/// <summary>
	/// Indicates the other reason to cause the error.
	/// </summary>
	Unknown
}
