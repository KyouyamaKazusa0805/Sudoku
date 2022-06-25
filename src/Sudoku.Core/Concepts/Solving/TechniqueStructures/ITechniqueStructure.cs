namespace Sudoku.Concepts.Solving.TechniqueStructures;

/// <summary>
/// Defines a pattern that is a technique.
/// </summary>
/// <typeparam name="TTechniquePattern">The type of the technique pattern.</typeparam>
public interface ITechniquePattern</*[Self]*/ TTechniquePattern>
	where TTechniquePattern : IEquatable<TTechniquePattern>, ITechniquePattern<TTechniquePattern>
{
	/// <summary>
	/// Indicates the whole map of cells that the technique used.
	/// </summary>
	public abstract Cells Map { get; }
}
