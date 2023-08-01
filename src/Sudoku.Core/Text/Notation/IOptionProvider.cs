namespace Sudoku.Text.Notation;

/// <summary>
/// Represents for an option provider instance.
/// </summary>
/// <typeparam name="TSelf">The type of the option provider itself.</typeparam>
/// <typeparam name="TCollection">
/// <inheritdoc cref="INotation{TSelf, TCollection, TElement, TConceptKindPresenter}" path="/typeparam[@name='TCollection']"/>
/// </typeparam>
/// <typeparam name="TElement">
/// <inheritdoc cref="INotation{TSelf, TCollection, TElement, TConceptKindPresenter}" path="/typeparam[@name='TElement']"/>
/// </typeparam>
/// <typeparam name="TConceptKindPresenter">
/// <inheritdoc cref="INotation{TSelf, TCollection, TElement, TConceptKindPresenter}" path="/typeparam[@name='TConceptKindPresenter']"/>
/// </typeparam>
public interface IOptionProvider<TSelf, TCollection, TElement, TConceptKindPresenter>
	where TSelf : struct, IOptionProvider<TSelf, TCollection, TElement, TConceptKindPresenter>
	where TCollection : notnull, IEnumerable<TElement>
	where TElement : notnull
	where TConceptKindPresenter : unmanaged, Enum;
