using System.Collections.Generic;
using System.Linq;

namespace Sudoku.Extensions
{
	public static class DictionaryEx
	{
		/// <summary>
		/// Converts the specified dictionary to an array, if each value can be iterated.
		/// </summary>
		/// <typeparam name="TKey">The type of the key of the dictioanry.</typeparam>
		/// <typeparam name="TValue">The type of each value.</typeparam>
		/// <typeparam name="T">
		/// The iterator type, which means the iteration type for the <typeparamref name="TValue"/>
		/// instances.
		/// </typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The dictionary.</param>
		/// <returns>The result array.</returns>
		public static T[][] ToArray<TKey, TValue, T>(this Dictionary<TKey, TValue> @this)
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

		/// <summary>
		/// Try to add a series of elements into the dictionary.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The dictionary.</param>
		/// <param name="list">The list to add into the dictionary.</param>
		/// <returns>The number of elements that is successful to add.</returns>
		public static int TryAddRange<TKey, TValue>(
			this Dictionary<TKey, TValue> @this, IEnumerable<KeyValuePair<TKey, TValue>> list)
			where TKey : notnull
		{
			int resultCount = 0;
			foreach (var (key, value) in list)
			{
				resultCount += @this.TryAdd(key, value) ? 1 : 0;
			}

			return resultCount;
		}
	}
}
