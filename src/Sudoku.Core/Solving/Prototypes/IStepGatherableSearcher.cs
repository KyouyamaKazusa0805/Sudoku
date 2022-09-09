namespace Sudoku.Solving.Prototypes;

/// <summary>
/// Defines a special step searcher that can gather all possible steps of various techniques, in a step.
/// </summary>
/// <typeparam name="TElement">
/// The type of the element. The type must be derived from <see cref="IGrouping{TKey, TElement}"/>
/// in order to allow grouping the steps up by the technique kind.
/// </typeparam>
/// <typeparam name="TGroupingKey">
/// <para>
/// The key that is used for grouping different step kinds. Generally this type can be a <see cref="string"/>.
/// </para>
/// <para>
/// The reason why the base type is <see cref="IComparable{T}"/> rather than <see cref="IEquatable{T}"/>
/// is that the type may be used for sorting different kinds of steps by their own key.
/// Sorting operations generally rely on comparable objects.
/// </para>
/// </typeparam>
/// <typeparam name="TGroupingValue">
/// The value of the gathered steps. The type must be derived from the interface type <see cref="IStep"/>
/// and be a reference type.
/// </typeparam>
public interface IStepGatherableSearcher<out TElement, out TGroupingKey, out TGroupingValue>
	where TElement : IGrouping<TGroupingKey, TGroupingValue>
	where TGroupingKey : notnull, IComparable<TGroupingKey>
	where TGroupingValue : class, IStep
{
	/// <summary>
	/// Search for all possible steps in a grid.
	/// </summary>
	/// <param name="puzzle">The puzzle grid.</param>
	/// <param name="cancellationToken">The cancellation token used for canceling an operation.</param>
	/// <returns>The result grouped by technique names.</returns>
	/// <exception cref="OperationCanceledException">Throws when the operation is canceled.</exception>
	public abstract IEnumerable<TElement> Search(scoped in Grid puzzle, CancellationToken cancellationToken = default);
}
