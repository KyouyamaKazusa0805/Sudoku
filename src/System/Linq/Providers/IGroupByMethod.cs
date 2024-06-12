namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>GroupBy</c>.
/// </summary>
/// <inheritdoc/>
public interface IGroupByMethod<TSelf, TSource> : IQueryExpressionMethod<TSelf, TSource>
	where TSelf :
		IGroupByMethod<TSelf, TSource>
#if NET9_0_OR_GREATER
		,
		allows ref struct
#endif
{
	/// <inheritdoc cref="Enumerable.GroupBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
	public virtual IEnumerable<IGrouping<TKey, TSource>> GroupBy<TKey>(Func<TSource, TKey> keySelector) where TKey : notnull
		=> GroupBy(keySelector, null);

	/// <inheritdoc cref="Enumerable.GroupBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey}, IEqualityComparer{TKey}?)"/>
	public virtual IEnumerable<IGrouping<TKey, TSource>> GroupBy<TKey>(Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
		where TKey : notnull
		=> this.ToLookup(keySelector, comparer ?? EqualityComparer<TKey>.Default);

	/// <inheritdoc cref="Enumerable.GroupBy{TSource, TKey, TElement}(IEnumerable{TSource}, Func{TSource, TKey}, Func{TSource, TElement})"/>
	public virtual IEnumerable<IGrouping<TKey, TElement>> GroupBy<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		where TKey : notnull
		=> GroupBy(keySelector, elementSelector, null);

	/// <inheritdoc cref="Enumerable.GroupBy{TSource, TKey, TElement}(IEnumerable{TSource}, Func{TSource, TKey}, Func{TSource, TElement}, IEqualityComparer{TKey}?)"/>
	public virtual IEnumerable<IGrouping<TKey, TElement>> GroupBy<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey>? comparer)
		where TKey : notnull
		=> this.ToLookup(keySelector, elementSelector);

	/// <inheritdoc cref="Enumerable.GroupBy{TSource, TKey, TElement, TResult}(IEnumerable{TSource}, Func{TSource, TKey}, Func{TSource, TElement}, Func{TKey, IEnumerable{TElement}, TResult})"/>
	public virtual IEnumerable<TResult> GroupBy<TKey, TElement, TResult>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
		where TKey : notnull
		=> GroupBy(keySelector, elementSelector, resultSelector, null);

	/// <inheritdoc cref="Enumerable.GroupBy{TSource, TKey, TElement, TResult}(IEnumerable{TSource}, Func{TSource, TKey}, Func{TSource, TElement}, Func{TKey, IEnumerable{TElement}, TResult}, IEqualityComparer{TKey}?)"/>
	public virtual IEnumerable<TResult> GroupBy<TKey, TElement, TResult>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
		where TKey : notnull
		=> this.ToLookup(keySelector, elementSelector, comparer).Select(p => resultSelector(p.Key, p));
}
