namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Skip</c>, <c>SkipLast</c> and <c>SkipWhile</c>.
/// </summary>
/// <inheritdoc/>
public interface ISkipMethod<TSelf, TSource> : ILinqMethod<TSelf, TSource> where TSelf : ISkipMethod<TSelf, TSource>
{
	/// <inheritdoc cref="Enumerable.Skip{TSource}(IEnumerable{TSource}, int)"/>
	public virtual IEnumerable<TSource> Skip(int count) => new List<TSource>(this)[count..];

	/// <inheritdoc cref="Enumerable.SkipLast{TSource}(IEnumerable{TSource}, int)"/>
	public virtual IEnumerable<TSource> SkipLast(int count) => new List<TSource>(this)[..^count];

	/// <inheritdoc cref="Enumerable.SkipWhile{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	public virtual IEnumerable<TSource> SkipWhile(Func<TSource, bool> predicate)
	{
		var list = new List<TSource>(this);
		var index = list.FindIndex(element => predicate(element));
		return list[index == -1 ? .. : (index + 1)..];
	}
}
