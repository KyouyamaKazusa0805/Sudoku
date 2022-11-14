namespace Sudoku.Solving.Logical.Patterns;

/// <summary>
/// Describes a technique pattern that can be gather-able.
/// </summary>
/// <typeparam name="T">The type of the technique pattern.</typeparam>
public interface ITechniquePatternGatherable<T> where T : IEquatable<T>, ITechniquePattern<T>
{
	/// <summary>
	/// Gathers all possible <typeparamref name="T"/>s in the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>All possible found <typeparamref name="T"/>.</returns>
	static abstract T[] Gather(scoped in Grid grid);
}
