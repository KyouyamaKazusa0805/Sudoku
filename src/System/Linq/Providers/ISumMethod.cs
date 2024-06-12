namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Sum</c> and <c>SumBy</c>.
/// </summary>
/// <inheritdoc/>
public interface ISumMethod<TSelf, TSource> : ILinqMethod<TSelf, TSource>
	where TSelf :
		ISumMethod<TSelf, TSource>
#if NET9_0_OR_GREATER
		,
		allows ref struct
#endif
	where TSource : IAdditiveIdentity<TSource, TSource>, IAdditionOperators<TSource, TSource, TSource>
{
	/// <inheritdoc/>
	public virtual TSource Sum()
	{
		var result = TSource.AdditiveIdentity;
		foreach (var element in this)
		{
			result += element;
		}
		return result;
	}
}
