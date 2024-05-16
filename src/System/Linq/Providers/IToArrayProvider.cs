namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>ToArray</c>.
/// </summary>
/// <inheritdoc/>
public interface IToArrayProvider<TSelf, TSource> : ILinqMethodProvider<TSelf, TSource>
	where TSelf : IToArrayProvider<TSelf, TSource>
{
	/// <inheritdoc cref="Enumerable.ToArray{TSource}(IEnumerable{TSource})"/>
	public virtual TSource[] ToArray() => [.. this];
}
