namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Cast</c>.
/// </summary>
/// <inheritdoc/>
public interface ICastMethod<TSelf> : ILinqMethod<TSelf, object> where TSelf : ICastMethod<TSelf>
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
