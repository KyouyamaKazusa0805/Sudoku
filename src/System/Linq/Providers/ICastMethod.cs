namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Cast</c>.
/// </summary>
/// <inheritdoc/>
public interface ICastMethod<TSelf, TSource> : ILinqMethod<TSelf, TSource>
	where TSelf :
		ICastMethod<TSelf, TSource>
#if NET9_0_OR_GREATER
		,
		allows ref struct
#endif
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
