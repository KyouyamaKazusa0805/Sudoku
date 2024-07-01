namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>OfType</c>.
/// </summary>
/// <inheritdoc/>
public interface IOfTypeMethod<TSelf, TSource> : ILinqMethod<TSelf, TSource>
	where TSelf : IOfTypeMethod<TSelf, TSource>, allows ref struct
{
	/// <inheritdoc cref="Enumerable.OfType{TResult}(IEnumerable)"/>
	public virtual IEnumerable<TResult> OfType<TResult>() where TResult : TSource
	{
		var result = new List<TResult>();
		foreach (var element in this)
		{
			if (element is TResult value)
			{
				result.Add(value);
			}
		}
		return result;
	}
}
