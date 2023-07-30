namespace Sudoku.Text;

/// <summary>
/// Represents a type that defines for various of sudoku notation, with option provider.
/// </summary>
/// <typeparam name="TSelf">
/// <inheritdoc cref="ISudokuConceptNotation{TSelf, TElement, TConceptKindPresenter}" path="/typeparam[@name='TSelf']"/>
/// </typeparam>
/// <typeparam name="TCollection">
/// <inheritdoc cref="ISudokuConceptNotation{TSelf, TCollection, TElement, TConceptKindPresenter}" path="/typeparam[@name='TCollection']"/>
/// </typeparam>
/// <typeparam name="TElement">
/// The type of the element after or before parsing, stored in the collection of type <typeparamref name="TCollection"/>.
/// </typeparam>
/// <typeparam name="TConceptKindPresenter">
/// <inheritdoc cref="ISudokuConceptNotation{TSelf, TElement, TConceptKindPresenter}" path="/typeparam[@name='TConceptKindPresenter']"/>
/// </typeparam>
/// <typeparam name="TOptionProvider">
/// The type of the option provider. The type should be a <see langword="record struct"/> or a normal <see langword="struct"/>
/// in order to allow using <see langword="with"/> expressions.
/// </typeparam>
public interface ISudokuConceptNotation<TSelf, TCollection, TElement, TConceptKindPresenter, TOptionProvider> :
	ISudokuConceptNotation<TSelf, TCollection, TElement, TConceptKindPresenter>
	where TSelf : notnull, ISudokuConceptNotation<TSelf, TElement, TConceptKindPresenter>
	where TCollection : notnull, IEnumerable<TElement>, ISimpleParsable<TCollection>
	where TElement : unmanaged, IBinaryInteger<TElement>
	where TConceptKindPresenter : unmanaged, Enum
	where TOptionProvider : struct, IOptionProvider<TOptionProvider, TCollection, TElement, TConceptKindPresenter>
{
	/// <summary>
	/// <inheritdoc cref="ISudokuConceptNotation{TSelf, TElement, TConceptKindPresenter}.Parse(string, TConceptKindPresenter)" path="/summary"/>
	/// </summary>
	/// <param name="text">
	/// <inheritdoc
	///     cref="ISudokuConceptNotation{TSelf, TElement, TConceptKindPresenter}.Parse(string, TConceptKindPresenter)"
	///     path="/param[@name='text']"/>
	/// </param>
	/// <param name="notation">
	/// <inheritdoc
	///     cref="ISudokuConceptNotation{TSelf, TElement, TConceptKindPresenter}.Parse(string, TConceptKindPresenter)"
	///     path="/param[@name='notation']"/>
	/// </param>
	/// <param name="option">The options to be used as extra controls.</param>
	/// <returns>
	/// <inheritdoc
	///     cref="ISudokuConceptNotation{TSelf, TElement, TConceptKindPresenter}.Parse(string, TConceptKindPresenter)"
	///     path="/returns"/>
	/// </returns>
	static abstract TElement Parse(string text, TConceptKindPresenter notation, scoped in TOptionProvider option);

	/// <summary>
	/// <inheritdoc
	///     cref="ISudokuConceptNotation{TSelf, TCollection, TElement, TConceptKindPresenter}.ParseCollection(string, TConceptKindPresenter)"
	///     path="/summary"/>
	/// </summary>
	/// <param name="text">
	/// <inheritdoc
	///     cref="ISudokuConceptNotation{TSelf, TElement, TConceptKindPresenter}.Parse(string, TConceptKindPresenter)"
	///     path="/param[@name='text']"/>
	/// </param>
	/// <param name="notation">
	/// <inheritdoc
	///     cref="ISudokuConceptNotation{TSelf, TElement, TConceptKindPresenter}.Parse(string, TConceptKindPresenter)"
	///     path="/param[@name='notation']"/>
	/// </param>
	/// <param name="option">
	/// <inheritdoc
	///     cref="ISudokuConceptNotation{TSelf, TCollection, TElement, TConceptKindPresenter, TOptionProvider}.Parse(string, TConceptKindPresenter, in TOptionProvider)"
	///     path="/param[@name='option']"/>
	/// </param>
	/// <returns>
	/// <inheritdoc
	///     cref="ISudokuConceptNotation{TSelf, TElement, TConceptKindPresenter}.Parse(string, TConceptKindPresenter)"
	///     path="/returns"/>
	/// </returns>
	static abstract TCollection ParseCollection(string text, TConceptKindPresenter notation, scoped in TOptionProvider option);

	/// <summary>
	/// <inheritdoc
	///     cref="ISudokuConceptNotation{TSelf, TElement, TConceptKindPresenter}.ToString(TElement, TConceptKindPresenter)"
	///     path="/summary"/>
	/// </summary>
	/// <param name="value">
	/// <inheritdoc
	///     cref="ISudokuConceptNotation{TSelf, TElement, TConceptKindPresenter}.ToString(TElement, TConceptKindPresenter)"
	///     path="/param[@name='value']"/>
	/// </param>
	/// <param name="notation">
	/// <inheritdoc
	///     cref="ISudokuConceptNotation{TSelf, TElement, TConceptKindPresenter}.ToString(TElement, TConceptKindPresenter)"
	///     path="/param[@name='notation']"/>
	/// </param>
	/// <param name="option">
	/// <inheritdoc
	///     cref="ISudokuConceptNotation{TSelf, TCollection, TElement, TConceptKindPresenter, TOptionProvider}.Parse(string, TConceptKindPresenter, in TOptionProvider)"
	///     path="/param[@name='option']"/>
	/// </param>
	/// <returns>
	/// <inheritdoc
	///     cref="ISudokuConceptNotation{TSelf, TElement, TConceptKindPresenter}.Parse(string, TConceptKindPresenter)"
	///     path="/returns"/>
	/// </returns>
	static abstract string ToString(TElement value, TConceptKindPresenter notation, scoped in TOptionProvider option);

	/// <summary>
	/// <inheritdoc
	///     cref="ISudokuConceptNotation{TSelf, TCollection, TElement, TConceptKindPresenter}.ToCollectionString(in TCollection, TConceptKindPresenter)"
	///     path="/summary"/>
	/// </summary>
	/// <param name="collection">
	/// <inheritdoc
	///     cref="ISudokuConceptNotation{TSelf, TCollection, TElement, TConceptKindPresenter}.ToCollectionString(in TCollection, TConceptKindPresenter)"
	///     path="/param[@name='collection']"/>
	/// </param>
	/// <param name="notation">
	/// <inheritdoc
	///     cref="ISudokuConceptNotation{TSelf, TElement, TConceptKindPresenter}.ToString(TElement, TConceptKindPresenter)"
	///     path="/param[@name='notation']"/>
	/// </param>
	/// <param name="option">
	/// <inheritdoc
	///     cref="ISudokuConceptNotation{TSelf, TCollection, TElement, TConceptKindPresenter, TOptionProvider}.Parse(string, TConceptKindPresenter, in TOptionProvider)"
	///     path="/param[@name='option']"/>
	/// </param>
	/// <returns>
	/// <inheritdoc
	///     cref="ISudokuConceptNotation{TSelf, TElement, TConceptKindPresenter}.Parse(string, TConceptKindPresenter)"
	///     path="/returns"/>
	/// </returns>
	static abstract string ToCollectionString(scoped in TCollection collection, TConceptKindPresenter notation, scoped in TOptionProvider option);
}
