namespace Sudoku.Text;

/// <summary>
/// Represents for an option provider instance.
/// </summary>
/// <typeparam name="TSelf">The type of the option provider itself.</typeparam>
/// <typeparam name="TCollection">
/// <inheritdoc cref="ISudokuConceptNotation{TSelf, TCollection, TElement, TConceptKindPresenter}" path="/typeparam[@name='TCollection']"/>
/// </typeparam>
/// <typeparam name="TElement">
/// <inheritdoc cref="ISudokuConceptNotation{TSelf, TCollection, TElement, TConceptKindPresenter}" path="/typeparam[@name='TElement']"/>
/// </typeparam>
/// <typeparam name="TConceptKindPresenter">
/// <inheritdoc cref="ISudokuConceptNotation{TSelf, TCollection, TElement, TConceptKindPresenter}" path="/typeparam[@name='TConceptKindPresenter']"/>
/// </typeparam>
public interface IOptionProvider<TSelf, TCollection, TElement, TConceptKindPresenter>
	where TSelf : struct, IOptionProvider<TSelf, TCollection, TElement, TConceptKindPresenter>
	where TCollection : notnull, IEnumerable<TElement>
	where TElement : unmanaged, IBinaryInteger<TElement>
	where TConceptKindPresenter : unmanaged, Enum;
