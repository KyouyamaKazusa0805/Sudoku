namespace Sudoku.Primitive;

/// <summary>
/// Represents a concept defined in Sudoku.
/// </summary>
/// <typeparam name="TSelf">The type of itself.</typeparam>
public interface ISudokuConcept<TSelf> :
	ICultureFormattable,
	IEqualityOperators<TSelf, TSelf, bool>,
	IEquatable<TSelf>,
	ISudokuConceptConvertible<TSelf>,
	ISudokuConceptParsable<TSelf>
	where TSelf : ISudokuConcept<TSelf>;
