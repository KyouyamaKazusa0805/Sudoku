namespace Sudoku.Solving;

/// <summary>
/// Defines a complex solver.
/// </summary>
/// <typeparam name="TSolverResult">The type of the target result.</typeparam>
public interface IComplexSolver<out TSolverResult>
{
	/// <summary>
	/// To solve the specified puzzle.
	/// </summary>
	/// <param name="puzzle">The puzzle to be solved.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
	/// <returns>The solver result that provides the information after solving.</returns>
	TSolverResult Solve(in Grid puzzle, CancellationToken cancellationToken = default);
}
