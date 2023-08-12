namespace Sudoku.Text.Notation;

/// <summary>
/// Represents a type that defines for a kind of notation for a collection-based concept in sudoku.
/// </summary>
/// <typeparam name="TSelf">
/// <inheritdoc cref="INotation{TSelf, TElement, TConceptKindPresenter}" path="/typeparam[@name='TSelf']"/>
/// </typeparam>
/// <typeparam name="TCollection">The type of the collection of elements of type <typeparamref name="TElement"/>.</typeparam>
/// <typeparam name="TElement">
/// The type of the element after or before parsing, stored in the collection of type <typeparamref name="TCollection"/>.
/// </typeparam>
/// <typeparam name="TConceptKindPresenter">
/// <inheritdoc cref="INotation{TSelf, TElement, TConceptKindPresenter}" path="/typeparam[@name='TConceptKindPresenter']"/>
/// </typeparam>
public interface INotation<TSelf, TCollection, TElement, TConceptKindPresenter> : INotation<TSelf, TElement, TConceptKindPresenter>
	where TSelf : notnull, INotation<TSelf, TElement, TConceptKindPresenter>
	where TCollection : notnull, IEnumerable<TElement>
	where TElement : notnull
	where TConceptKindPresenter : unmanaged, Enum
{
	/// <summary>
	/// Try to parse the specified text using the specified kind of the notation rule,
	/// converting it into a collection of type <typeparamref name="TCollection"/>.
	/// </summary>
	/// <param name="text">
	/// <inheritdoc
	///     cref="INotation{TSelf, TElement, TConceptKindPresenter}.Parse(string, TConceptKindPresenter)"
	///     path="/param[@name='text']"/>
	/// </param>
	/// <param name="notation">The notation kind to be used.</param>
	/// <returns>
	/// <inheritdoc
	///     cref="INotation{TSelf, TElement, TConceptKindPresenter}.Parse(string, TConceptKindPresenter)"
	///     path="/returns"/>
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="notation"/> is not defined.</exception>
	public static abstract TCollection ParseCollection(string text, TConceptKindPresenter notation);

	/// <summary>
	/// Gets the text notation that can represent the specified collection via the specified notation kind.
	/// </summary>
	/// <param name="collection">
	/// <inheritdoc
	///     cref="INotation{TSelf, TElement, TConceptKindPresenter}.ToString(TElement, TConceptKindPresenter)"
	///     path="/param[@name='value']"/>
	/// </param>
	/// <param name="notation">
	/// <inheritdoc
	///     cref="INotation{TSelf, TElement, TConceptKindPresenter}.ToString(TElement, TConceptKindPresenter)"
	///     path="/param[@name='notation']"/>
	/// </param>
	/// <returns>
	/// <inheritdoc
	///     cref="INotation{TSelf, TElement, TConceptKindPresenter}.ToString(TElement, TConceptKindPresenter)"
	///     path="/returns"/>
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="notation"/> is not defined.</exception>
	public static abstract string ToCollectionString(TCollection collection, TConceptKindPresenter notation);
}
