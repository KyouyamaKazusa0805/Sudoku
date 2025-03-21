namespace Sudoku.Solving.Backtracking;

/// <summary>
/// Represents a backtracking solver.
/// </summary>
internal sealed class BfsBacktrackingSolver : BacktrackingSolver
{
	/// <summary>
	/// Initializes a <see cref="BfsBacktrackingSolver"/> instance.
	/// </summary>
	public BfsBacktrackingSolver() : base() => UseBreadthFirstSearch = true;
}
