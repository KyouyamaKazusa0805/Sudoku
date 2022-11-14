namespace System.Linq;

/// <summary>
/// Defines a type that supports <see langword="orderby"/> clauses:
/// <list type="bullet">
/// <item><see langword="orderby"/> instance</item>
/// <item><see langword="orderby"/> instance <see langword="ascending"/></item>
/// <item><see langword="orderby"/> instance <see langword="descending"/></item>
/// </list>
/// </summary>
/// <typeparam name="T">The type of the element.</typeparam>
public interface IOrderByClauseProvider<T> : ILinqProvider<T>
{
	/// <summary>
	/// Sorts the elements of a sequence in ascending order.
	/// </summary>
	/// <typeparam name="TKey">The type of the key being used for sorting.</typeparam>
	/// <param name="keySelector">The function that compares two instances of type <typeparamref name="TKey"/>.</param>
	/// <returns>The ordered result.</returns>
	IOrderedEnumerable<TKey> OrderBy<TKey>(Func<T, TKey> keySelector);

	/// <summary>
	/// Sorts the elements of a sequence in descending order.
	/// </summary>
	/// <typeparam name="TKey">The type of the key being used for sorting.</typeparam>
	/// <param name="keySelector">The function that compares two instances of type <typeparamref name="TKey"/>.</param>
	/// <returns>The ordered result.</returns>
	IOrderedEnumerable<TKey> OrderByDescending<TKey>(Func<T, TKey> keySelector);

	/// <summary>
	/// Performs a subsequent ordering of the elements in a sequence in ascending order.
	/// </summary>
	/// <typeparam name="TKey">The type of the key being used for sorting.</typeparam>
	/// <param name="keySelector">The function that compares two instances of type <typeparamref name="TKey"/>.</param>
	/// <returns>The ordered result.</returns>
	IOrderedEnumerable<TKey> ThenBy<TKey>(Func<T, TKey> keySelector);

	/// <summary>
	/// Performs a subsequent ordering of the elements in a sequence in descending order.
	/// </summary>
	/// <typeparam name="TKey">The type of the key being used for sorting.</typeparam>
	/// <param name="keySelector">The function that compares two instances of type <typeparamref name="TKey"/>.</param>
	/// <returns>The ordered result.</returns>
	IOrderedEnumerable<TKey> ThenByDescending<TKey>(Func<T, TKey> keySelector);

	/// <inheritdoc cref="OrderBy{TKey}(Func{T, TKey})"/>
	sealed unsafe IOrderedEnumerable<TKey> OrderByUnsafe<TKey>(delegate*<T, TKey> keySelector) => OrderBy(e => keySelector(e));

	/// <inheritdoc cref="OrderByDescending{TKey}(Func{T, TKey})"/>
	sealed unsafe IOrderedEnumerable<TKey> OrderByDescendingUnsafe<TKey>(delegate*<T, TKey> keySelector)
		=> OrderByDescending(e => keySelector(e));

	/// <inheritdoc cref="ThenBy{TKey}(Func{T, TKey})"/>
	sealed unsafe IOrderedEnumerable<TKey> ThenByUnsafe<TKey>(delegate*<T, TKey> keySelector) => ThenBy(e => keySelector(e));

	/// <inheritdoc cref="ThenByDescending{TKey}(Func{T, TKey})"/>
	sealed unsafe IOrderedEnumerable<TKey> ThenByDescendingUnsafe<TKey>(delegate*<T, TKey> keySelector)
		=> ThenByDescending(e => keySelector(e));
}
