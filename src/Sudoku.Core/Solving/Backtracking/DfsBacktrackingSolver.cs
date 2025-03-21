namespace Sudoku.Solving.Backtracking;

/// <summary>
/// Represents a backtracking solver.
/// </summary>
internal sealed class DfsBacktrackingSolver : BacktrackingSolver
{
	/// <summary>
	/// Initializes a <see cref="DfsBacktrackingSolver"/> instance.
	/// </summary>
	public DfsBacktrackingSolver() : base() => UseBreadthFirstSearch = false;
}
