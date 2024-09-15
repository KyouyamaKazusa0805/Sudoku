namespace Sudoku.Solving;

/// <summary>
/// Represents a solver that can find all possible solutions.
/// </summary>
public interface IMultipleSolutionSolver
{
	/// <summary>
	/// Find all possible solutions of the specified puzzle.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <returns>A sequence of solutions found.</returns>
	public abstract ReadOnlySpan<Grid> SolveAll(ref readonly Grid grid);
}
