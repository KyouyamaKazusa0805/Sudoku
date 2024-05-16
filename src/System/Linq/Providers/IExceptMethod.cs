namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Except</c>.
/// </summary>
/// <inheritdoc/>
public interface IExceptMethod<TSelf, TSource> : ILinqMethod<TSelf, TSource>
	where TSelf : IExceptMethod<TSelf, TSource>
{
	/// <inheritdoc cref="Enumerable.Except{TSource}(IEnumerable{TSource}, IEnumerable{TSource})"/>
	public virtual IEnumerable<TSource> Except(IEnumerable<TSource> second) => Except(second, null);

	/// <inheritdoc cref="Enumerable.Except{TSource}(IEnumerable{TSource}, IEnumerable{TSource}, IEqualityComparer{TSource}?)"/>
	public virtual IEnumerable<TSource> Except(IEnumerable<TSource> second, IEqualityComparer<TSource>? comparer)
	{
		comparer ??= EqualityComparer<TSource>.Default;

		var set = new HashSet<TSource>(second, comparer);
		var result = new List<TSource>();
		foreach (var element in this)
		{
			if (set.Add(element))
			{
				result.Add(element);
			}
		}
		return result;
	}
}
