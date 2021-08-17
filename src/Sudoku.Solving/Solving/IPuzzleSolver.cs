namespace Sudoku.Solving;

/// <summary>
/// Defines and provides with a solver that used for solving a sudoku puzzle.
/// </summary>
public interface IPuzzleSolver
{
	/// <summary>
	/// To solve the specified puzzle.
	/// </summary>
	/// <param name="puzzle">The puzzle to be solved.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
	/// <returns>The solver result that provides the information after solving.</returns>
	ISolverResult Solve(in Grid puzzle, CancellationToken cancellationToken = default);

	/// <summary>
	/// To solve the specified puzzle asynchronously.
	/// </summary>
	/// <param name="puzzle">The puzzle to be solved.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
	/// <returns>
	/// A task that holds the operation to solve the puzzle, whose inner value is the solver result
	/// that provides the information after solving.
	/// </returns>
	ValueTask<ISolverResult> SolveAsync(in Grid puzzle, CancellationToken cancellationToken = default);
}
