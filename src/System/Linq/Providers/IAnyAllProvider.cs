namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Any</c> and <c>All</c>.
/// </summary>
/// <inheritdoc/>
public interface IAnyAllProvider<TSelf, TSource> : ILinqMethodProvider<TSelf, TSource>
	where TSelf : IAnyAllProvider<TSelf, TSource>
{
	/// <inheritdoc cref="Enumerable.Any{TSource}(IEnumerable{TSource})"/>
	public virtual bool Any()
	{
		using var iterator = GetEnumerator();
		return iterator.MoveNext();
	}

	/// <inheritdoc cref="Enumerable.Any{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	public virtual bool Any(Func<TSource, bool> predicate)
	{
		foreach (var element in this)
		{
			if (predicate(element))
			{
				return true;
			}
		}
		return false;
	}

	/// <inheritdoc cref="Enumerable.All{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	public virtual bool All(Func<TSource, bool> predicate)
	{
		foreach (var element in this)
		{
			if (!predicate(element))
			{
				return false;
			}
		}
		return true;
	}
}
