namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Union</c>.
/// </summary>
/// <inheritdoc/>
public interface IUnionMethod<TSelf, TSource> : ILinqMethod<TSelf, TSource>
	where TSelf : IUnionMethod<TSelf, TSource>, allows ref struct
{
	/// <inheritdoc cref="Enumerable.Union{TSource}(IEnumerable{TSource}, IEnumerable{TSource})"/>
	public virtual IEnumerable<TSource> Union(IEnumerable<TSource> second) => Union(second, null);

	/// <inheritdoc cref="Enumerable.Union{TSource}(IEnumerable{TSource}, IEnumerable{TSource}, IEqualityComparer{TSource}?)"/>
	public virtual IEnumerable<TSource> Union(IEnumerable<TSource> second, IEqualityComparer<TSource>? comparer)
	{
		comparer ??= EqualityComparer<TSource>.Default;

		var set = new HashSet<TSource>(second, comparer);
		foreach (var element in this)
		{
			set.Add(element);
		}
		return set;
	}
}
