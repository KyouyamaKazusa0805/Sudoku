namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>ToArray</c>.
/// </summary>
/// <inheritdoc/>
public interface IToArrayMethod<TSelf, TSource> : ILinqMethod<TSelf, TSource> where TSelf : IToArrayMethod<TSelf, TSource>
{
	/// <inheritdoc cref="Enumerable.ToArray{TSource}(IEnumerable{TSource})"/>
	public virtual TSource[] ToArray() => [.. this];
}
