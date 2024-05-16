namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Index</c>.
/// </summary>
/// <inheritdoc/>
public interface IIndexProvider<TSelf, TSource> : ILinqMethodProvider<TSelf, TSource>
	where TSelf : IIndexProvider<TSelf, TSource>
{
	/// <inheritdoc/>
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
