namespace Sudoku.Text;

/// <summary>
/// Represents a type that defines for various of sudoku notation, for collections.
/// </summary>
/// <typeparam name="TSelf">
/// <inheritdoc cref="ISudokuNotation{TSelf, TElement, TConceptKindPresenter}" path="/typeparam[@name='TSelf']"/>
/// </typeparam>
/// <typeparam name="TCollection">The type of the collection of elements of type <typeparamref name="TElement"/>.</typeparam>
/// <typeparam name="TElement">
/// The type of the element after or before parsing, stored in the collection of type <typeparamref name="TCollection"/>.
/// </typeparam>
/// <typeparam name="TConceptKindPresenter">
/// <inheritdoc cref="ISudokuNotation{TSelf, TElement, TConceptKindPresenter}" path="/typeparam[@name='TConceptKindPresenter']"/>
/// </typeparam>
public interface ISudokuNotation<TSelf, TCollection, TElement, TConceptKindPresenter> : ISudokuNotation<TSelf, TElement, TConceptKindPresenter>
	where TSelf : notnull, ISudokuNotation<TSelf, TElement, TConceptKindPresenter>
	where TCollection : notnull, IEnumerable<TElement>, ISimpleParsable<TCollection>
	where TElement : unmanaged, IBinaryInteger<TElement>
	where TConceptKindPresenter : unmanaged, Enum
{
	/// <summary>
	/// Try to parse the specified text using the specified kind of the notation rule,
	/// converting it into a collection of type <typeparamref name="TCollection"/>.
	/// </summary>
	/// <param name="text">
	/// <inheritdoc
	///     cref="ISudokuNotation{TSelf, TElement, TConceptKindPresenter}.Parse(string, TConceptKindPresenter)"
	///     path="/param[@name='text']"/>
	/// </param>
	/// <param name="notation">The notation kind to be used.</param>
	/// <returns>
	/// <inheritdoc
	///     cref="ISudokuNotation{TSelf, TElement, TConceptKindPresenter}.Parse(string, TConceptKindPresenter)"
	///     path="/returns"/>
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="notation"/> is not defined.</exception>
	static abstract TCollection ParseCollection(string text, TConceptKindPresenter notation);

	/// <summary>
	/// Gets the text notation that can represent the specified collection via the specified notation kind.
	/// </summary>
	/// <param name="collection">
	/// <inheritdoc
	///     cref="ISudokuNotation{TSelf, TElement, TConceptKindPresenter}.ToString(TElement, TConceptKindPresenter)"
	///     path="/param[@name='value']"/>
	/// </param>
	/// <param name="notation">
	/// <inheritdoc
	///     cref="ISudokuNotation{TSelf, TElement, TConceptKindPresenter}.ToString(TElement, TConceptKindPresenter)"
	///     path="/param[@name='notation']"/>
	/// </param>
	/// <returns>
	/// <inheritdoc
	///     cref="ISudokuNotation{TSelf, TElement, TConceptKindPresenter}.ToString(TElement, TConceptKindPresenter)"
	///     path="/returns"/>
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="notation"/> is not defined.</exception>
	static abstract string ToCollectionString(scoped in TCollection collection, TConceptKindPresenter notation);
}
