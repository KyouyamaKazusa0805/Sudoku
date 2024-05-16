namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Take</c>, <c>TakeLast</c> and <c>TakeWhile</c>.
/// </summary>
/// <inheritdoc/>
public interface ITakeProvider<TSelf, TSource> : ILinqMethodProvider<TSelf, TSource>
	where TSelf : ITakeProvider<TSelf, TSource>
{
	/// <inheritdoc cref="Enumerable.Take{TSource}(IEnumerable{TSource}, int)"/>
	public virtual IEnumerable<TSource> Take(int count) => new List<TSource>(this)[..count];

	/// <inheritdoc cref="Enumerable.Take{TSource}(IEnumerable{TSource}, Range)"/>
	public virtual IEnumerable<TSource> Take(Range range) => new List<TSource>(this)[range];

	/// <inheritdoc cref="Enumerable.TakeLast{TSource}(IEnumerable{TSource}, int)"/>
	public virtual IEnumerable<TSource> TakeLast(int count) => new List<TSource>(this)[^count..];

	/// <inheritdoc cref="Enumerable.TakeWhile{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	public virtual IEnumerable<TSource> TakeWhile(Func<TSource, bool> predicate)
	{
		var result = new List<TSource>();
		foreach (var element in this)
		{
			if (predicate(element))
			{
				result.Add(element);
			}
			break;
		}
		return result;
	}

	/// <inheritdoc cref="Enumerable.TakeWhile{TSource}(IEnumerable{TSource}, Func{TSource, int, bool})"/>
	public virtual IEnumerable<TSource> TakeWhile(Func<TSource, int, bool> predicate)
	{
		var result = new List<TSource>();
		var i = 0;
		foreach (var element in this)
		{
			if (predicate(element, i++))
			{
				result.Add(element);
			}
			break;
		}
		return result;
	}
}
