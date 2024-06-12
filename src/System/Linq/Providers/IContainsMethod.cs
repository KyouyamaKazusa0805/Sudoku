namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Contains</c>.
/// </summary>
/// <inheritdoc/>
public interface IContainsMethod<TSelf, TSource> : ILinqMethod<TSelf, TSource>
	where TSelf :
		IContainsMethod<TSelf, TSource>
#if NET9_0_OR_GREATER
		,
		allows ref struct
#endif
	where TSource : notnull
{
	/// <inheritdoc cref="Enumerable.Contains{TSource}(IEnumerable{TSource}, TSource)"/>
	public virtual bool Contains(TSource value) => Contains(value, null);

	/// <inheritdoc cref="Enumerable.Contains{TSource}(IEnumerable{TSource}, TSource, IEqualityComparer{TSource}?)"/>
	public virtual bool Contains(TSource value, IEqualityComparer<TSource>? comparer)
	{
		comparer ??= EqualityComparer<TSource>.Default;

		var valueHash = comparer.GetHashCode(value);
		foreach (var element in this)
		{
			var elementHash = comparer.GetHashCode(element);
			if (valueHash != elementHash)
			{
				continue;
			}

			if (!comparer.Equals(element, value))
			{
				continue;
			}

			return true;
		}
		return false;
	}
}
