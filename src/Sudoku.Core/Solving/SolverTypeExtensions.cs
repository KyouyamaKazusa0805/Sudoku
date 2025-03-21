namespace Sudoku.Solving;

/// <summary>
/// Provides with extension methods on <see cref="SolverType"/>.
/// </summary>
/// <seealso cref="SolverType"/>
public static class SolverTypeExtensions
{
	/// <summary>
	/// Creates a <see cref="ISolver"/> instance via the specified type.
	/// </summary>
	/// <param name="this">The type.</param>
	/// <returns>The instance.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument is not defined.</exception>
	public static ISolver Create(this SolverType @this)
		=> @this switch
		{
			SolverType.Backtracking => new BacktrackingSolver(),
			SolverType.BfsBacktracking => new BfsBacktrackingSolver(),
			SolverType.DfsBacktracking => new DfsBacktrackingSolver(),
			SolverType.Bitwise => new BitwiseSolver(),
			SolverType.DancingLinks => new DancingLinksSolver(),
			SolverType.DictionaryQuery => new DictionaryQuerySolver(),
			SolverType.EnumerableQuery => new EnumerableQuerySolver(),
			_ => throw new ArgumentOutOfRangeException(nameof(@this))
		};
}
