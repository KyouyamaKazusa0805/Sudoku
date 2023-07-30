namespace Sudoku.Text;

/// <summary>
/// Represents a type that defines for various of sudoku notation, with option provider.
/// </summary>
/// <typeparam name="TSelf">
/// <inheritdoc cref="ISudokuNotation{TSelf, TElement, TConceptKindPresenter}" path="/typeparam[@name='TSelf']"/>
/// </typeparam>
/// <typeparam name="TCollection">
/// <inheritdoc cref="ISudokuNotation{TSelf, TCollection, TElement, TConceptKindPresenter}" path="/typeparam[@name='TCollection']"/>
/// </typeparam>
/// <typeparam name="TElement">
/// The type of the element after or before parsing, stored in the collection of type <typeparamref name="TCollection"/>.
/// </typeparam>
/// <typeparam name="TConceptKindPresenter">
/// <inheritdoc cref="ISudokuNotation{TSelf, TElement, TConceptKindPresenter}" path="/typeparam[@name='TConceptKindPresenter']"/>
/// </typeparam>
/// <typeparam name="TOptionProvider">
/// The type of the option provider. The type should be a <see langword="record struct"/> or a normal <see langword="struct"/>
/// in order to allow using <see langword="with"/> expressions.
/// </typeparam>
public interface ISudokuNotation<TSelf, TCollection, TElement, TConceptKindPresenter, TOptionProvider> :
	ISudokuNotation<TSelf, TCollection, TElement, TConceptKindPresenter>
	where TSelf : notnull, ISudokuNotation<TSelf, TElement, TConceptKindPresenter>
	where TCollection : notnull, IEnumerable<TElement>, ISimpleParsable<TCollection>
	where TElement : unmanaged, IBinaryInteger<TElement>
	where TConceptKindPresenter : unmanaged, Enum
	where TOptionProvider : struct, IOptionProvider<TOptionProvider, TCollection, TElement, TConceptKindPresenter>;
