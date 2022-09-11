namespace Sudoku.Solving.Prototypes;

/// <summary>
/// Provides with a gatherer that gathers all possible structures existing in a grid.
/// </summary>
/// <typeparam name="TResult">The type of result elements.</typeparam>
public interface IStructureGatherer<TResult>
{
	/// <summary>
	/// Gathers all possible structures from a sudoku grid.
	/// </summary>
	/// <param name="grid">The grid used.</param>
	/// <returns>The results found.</returns>
	protected internal static abstract ICollection<TResult>?[] Gather(scoped in Grid grid);
}
