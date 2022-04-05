namespace Sudoku.Concepts.Solving;

/// <summary>
/// Defines a pattern that is a technique.
/// </summary>
/// <typeparam name="TSelf">The type of the technique pattern.</typeparam>
public interface ITechniquePattern</*[Self]*/ TSelf> where TSelf : IEquatable<TSelf>, ITechniquePattern<TSelf>
{
	/// <summary>
	/// Indicates the whole map of cells that the technique used.
	/// </summary>
	Cells Map { get; }
}
