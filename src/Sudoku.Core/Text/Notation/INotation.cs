namespace Sudoku.Text.Notation;

/// <summary>
/// Represents a type that defines for a kind of notation for a concept in sudoku.
/// </summary>
public interface INotation;

/// <summary>
/// <inheritdoc cref="INotation" path="/summary"/>
/// </summary>
/// <typeparam name="TSelf">The type of the implementation.</typeparam>
/// <typeparam name="TElement">The type of the element after or before parsing.</typeparam>
public interface INotation<TSelf, TElement> : INotation where TSelf : notnull, INotation<TSelf, TElement> where TElement : notnull
{
	/// <summary>
	/// Gets the text notation that can represent the specified value.
	/// </summary>
	/// <param name="value">The instance to be output.</param>
	/// <returns>The string notation of the value.</returns>
	public static abstract string ToString(TElement value);
}

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
