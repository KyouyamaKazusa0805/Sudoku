namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Cast</c>.
/// </summary>
/// <inheritdoc/>
public interface ICastProvider<TSelf> : ILinqMethodProvider<TSelf, object> where TSelf : ICastProvider<TSelf>
{
	/// <inheritdoc cref="Enumerable.Cast{TResult}(IEnumerable)"/>
	public virtual IEnumerable<TSource> Cast<TSource>()
	{
		var result = new List<TSource>();
		foreach (var element in this)
		{
			result.Add((TSource)element);
		}
		return result;
	}
}
