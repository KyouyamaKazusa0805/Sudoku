namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Concat</c>.
/// </summary>
/// <inheritdoc/>
public interface IConcatMethod<TSelf, TSource> : ILinqMethod<TSelf, TSource>
	where TSelf : IConcatMethod<TSelf, TSource>, allows ref struct
{
	/// <inheritdoc cref="Enumerable.Concat{TSource}(IEnumerable{TSource}, IEnumerable{TSource})"/>
	public virtual IEnumerable<TSource> Concat(IEnumerable<TSource> second) => [.. this, .. second];
}
