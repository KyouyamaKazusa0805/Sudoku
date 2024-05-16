namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Append</c> and <c>Prepend</c>.
/// </summary>
/// <inheritdoc/>
public interface IAppendPrependMethod<TSelf, TSource> : ILinqMethod<TSelf, TSource> where TSelf : IAppendPrependMethod<TSelf, TSource>
{
	/// <inheritdoc cref="Enumerable.Append{TSource}(IEnumerable{TSource}, TSource)"/>
	public virtual IEnumerable<TSource> Append(TSource element) => [.. this, element];

	/// <inheritdoc cref="Enumerable.Prepend{TSource}(IEnumerable{TSource}, TSource)"/>
	public virtual IEnumerable<TSource> Prepend(TSource element) => [element, .. this];
}
