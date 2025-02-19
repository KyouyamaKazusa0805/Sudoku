namespace Sudoku.Solving.Bitwise;

/// <summary>
/// Provides extra data for event <see cref="BitwiseSolver.SolutionFound"/>.
/// </summary>
/// <param name="solutionString">The solution string.</param>
/// <seealso cref="BitwiseSolver.SolutionFound"/>
public sealed partial class BitwiseSolverSolutionFoundEventArgs([Property] string solutionString) : EventArgs
{
	/// <summary>
	/// Indicates the solution.
	/// </summary>
	public Grid Solution => Grid.Parse(solutionString);
}
