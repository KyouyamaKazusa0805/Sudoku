namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>DefaultIfEmpty</c>.
/// </summary>
/// <inheritdoc/>
public interface IDefaultIfEmptyMethod<TSelf, TSource> : IAnyAllMethod<TSelf, TSource>, ILinqMethod<TSelf, TSource>
	where TSelf : IDefaultIfEmptyMethod<TSelf, TSource>, allows ref struct
{
	/// <inheritdoc cref="Enumerable.DefaultIfEmpty{TSource}(IEnumerable{TSource})"/>
	public virtual IEnumerable<TSource?> DefaultIfEmpty() => Any() ? this : [default];

	/// <inheritdoc cref="Enumerable.DefaultIfEmpty{TSource}(IEnumerable{TSource}, TSource)"/>
	public virtual IEnumerable<TSource> DefaultIfEmpty(TSource defaultValue) => Any() ? this : [defaultValue];
}
