namespace Sudoku.Solving.Logical.Patterns;

/// <summary>
/// Defines a pattern that is a technique.
/// </summary>
/// <typeparam name="T">The type of the technique pattern.</typeparam>
public interface ITechniquePattern<T> where T : IEquatable<T>, ITechniquePattern<T>
{
	/// <summary>
	/// Indicates the whole map of cells that the technique used.
	/// </summary>
	CellMap Map { get; }
}
