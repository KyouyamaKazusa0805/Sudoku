namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>ThenBy</c> and <c>ThenByDescending</c>.
/// </summary>
/// <inheritdoc/>
public interface IThenByMethod<TSelf, TSource> : IQueryExpressionMethod<TSelf, TSource>
	where TSelf : IThenByMethod<TSelf, TSource>, allows ref struct
{
	/// <inheritdoc/>
	static bool ILinqMethod<TSelf, TSource>.IsValueLazilyCalculated => true;


	/// <inheritdoc cref="Enumerable.ThenBy{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey})"/>
	public virtual IEnumerable<TSource> ThenBy<TKey>(Func<TSource, TKey> keySelector) where TKey : notnull
		=> ThenBy(keySelector, null);

	/// <inheritdoc cref="Enumerable.ThenBy{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey}, IComparer{TKey}?)"/>
	public virtual IEnumerable<TSource> ThenBy<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey>? comparer) where TKey : notnull
	{
		// Implementation details:
		// The implementation here is same as 'OrderBy', because we cannot determine the internal comparison rule
		// like 'IOrderedEnumerable<T>'.
		// Such interface type uses a list of comparison rules to compare values,
		// calling methods one by one to check which value is larger.
		comparer ??= Comparer<TKey>.Default;
		var result = new SortedList<TKey, TSource>(comparer);
		foreach (var element in this)
		{
			result.Add(keySelector(element), element);
		}
		return result.Values;
	}

	/// <inheritdoc cref="Enumerable.ThenByDescending{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey})"/>
	public virtual IEnumerable<TSource> ThenByDescending<TKey>(Func<TSource, TKey> keySelector) where TKey : notnull
		=> ThenByDescending(keySelector, null);

	/// <inheritdoc cref="Enumerable.ThenByDescending{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey}, IComparer{TKey}?)"/>
	public virtual IEnumerable<TSource> ThenByDescending<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
		where TKey : notnull
	{
		comparer ??= Comparer<TKey>.Default;
		var result = new SortedList<TKey, TSource>(Comparer<TKey>.Create((l, r) => -comparer.Compare(l, r)));
		foreach (var element in this)
		{
			result.Add(keySelector(element), element);
		}
		return result.Values;
	}
}
