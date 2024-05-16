namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Slice</c>.
/// </summary>
/// <inheritdoc/>
public interface ISliceProvider<TSelf, TSource> : ILinqMethodProvider<TSelf, TSource>
	where TSelf : ISliceProvider<TSelf, TSource>
{
	/// <inheritdoc cref="ReadOnlySpan{T}.Slice(int, int)"/>
	public virtual IEnumerable<TSource> Slice(int start, int count) => new List<TSource>(this).Slice(start, count);
}
