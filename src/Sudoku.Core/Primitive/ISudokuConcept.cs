namespace Sudoku.Primitive;

/// <summary>
/// Represents a concept defined in Sudoku.
/// </summary>
/// <typeparam name="TSelf">The type of itself.</typeparam>
public interface ISudokuConcept<TSelf> :
	IEqualityOperators<TSelf, TSelf, bool>,
	IEquatable<TSelf>,
	IFormattable,
	ISudokuConceptConvertible<TSelf>,
	ISudokuConceptParsable<TSelf>
	where TSelf : ISudokuConcept<TSelf>;
