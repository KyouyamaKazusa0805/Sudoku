namespace System.Collections.Generic;

/// <summary>
/// Provides extension methods on <see cref="Dictionary{TKey, TValue}"/>.
/// </summary>
/// <seealso cref="Dictionary{TKey, TValue}"/>
public static class DictionaryExtensions
{
	/// <summary>
	/// Converts the specified dictionary to an array, if each value can be iterated.
	/// </summary>
	/// <typeparam name="TKey">The type of the key of the dictionary.</typeparam>
	/// <typeparam name="TElement">The type of each element.</typeparam>
	/// <param name="this">The dictionary.</param>
	/// <returns>The result array.</returns>
	public static TElement[][] ToArray<TKey, TElement>(this IDictionary<TKey, IList<TElement>> @this)
	where TKey : notnull
	{
		var keys = @this.Keys;
		var result = new TElement[keys.Count][];
		int i = 0;
		foreach (var key in keys)
		{
			result[i++] = @this[key].ToArray();
		}

		return result;
	}

	/// <summary>
	/// Converts the specified dictionary to an array, if each value can be iterated.
	/// </summary>
	/// <typeparam name="TKey">The type of the key of the dictionary.</typeparam>
	/// <typeparam name="TElement">The type of each element.</typeparam>
	/// <param name="this">The dictionary.</param>
	/// <returns>The result array.</returns>
	public static TElement[][] ToArray<TKey, TElement>(this IDictionary<TKey, IEnumerable<TElement>> @this)
	where TKey : notnull
	{
		var keys = @this.Keys;
		var result = new TElement[keys.Count][];
		int i = 0;
		foreach (var key in keys)
		{
			result[i++] = @this[key].ToArray();
		}

		return result;
	}
}
