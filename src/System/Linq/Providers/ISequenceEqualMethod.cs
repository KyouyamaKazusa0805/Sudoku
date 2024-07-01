namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>SequenceEqual</c>.
/// </summary>
/// <inheritdoc/>
public interface ISequenceEqualMethod<TSelf, TSource> : ILinqMethod<TSelf, TSource>
	where TSelf : ISequenceEqualMethod<TSelf, TSource>, allows ref struct
	where TSource : notnull
{
	/// <inheritdoc cref="Enumerable.SequenceEqual{TSource}(IEnumerable{TSource}, IEnumerable{TSource})"/>
	public virtual bool SequenceEqual(IEnumerable<TSource> second) => SequenceEqual(second, null);

	/// <inheritdoc cref="Enumerable.SequenceEqual{TSource}(IEnumerable{TSource}, IEnumerable{TSource}, IEqualityComparer{TSource}?)"/>
	public virtual bool SequenceEqual(IEnumerable<TSource> second, IEqualityComparer<TSource>? comparer)
	{
		using var e1 = GetEnumerator();
		using var e2 = second.GetEnumerator();
		comparer ??= EqualityComparer<TSource>.Default;

		while (e1.MoveNext())
		{
			if (!e2.MoveNext())
			{
				return false;
			}

			if (comparer.GetHashCode(e1.Current) != comparer.GetHashCode(e2.Current) || !comparer.Equals(e1.Current, e2.Current))
			{
				return false;
			}
		}
		return !e2.MoveNext();
	}
}
