namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>OrderBy</c> and <c>OrderByDescending</c>.
/// </summary>
/// <typeparam name="TSelf">The type of itself.</typeparam>
/// <typeparam name="TSource">The type of each element that the type supports for iteration.</typeparam>
/// <typeparam name="TThenBy">The type of instance that methods <c>OrderBy</c> and <c>OrderByDescending</c> will return.</typeparam>
/// <typeparam name="TThenBySource">The type of each element of collection type <typeparamref name="TThenBy"/>.</typeparam>
public interface IOrderByMethod<TSelf, TSource, TThenBy, TThenBySource> : IQueryExpressionMethod<TSelf, TSource>
	where TSelf : IOrderByMethod<TSelf, TSource, TThenBy, TThenBySource>
	where TThenBy : IThenByMethod<TThenBy, TThenBySource, TThenBy, TThenBySource>
{
	/// <inheritdoc cref="Enumerable.OrderBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
	public virtual TThenBy OrderBy<TKey>(Func<TSource, TKey> keySelector) where TKey : notnull
		=> OrderBy(keySelector, null);

	/// <inheritdoc cref="Enumerable.OrderBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey}, IComparer{TKey}?)"/>
	public virtual TThenBy OrderBy<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey>? comparer) where TKey : notnull
		=> throw new NotImplementedException();

	/// <inheritdoc cref="Enumerable.OrderByDescending{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
	public virtual TThenBy OrderByDescending<TKey>(Func<TSource, TKey> keySelector) where TKey : notnull
		=> OrderByDescending(keySelector, null);

	/// <inheritdoc cref="Enumerable.OrderByDescending{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey}, IComparer{TKey}?)"/>
	public virtual TThenBy OrderByDescending<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey>? comparer) where TKey : notnull
		=> throw new NotImplementedException();
}
