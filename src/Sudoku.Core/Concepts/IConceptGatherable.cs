namespace Sudoku.Concepts;

/// <summary>
/// Provides with a gatherer that gathers all possible structures of a concept existing in a grid.
/// </summary>
/// <typeparam name="TResult">
/// The type of the concept. For example, if you want to find all possible <see cref="Conjugate"/> instances
/// in a grid, the type argument will be <see cref="Conjugate"/>.
/// </typeparam>
public interface IConceptGatherable<TResult>
{
	/// <summary>
	/// Gathers all possible structures from a sudoku grid.
	/// </summary>
	/// <param name="grid">The grid used.</param>
	/// <returns>The results found.</returns>
	public static abstract ICollection<TResult>?[] Gather(scoped in Grid grid);
}
