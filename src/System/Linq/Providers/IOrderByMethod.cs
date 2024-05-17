namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>OrderBy</c> and <c>OrderByDescending</c>.
/// </summary>
/// <inheritdoc/>
public interface IOrderByMethod<TSelf, TSource> : IQueryExpressionMethod<TSelf, TSource> where TSelf : IOrderByMethod<TSelf, TSource>
{
	/// <inheritdoc cref="Enumerable.OrderBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
	public virtual IOrderedEnumerable<TSource> OrderBy<TKey>(Func<TSource, TKey> keySelector) where TKey : notnull
		=> OrderBy(keySelector, null);

	/// <inheritdoc cref="Enumerable.OrderBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey}, IComparer{TKey}?)"/>
	public virtual IOrderedEnumerable<TSource> OrderBy<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
		where TKey : notnull
		=> throw new NotImplementedException();

	/// <inheritdoc cref="Enumerable.OrderByDescending{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
	public virtual IOrderedEnumerable<TSource> OrderByDescending<TKey>(Func<TSource, TKey> keySelector) where TKey : notnull
		=> OrderByDescending(keySelector, null);

	/// <inheritdoc cref="Enumerable.OrderByDescending{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey}, IComparer{TKey}?)"/>
	public virtual IOrderedEnumerable<TSource> OrderByDescending<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
		where TKey : notnull
		=> throw new NotImplementedException();
}
