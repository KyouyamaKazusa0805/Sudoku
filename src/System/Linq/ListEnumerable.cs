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
				result.Add(element);
			}
		}

		return result.ToArray();
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
}
