namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Where</c>.
/// </summary>
/// <inheritdoc/>
public interface IWhereMethod<TSelf, TSource> : IQueryExpressionMethod<TSelf, TSource>
	where TSelf : IWhereMethod<TSelf, TSource>, allows ref struct
{
	/// <inheritdoc cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	public virtual IEnumerable<TSource> Where(Func<TSource, bool> predicate)
	{
		var result = new List<TSource>();
		foreach (var element in this)
		{
			if (predicate(element))
			{
				result.Add(element);
			}
		}
		return result;
	}

	/// <inheritdoc cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, int, bool})"/>
	public virtual IEnumerable<TSource> Where(Func<TSource, int, bool> predicate)
	{
		var result = new List<TSource>();
		var i = 0;
		foreach (var element in this)
		{
			if (predicate(element, i++))
			{
				result.Add(element);
			}
		}
		return result;
	}
}
