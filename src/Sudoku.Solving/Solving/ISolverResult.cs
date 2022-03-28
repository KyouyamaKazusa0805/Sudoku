namespace Sudoku.Solving;

/// <summary>
/// Defines a result that created by a solver to represent a result of analysis after the solver calculated.
/// </summary>
public interface ISolverResult
{
	/// <summary>
	/// Indicates whether the solver has solved the puzzle.
	/// </summary>
	bool IsSolved { get; init; }

	/// <summary>
	/// Indicates why the solving operation is failed. This property is useless when <see cref="IsSolved"/>
	/// keeps the <see langword="true"/> value.
	/// </summary>
	/// <seealso cref="IsSolved"/>
	FailedReason FailedReason { get; init; }

	/// <summary>
	/// Indicates the original sudoku puzzle to solve.
	/// </summary>
	Grid OriginalPuzzle { get; init; }

	/// <summary>
	/// Indicates the result sudoku grid solved. If the solver can't solve this puzzle, the value will be
	/// <see cref="Grid.Undefined"/>.
	/// </summary>
	/// <seealso cref="Grid.Undefined"/>
	Grid Solution { get; init; }

	/// <summary>
	/// Indicates the elapsed time used during solving the puzzle. The value may not be an useful value.
	/// Some case if the puzzle doesn't contain a valid unique solution, the value may be
	/// <see cref="TimeSpan.Zero"/>.
	/// </summary>
	/// <seealso cref="TimeSpan.Zero"/>
	TimeSpan ElapsedTime { get; init; }


	/// <summary>
	/// Get the string representation of the current instance.
	/// </summary>
	/// <returns>The string representation of the current instance.</returns>
	string ToDisplayString();
}
