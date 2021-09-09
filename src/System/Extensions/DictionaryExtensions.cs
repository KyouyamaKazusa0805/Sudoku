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
	/// <typeparam name="TValue">The type of each value.</typeparam>
	/// <typeparam name="T">
	/// The iterator type, which means the iteration type for the <typeparamref name="TValue"/>
	/// instances.
	/// </typeparam>
	/// <param name="this">The dictionary.</param>
	/// <returns>The result array.</returns>
	public static T[][] ToArray<TKey, TValue, T>(this IDictionary<TKey, TValue> @this)
	where TKey : notnull
	where TValue : IEnumerable<T>
	{
		var keys = @this.Keys;
		var result = new T[keys.Count][];
		int i = 0;
		foreach (var key in keys)
		{
			result[i++] = @this[key].ToArray();
		}

		return result;
	}
}
