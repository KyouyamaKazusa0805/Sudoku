namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Intersect</c>.
/// </summary>
/// <inheritdoc/>
public interface IIntersectionMethod<TSelf, TSource> : ILinqMethod<TSelf, TSource>
	where TSelf :
		IIntersectionMethod<TSelf, TSource>
#if NET9_0_OR_GREATER
		,
		allows ref struct
#endif
{
	/// <inheritdoc cref="Enumerable.Intersect{TSource}(IEnumerable{TSource}, IEnumerable{TSource})"/>
	public virtual IEnumerable<TSource> Intersect(IEnumerable<TSource> second) => Intersect(second, null);

	/// <inheritdoc cref="Enumerable.Intersect{TSource}(IEnumerable{TSource}, IEnumerable{TSource}, IEqualityComparer{TSource}?)"/>
	public virtual IEnumerable<TSource> Intersect(IEnumerable<TSource> second, IEqualityComparer<TSource>? comparer)
	{
		comparer ??= EqualityComparer<TSource>.Default;

		var set = new HashSet<TSource>(second, comparer);
		var result = new List<TSource>();
		foreach (var element in this)
		{
			if (set.Remove(element))
			{
				result.Add(element);
			}
		}
		return result;
	}
}
