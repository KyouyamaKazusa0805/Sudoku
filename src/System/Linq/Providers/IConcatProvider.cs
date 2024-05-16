namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Concat</c>.
/// </summary>
/// <inheritdoc/>
public interface IConcatProvider<TSelf, TSource> : ILinqMethodProvider<TSelf, TSource>
	where TSelf : IConcatProvider<TSelf, TSource>
{
	/// <inheritdoc cref="Enumerable.Concat{TSource}(IEnumerable{TSource}, IEnumerable{TSource})"/>
	public virtual IEnumerable<TSource> Concat(IEnumerable<TSource> second) => [.. this, .. second];
}
