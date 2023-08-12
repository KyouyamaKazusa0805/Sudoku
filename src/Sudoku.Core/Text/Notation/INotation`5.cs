namespace Sudoku.Text.Notation;

/// <summary>
/// Represents a type that defines for a kind of notation for a concept in sudoku,
/// with adjustment by passing <typeparamref name="TOptionProvider"/> values.
/// </summary>
/// <typeparam name="TSelf">
/// <inheritdoc cref="INotation{TSelf, TElement, TConceptKindPresenter}" path="/typeparam[@name='TSelf']"/>
/// </typeparam>
/// <typeparam name="TCollection">
/// <inheritdoc cref="INotation{TSelf, TCollection, TElement, TConceptKindPresenter}" path="/typeparam[@name='TCollection']"/>
/// </typeparam>
/// <typeparam name="TElement">
/// The type of the element after or before parsing, stored in the collection of type <typeparamref name="TCollection"/>.
/// </typeparam>
/// <typeparam name="TConceptKindPresenter">
/// <inheritdoc cref="INotation{TSelf, TElement, TConceptKindPresenter}" path="/typeparam[@name='TConceptKindPresenter']"/>
/// </typeparam>
/// <typeparam name="TOptionProvider">
/// The type of the option provider. The type should be a <see langword="record struct"/> or a normal <see langword="struct"/>
/// in order to allow using <see langword="with"/> expressions.
/// </typeparam>
public interface INotation<TSelf, TCollection, TElement, TConceptKindPresenter, TOptionProvider> :
	INotation<TSelf, TCollection, TElement, TConceptKindPresenter>
	where TSelf : notnull, INotation<TSelf, TElement, TConceptKindPresenter>
	where TCollection : notnull, IEnumerable<TElement>, ISimpleParsable<TCollection>
	where TElement : notnull
	where TConceptKindPresenter : unmanaged, Enum
	where TOptionProvider : struct, IOptionProvider<TOptionProvider, TCollection, TElement, TConceptKindPresenter>
{
	/// <summary>
	/// <inheritdoc cref="INotation{TSelf, TElement, TConceptKindPresenter}.Parse(string, TConceptKindPresenter)" path="/summary"/>
	/// </summary>
	/// <param name="text">
	/// <inheritdoc cref="INotation{TSelf, TElement, TConceptKindPresenter}.Parse(string, TConceptKindPresenter)" path="/param[@name='text']"/>
	/// </param>
	/// <param name="notation">
	/// <inheritdoc
	///     cref="INotation{TSelf, TElement, TConceptKindPresenter}.Parse(string, TConceptKindPresenter)"
	///     path="/param[@name='notation']"/>
	/// </param>
	/// <param name="option">The options to be used as extra controls.</param>
	/// <returns>
	/// <inheritdoc cref="INotation{TSelf, TElement, TConceptKindPresenter}.Parse(string, TConceptKindPresenter)" path="/returns"/>
	/// </returns>
	public static abstract TElement Parse(string text, TConceptKindPresenter notation, TOptionProvider option);

	/// <summary>
	/// <inheritdoc
	///     cref="INotation{TSelf, TCollection, TElement, TConceptKindPresenter}.ParseCollection(string, TConceptKindPresenter)"
	///     path="/summary"/>
	/// </summary>
	/// <param name="text">
	/// <inheritdoc cref="INotation{TSelf, TElement, TConceptKindPresenter}.Parse(string, TConceptKindPresenter)" path="/param[@name='text']"/>
	/// </param>
	/// <param name="notation">
	/// <inheritdoc
	///     cref="INotation{TSelf, TElement, TConceptKindPresenter}.Parse(string, TConceptKindPresenter)"
	///     path="/param[@name='notation']"/>
	/// </param>
	/// <param name="option">
	/// <inheritdoc
	///     cref="INotation{TSelf, TCollection, TElement, TConceptKindPresenter, TOptionProvider}.Parse(string, TConceptKindPresenter, TOptionProvider)"
	///     path="/param[@name='option']"/>
	/// </param>
	/// <returns>
	/// <inheritdoc cref="INotation{TSelf, TElement, TConceptKindPresenter}.Parse(string, TConceptKindPresenter)" path="/returns"/>
	/// </returns>
	public static abstract TCollection ParseCollection(string text, TConceptKindPresenter notation, TOptionProvider option);

	/// <summary>
	/// <inheritdoc cref="INotation{TSelf, TElement, TConceptKindPresenter}.ToString(TElement, TConceptKindPresenter)" path="/summary"/>
	/// </summary>
	/// <param name="value">
	/// <inheritdoc cref="INotation{TSelf, TElement, TConceptKindPresenter}.ToString(TElement, TConceptKindPresenter)" path="/param[@name='value']"/>
	/// </param>
	/// <param name="notation">
	/// <inheritdoc
	///     cref="INotation{TSelf, TElement, TConceptKindPresenter}.ToString(TElement, TConceptKindPresenter)"
	///     path="/param[@name='notation']"/>
	/// </param>
	/// <param name="option">
	/// <inheritdoc
	///     cref="INotation{TSelf, TCollection, TElement, TConceptKindPresenter, TOptionProvider}.Parse(string, TConceptKindPresenter, TOptionProvider)"
	///     path="/param[@name='option']"/>
	/// </param>
	/// <returns>
	/// <inheritdoc cref="INotation{TSelf, TElement, TConceptKindPresenter}.Parse(string, TConceptKindPresenter)" path="/returns"/>
	/// </returns>
	public static abstract string ToString(TElement value, TConceptKindPresenter notation, TOptionProvider option);

	/// <summary>
	/// <inheritdoc
	///     cref="INotation{TSelf, TCollection, TElement, TConceptKindPresenter}.ToCollectionString(TCollection, TConceptKindPresenter)"
	///     path="/summary"/>
	/// </summary>
	/// <param name="collection">
	/// <inheritdoc
	///     cref="INotation{TSelf, TCollection, TElement, TConceptKindPresenter}.ToCollectionString(TCollection, TConceptKindPresenter)"
	///     path="/param[@name='collection']"/>
	/// </param>
	/// <param name="notation">
	/// <inheritdoc
	///     cref="INotation{TSelf, TElement, TConceptKindPresenter}.ToString(TElement, TConceptKindPresenter)"
	///     path="/param[@name='notation']"/>
	/// </param>
	/// <param name="option">
	/// <inheritdoc
	///     cref="INotation{TSelf, TCollection, TElement, TConceptKindPresenter, TOptionProvider}.Parse(string, TConceptKindPresenter, TOptionProvider)"
	///     path="/param[@name='option']"/>
	/// </param>
	/// <returns>
	/// <inheritdoc cref="INotation{TSelf, TElement, TConceptKindPresenter}.Parse(string, TConceptKindPresenter)" path="/returns"/>
	/// </returns>
	public static abstract string ToCollectionString(TCollection collection, TConceptKindPresenter notation, TOptionProvider option);
}
