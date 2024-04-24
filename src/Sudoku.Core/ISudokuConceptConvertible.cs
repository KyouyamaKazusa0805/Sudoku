namespace Sudoku;

/// <summary>
/// Represents an interface type that describes for an ability on <c>ToString</c> with custom sudoku coordinate logic.
/// </summary>
/// <typeparam name="TSelf">The type of itself.</typeparam>
public interface ISudokuConceptConvertible<TSelf> where TSelf : ISudokuConceptConvertible<TSelf>
{
	/// <inheritdoc cref="object.ToString"/>
	public abstract string ToString();

	/// <summary>
	/// Returns a string that represents the current object.
	/// </summary>
	/// <typeparam name="T">The type of the converter.</typeparam>
	/// <param name="converter">The converter instance that the current concept will rely on.</param>
	/// <returns>A string that represents the current object.</returns>
	public abstract string ToString<T>(T converter) where T : CoordinateConverter;
}
