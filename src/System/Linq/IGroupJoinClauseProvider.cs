namespace System.Linq;

/// <summary>
/// Defines a type that supports
/// <see langword="join"/>-<see langword="in"/>-<see langword="on"/>-<see langword="equals"/>-<see langword="into"/>
/// clauses.
/// </summary>
/// <typeparam name="T">The type of the element.</typeparam>
public interface IGroupJoinClauseProvider<T> : ILinqProvider<T>
{
	/// <summary>
	/// Correlates the elements of two sequences based on equality of keys and groups the results.
	/// The default equality comparer is used to compare keys.
	/// </summary>
	/// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
	/// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
	/// <typeparam name="TResult">The type of the result elements.</typeparam>
	/// <param name="inner">The sequence to join to the first sequence.</param>
	/// <param name="outerKeySelector">
	/// A function to extract the join key from each element of the first sequence.
	/// </param>
	/// <param name="innerKeySelector">
	/// A function to extract the join key from each element of the second sequence.
	/// </param>
	/// <param name="resultSelector">
	/// A function to create a result element from an element from the first sequence
	/// and a collection of matching elements from the second sequence.
	/// </param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> that has elements of type TResult that are obtained
	/// by performing an inner join on two sequences.
	/// </returns>
	IEnumerable<TResult> GroupJoin<TInner, TKey, TResult>(
		IEnumerable<TInner> inner,
		Func<T, TKey> outerKeySelector,
		Func<TInner, TKey> innerKeySelector,
		Func<T, IEnumerable<TInner>, TResult> resultSelector);

	/// <inheritdoc cref="GroupJoin{TInner, TKey, TResult}(IEnumerable{TInner}, Func{T, TKey}, Func{TInner, TKey}, Func{T, IEnumerable{TInner}, TResult})"/>
	sealed unsafe IEnumerable<TResult> GroupJoinUnsafe<TInner, TKey, TResult>(
		IEnumerable<TInner> inner,
		delegate*<T, TKey> outerKeySelector,
		delegate*<TInner, TKey> innerKeySelector,
		delegate*<T, IEnumerable<TInner>, TResult> resultSelector)
		=> GroupJoin(inner, e => outerKeySelector(e), e => innerKeySelector(e), (outer, inner) => resultSelector(outer, inner));
}
