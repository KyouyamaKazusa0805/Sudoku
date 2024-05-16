namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Count</c> and <c>LongCount</c>.
/// </summary>
/// <inheritdoc/>
public interface ICountProvider<TSelf, TSource> : ILinqMethodProvider<TSelf, TSource>
	where TSelf : ICountProvider<TSelf, TSource>
{
	/// <inheritdoc cref="Enumerable.TryGetNonEnumeratedCount{TSource}(IEnumerable{TSource}, out int)"/>
	public virtual bool TryGetNonEnumeratedCount(out int count)
	{
		switch (this)
		{
			case ICollection<TSource> collection:
			{
				count = collection.Count;
				return true;
			}
			case ICollection collection:
			{
				count = collection.Count;
				return true;
			}
			default:
			{
				count = 0;
				return false;
			}
		}
	}

	/// <inheritdoc cref="Enumerable.Count{TSource}(IEnumerable{TSource})"/>
	public virtual int Count()
	{
		var result = 0;
		foreach (var element in this)
		{
			result++;
		}
		return result;
	}

	/// <inheritdoc cref="Enumerable.Count{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	public virtual int Count(Func<TSource, bool> predicate)
	{
		var result = 0;
		foreach (var element in this)
		{
			if (predicate(element))
			{
				result++;
			}
		}
		return result;
	}

	/// <inheritdoc cref="Enumerable.LongCount{TSource}(IEnumerable{TSource})"/>
	public virtual long LongCount()
	{
		var result = 0L;
		foreach (var element in this)
		{
			result++;
		}
		return result;
	}

	/// <inheritdoc cref="Enumerable.LongCount{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	public virtual long LongCount(Func<TSource, bool> predicate)
	{
		var result = 0L;
		foreach (var element in this)
		{
			if (predicate(element))
			{
				result++;
			}
		}
		return result;
	}
}
