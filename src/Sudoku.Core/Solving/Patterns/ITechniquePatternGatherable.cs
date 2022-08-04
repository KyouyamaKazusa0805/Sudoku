namespace Sudoku.Solving.Patterns;

/// <summary>
/// Describes a technique pattern that can be gather-able.
/// </summary>
/// <typeparam name="TTechniquePattern">The type of the technique pattern.</typeparam>
public interface ITechniquePatternGatherable<TTechniquePattern>
	where TTechniquePattern : IEquatable<TTechniquePattern>, ITechniquePattern<TTechniquePattern>
{
	/// <summary>
	/// Gathers all possible <typeparamref name="TTechniquePattern"/>s in the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>All possible found <typeparamref name="TTechniquePattern"/>.</returns>
	public static abstract TTechniquePattern[] Gather(scoped in Grid grid);
}
