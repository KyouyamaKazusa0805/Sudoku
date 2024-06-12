namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Distinct</c>.
/// </summary>
/// <inheritdoc/>
public interface IDistinctMethod<TSelf, TSource> : ILinqMethod<TSelf, TSource>
	where TSelf : IDistinctMethod<TSelf, TSource>, allows ref struct
{
	/// <inheritdoc cref="Enumerable.Distinct{TSource}(IEnumerable{TSource})"/>
	public virtual IEnumerable<TSource> Distinct() => Distinct(EqualityComparer<TSource>.Default);

	/// <inheritdoc cref="Enumerable.Distinct{TSource}(IEnumerable{TSource}, IEqualityComparer{TSource}?)"/>
	public virtual IEnumerable<TSource> Distinct(IEqualityComparer<TSource>? comparer)
	{
		var hashSet = new HashSet<TSource>(comparer);
		foreach (var element in this)
		{
			hashSet.Add(element);
		}
		return hashSet;
	}
}
