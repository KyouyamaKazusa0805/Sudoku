namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>OrderBy</c> and <c>OrderByDescending</c>.
/// </summary>
/// <inheritdoc/>
public interface IOrderByMethod<TSelf, TSource> : IQueryExpressionMethod<TSelf, TSource>
	where TSelf : IOrderByMethod<TSelf, TSource>, allows ref struct
{
	/// <inheritdoc/>
	static bool ILinqMethod<TSelf, TSource>.IsValueLazilyCalculated => true;


	/// <inheritdoc cref="Enumerable.OrderBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
	public virtual IEnumerable<TSource> OrderBy<TKey>(Func<TSource, TKey> keySelector) where TKey : notnull
		=> OrderBy(keySelector, null);

	/// <inheritdoc cref="Enumerable.OrderBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey}, IComparer{TKey}?)"/>
	public virtual IEnumerable<TSource> OrderBy<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
		where TKey : notnull
	{
		comparer ??= Comparer<TKey>.Default;
		var result = new SortedList<TKey, TSource>(comparer);
		foreach (var element in this)
		{
			result.Add(keySelector(element), element);
		}
		return result.Values;
	}

	/// <inheritdoc cref="Enumerable.OrderByDescending{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
	public virtual IEnumerable<TSource> OrderByDescending<TKey>(Func<TSource, TKey> keySelector) where TKey : notnull
		=> OrderByDescending(keySelector, null);

	/// <inheritdoc cref="Enumerable.OrderByDescending{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey}, IComparer{TKey}?)"/>
	public virtual IEnumerable<TSource> OrderByDescending<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
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
