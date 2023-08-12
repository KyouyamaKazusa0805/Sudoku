namespace Sudoku.Text.Notation;

/// <summary>
/// <inheritdoc cref="INotation" path="/summary"/>
/// </summary>
/// <typeparam name="TSelf"><inheritdoc cref="INotation{TSelf, TElement}" path="/typeparam[@name='TSelf']"/></typeparam>
/// <typeparam name="TElement"><inheritdoc cref="INotation{TSelf, TElement}" path="/typeparam[@name='TElement']"/></typeparam>
/// <typeparam name="TConceptKindPresenter">
/// The type of the concept kind presenter. This type provides with an enumeration field representing a kind of notation behavior.
/// </typeparam>
public interface INotation<TSelf, TElement, TConceptKindPresenter> : INotation
	where TSelf : notnull, INotation<TSelf, TElement, TConceptKindPresenter>
	where TElement : notnull
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
	public static abstract TElement Parse(string text, TConceptKindPresenter notation);

	/// <summary>
	/// Gets the text notation that can represent the specified value via the specified notation kind.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="notation">
	/// <inheritdoc cref="Parse(string, TConceptKindPresenter)" path="/param[@name='notation']"/>
	/// </param>
	/// <returns>The string representation of the value.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="notation"/> is not defined.</exception>
	public static abstract string ToString(TElement value, TConceptKindPresenter notation);
}
