namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Contains</c>.
/// </summary>
/// <inheritdoc/>
public interface IContainsProvider<TSelf, TSource> : ILinqMethodProvider<TSelf, TSource>
	where TSelf : IContainsProvider<TSelf, TSource>
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
