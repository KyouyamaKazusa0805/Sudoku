namespace System.Linq;

/// <summary>
/// Represents LINQ methods used by <see cref="HashSet{T}"/> instances.
/// </summary>
/// <seealso cref="HashSet{T}"/>
public static class HashSetEnumerable
{
	/// <summary>
	/// Indicates the first element of the collection.
	/// </summary>
	/// <typeparam name="TSource">The type of each element.</typeparam>
	/// <param name="source">The collection.</param>
	/// <returns>The first element of the collection.</returns>
	/// <exception cref="InvalidOperationException">Throws when the collection contains no elements.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TSource First<TSource>(this HashSet<TSource> source)
	{
		using var enumerator = source.GetEnumerator();
		return enumerator.MoveNext()
			? enumerator.Current
			: throw new InvalidOperationException(SR.ExceptionMessage("NoElementsFoundInCollection"));
	}

	/// <inheritdoc cref="Enumerable.First{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	public static TSource First<TSource>(this HashSet<TSource> source, Func<TSource, bool> predicate)
	{
		foreach (var element in source)
		{
			if (predicate(element))
			{
				return element;
			}
		}
		throw new InvalidOperationException(SR.ExceptionMessage("NoElementsFoundInCollection"));
	}

	/// <inheritdoc cref="Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	public static TSource? FirstOrDefault<TSource>(this HashSet<TSource> source, Func<TSource, bool> predicate)
	{
		foreach (var element in source)
		{
			if (predicate(element))
			{
				return element;
			}
		}
		return default;
	}
}
