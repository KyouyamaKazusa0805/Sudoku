namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Cast</c>.
/// </summary>
/// <inheritdoc/>
public interface ICastMethod<TSelf, TSource> : ILinqMethod<TSelf, TSource>
	where TSelf : ICastMethod<TSelf, TSource>, allows ref struct
{
	/// <inheritdoc cref="Enumerable.Cast{TResult}(IEnumerable)"/>
	public virtual IEnumerable<TResult> Cast<TResult>() where TResult : TSource
	{
		var result = new List<TResult>();
		foreach (var element in this)
		{
			result.Add((TResult)element!);
		}
		return result;
	}
}
