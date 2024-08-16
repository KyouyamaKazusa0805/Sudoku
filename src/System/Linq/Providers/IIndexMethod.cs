namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Index</c>.
/// </summary>
/// <inheritdoc/>
public interface IIndexMethod<TSelf, TSource> : ILinqMethod<TSelf, TSource>
	where TSelf : IIndexMethod<TSelf, TSource>, allows ref struct
{
	/// <inheritdoc cref="Enumerable.Index{TSource}(IEnumerable{TSource})"/>
	public virtual IEnumerable<(int Index, TSource Item)> Index()
	{
		var i = 0;
		var result = new List<(int, TSource)>();
		foreach (var element in this)
		{
			result.Add((i++, element));
		}
		return result;
	}
}
