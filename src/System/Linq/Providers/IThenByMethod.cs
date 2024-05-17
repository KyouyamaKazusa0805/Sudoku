namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>ThenBy</c> and <c>ThenByDescending</c>.
/// </summary>
/// <typeparam name="TSelf">The type of itself.</typeparam>
/// <typeparam name="TSource">The type of each element that the type supports for iteration.</typeparam>
/// <typeparam name="TThenBy">The type of instance that methods <c>ThenBy</c> and <c>ThenByDescending</c> will return.</typeparam>
/// <typeparam name="TThenBySource">The type of each element of collection type <typeparamref name="TThenBy"/>.</typeparam>
public interface IThenByMethod<TSelf, TSource, TThenBy, TThenBySource> : IQueryExpressionMethod<TSelf, TSource>
	where TSelf : IThenByMethod<TSelf, TSource, TThenBy, TThenBySource>
	where TThenBy : IThenByMethod<TSelf, TSource, TThenBy, TThenBySource>
{
	/// <inheritdoc cref="Enumerable.ThenBy{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey})"/>
	public virtual TThenBy ThenBy<TKey>(Func<TSource, TKey> keySelector) where TKey : notnull
		=> ThenBy(keySelector, null);

	/// <inheritdoc cref="Enumerable.ThenBy{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey}, IComparer{TKey}?)"/>
	public virtual TThenBy ThenBy<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
		where TKey : notnull
		=> throw new NotImplementedException();

	/// <inheritdoc cref="Enumerable.ThenByDescending{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey})"/>
	public virtual TThenBy ThenByDescending<TKey>(Func<TSource, TKey> keySelector) where TKey : notnull
		=> ThenByDescending(keySelector, null);

	/// <inheritdoc cref="Enumerable.ThenByDescending{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey}, IComparer{TKey}?)"/>
	public virtual TThenBy ThenByDescending<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
		where TKey : notnull
		=> throw new NotImplementedException();
}
