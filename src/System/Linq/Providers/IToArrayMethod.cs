namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>ToArray</c>.
/// </summary>
/// <inheritdoc/>
public interface IToArrayMethod<TSelf, TSource> : ILinqMethod<TSelf, TSource>
	where TSelf :
		IToArrayMethod<TSelf, TSource>
#if NET9_0_OR_GREATER
		,
		allows ref struct
#endif
{
	/// <inheritdoc cref="Enumerable.ToArray{TSource}(IEnumerable{TSource})"/>
	public virtual TSource[] ToArray() => [.. this];
}
