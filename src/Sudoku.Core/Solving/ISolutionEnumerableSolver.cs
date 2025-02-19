namespace Sudoku.Solving;

/// <summary>
/// Represents a solver that can find all possible solutions with detection .
/// </summary>
/// <typeparam name="TSelf"><include file="../../global-doc-comments.xml" path="/g/self-type-constraint"/></typeparam>
/// <typeparam name="TInput">
/// The type of input. Generally the type to this parameter is <see cref="Grid"/> or <see cref="string"/>.
/// </typeparam>
public interface ISolutionEnumerableSolver<TSelf, TInput> where TSelf : ISolutionEnumerableSolver<TSelf, TInput>
{
	/// <summary>
	/// Provide a way to detect event to be triggered when a solution is found;
	/// no matter whether the puzzle has a unique solution or not (multiple solutions).
	/// </summary>
	public abstract event SolverSolutionFoundEventHandler<TSelf, TInput>? SolutionFound;


	/// <summary>
	/// Try to perform enumeration on solutions.
	/// </summary>
	/// <param name="grid">The grid to be solved.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current operation.</param>
	public abstract void EnumerateSolutionsCore(TInput grid, CancellationToken cancellationToken);
}
