namespace Sudoku.Solving;

/// <summary>
/// Represents a solver type.
/// </summary>
public enum SolverType
{
	/// <summary>
	/// Indicates the backtracking solver.
	/// </summary>
	Backtracking,

	/// <summary>
	/// Indicates the backtracking solver, with BFS checking rule.
	/// </summary>
	BfsBacktracking,

	/// <summary>
	/// Indicates the backtracking solver, with DFS checking rule.
	/// </summary>
	DfsBacktracking,

	/// <summary>
	/// Indicates the bitwise solver.
	/// </summary>
	Bitwise,

	/// <summary>
	/// Indicates the dancing links solver.
	/// </summary>
	DancingLinks,

	/// <summary>
	/// Indicates the dictionary query solver.
	/// </summary>
	DictionaryQuery,

	/// <summary>
	/// Indicates the enumerable query solver.
	/// </summary>
	EnumerableQuery
}
