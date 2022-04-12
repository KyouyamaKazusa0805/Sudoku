namespace Sudoku.Concepts.Solving;

/// <summary>
/// Defines a pattern that is a technique.
/// </summary>
/// <typeparam name="T">The type of the technique pattern.</typeparam>
public interface ITechniquePattern</*[Self]*/ T> where T : IEquatable<T>, ITechniquePattern<T>
{
	/// <summary>
	/// Indicates the whole map of cells that the technique used.
	/// </summary>
	Cells Map { get; }
}
