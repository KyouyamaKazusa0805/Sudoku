namespace Sudoku.Text;

/// <summary>
/// Represents a type that defines for various of sudoku notation.
/// </summary>
/// <typeparam name="TSelf">The type of the implementation.</typeparam>
/// <typeparam name="TElement">The type of the element after or before parsing.</typeparam>
/// <typeparam name="TConceptKindPresenter">The type of the concept kind presenter.</typeparam>
public interface ISudokuNotation<TSelf, TElement, TConceptKindPresenter>
	where TSelf : notnull, ISudokuNotation<TSelf, TElement, TConceptKindPresenter>
	where TElement : unmanaged, IBinaryInteger<TElement>
	where TConceptKindPresenter : unmanaged, Enum
{
	/// <summary>
	/// Try to parse the specified text, converting it into the target cell value via the specified notation kind.
	/// </summary>
	/// <param name="text">The text to be parsed.</param>
	/// <param name="notation">The notation that limits the conversion rule of the notation.</param>
	/// <returns>The converted result.</returns>
	/// <exception cref="InvalidOperationException">Throws when the argument <paramref name="text"/> cannot be parsed.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="notation"/> is not defined.</exception>
	static abstract TElement Parse(string text, TConceptKindPresenter notation);

	/// <summary>
	/// Gets the text notation that can represent the specified value via the specified notation kind.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="notation">
	/// <inheritdoc cref="Parse(string, TConceptKindPresenter)" path="/param[@name='notation']"/>
	/// </param>
	/// <returns>The string representation of the value.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="notation"/> is not defined.</exception>
	static abstract string ToString(TElement value, TConceptKindPresenter notation);
}
