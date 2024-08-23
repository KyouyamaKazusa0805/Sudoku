namespace System.Linq;

/// <summary>
/// Provides with LINQ methods on a <see cref="string"/> value.
/// </summary>
public static class StringEnumerable
{
	/// <summary>
	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})" path="/summary"/>
	/// </summary>
	/// <param name="this">The string instance.</param>
	/// <param name="selector">
	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})" path="/param[@name='selector']"/>
	/// </param>
	/// <returns>
	/// A <see cref="ReadOnlySpan{T}"/> of <typeparamref name="TResult"/> whose elements are the result of invoking the transfrom function
	/// on each element of <paramref name="this"/>.
	/// </returns>
	public static ReadOnlySpan<TResult> Select<TResult>(this string @this, Func<char, TResult> selector)
	{
		var result = new TResult[@this.Length];
		var i = 0;
		foreach (var element in @this)
		{
			result[i++] = selector(element);
		}
		return result;
	}

	/// <summary>
	/// <inheritdoc cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, bool})" path="/summary"/>
	/// </summary>
	/// <param name="this">The string instance.</param>
	/// <param name="predicate">
	/// <inheritdoc cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, bool})" path="/param[@name='selector']"/>
	/// </param>
	/// <returns>
	/// A new <see cref="string"/> instance that contains characters from the input sequence that satisfy the condition.
	/// </returns>
	public static ReadOnlyCharSequence Where(this string @this, Func<char, bool> predicate)
	{
		var result = new char[@this.Length];
		var i = 0;
		foreach (var element in @this)
		{
			if (predicate(element))
			{
				result[i++] = element;
			}
		}
		return result;
	}

	/// <summary>
	/// Projects each element of a sequence to a <see cref="string"/>, flattens the resulting sequences into one sequence,
	/// and invokes a result selector function on each element therein.
	/// </summary>
	/// <param name="this"></param>
	/// <param name="collectionSelector"></param>
	/// <param name="resultSelector"></param>
	/// <returns>
	/// A <see cref="string"/> whose elements are the result of invoking the one-to-many transform function
	/// <paramref name="collectionSelector"/> on each element of <paramref name="this"/>
	/// and then mapping each of those sequence elements and their corresponding source element to a result element.
	/// </returns>
	public static ReadOnlySpan<string> SelectMany(this string @this, Func<char, string> collectionSelector, Func<char, char, string> resultSelector)
	{
		var length = @this.Length;
		var result = new List<string>(length << 1);
		for (var i = 0; i < length; i++)
		{
			var element = @this[i];
			foreach (var subElement in collectionSelector(element))
			{
				result.Add(resultSelector(element, subElement));
			}
		}
		return result.AsReadOnlySpan();
	}
}
