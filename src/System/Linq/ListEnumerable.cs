namespace System.Linq;

/// <summary>
/// Represents with LINQ methods for <see cref="List{T}"/>.
/// </summary>
/// <seealso cref="List{T}"/>
public static class ListEnumerable
{
	/// <inheritdoc cref="ArrayEnumerable.Count{T}(T[], Func{T, bool})"/>
	public static int Count<T>(this List<T> @this, Func<T, bool> predicate)
	{
		var result = 0;
		foreach (var element in @this)
		{
			if (predicate(element))
			{
				result++;
			}
		}
		return result;
	}

	/// <inheritdoc cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	public static ReadOnlySpan<TSource> Where<TSource>(this List<TSource> source, Func<TSource, bool> condition)
	{
		var result = new List<TSource>(source.Count);
		foreach (var element in source)
		{
			if (condition(element))
			{
				result.AddRef(in element);
			}
		}
		return result.AsReadOnlySpan();
	}

	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})"/>
	public static ReadOnlySpan<TResult> Select<TSource, TResult>(this List<TSource> source, Func<TSource, TResult> selector)
	{
		var result = new TResult[source.Count];
		var i = 0;
		foreach (var element in source)
		{
			result[i++] = selector(element);
		}
		return result;
	}

	/// <inheritdoc cref="Enumerable.SelectMany{TSource, TCollection, TResult}(IEnumerable{TSource}, Func{TSource, IEnumerable{TCollection}}, Func{TSource, TCollection, TResult})"/>
	public static ReadOnlySpan<TResult> SelectMany<TSource, TCollection, TResult>(
		this List<TSource> @this,
		Func<TSource, IEnumerable<TCollection>> collectionSelector,
		Func<TSource, TCollection, TResult> resultSelector
	)
	{
		var result = new List<TResult>(@this.Count << 1);
		foreach (var element in @this)
		{
			foreach (var collectionElement in collectionSelector(element))
			{
				result.AddRef(resultSelector(element, collectionElement));
			}
		}
		return result.AsReadOnlySpan();
	}
}
