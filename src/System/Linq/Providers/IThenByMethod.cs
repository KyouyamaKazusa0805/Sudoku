namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>ThenBy</c> and <c>ThenByDescending</c>.
/// </summary>
/// <inheritdoc/>
public interface IThenByMethod<TSelf, TSource> : IQueryExpressionMethod<TSelf, TSource> where TSelf : IThenByMethod<TSelf, TSource>
{
	/// <inheritdoc cref="Enumerable.ThenBy{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey})"/>
	public virtual TSelf ThenBy<TKey>(Func<TSource, TKey> keySelector) where TKey : notnull
		=> ThenBy(keySelector, null);

	/// <inheritdoc cref="Enumerable.ThenBy{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey}, IComparer{TKey}?)"/>
	public virtual TSelf ThenBy<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
		where TKey : notnull
		=> throw new NotImplementedException();

	/// <inheritdoc cref="Enumerable.ThenByDescending{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey})"/>
	public virtual TSelf ThenByDescending<TKey>(Func<TSource, TKey> keySelector) where TKey : notnull
		=> ThenByDescending(keySelector, null);

	/// <inheritdoc cref="Enumerable.ThenByDescending{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey}, IComparer{TKey}?)"/>
	public virtual TSelf ThenByDescending<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
		where TKey : notnull
		=> throw new NotImplementedException();
}
