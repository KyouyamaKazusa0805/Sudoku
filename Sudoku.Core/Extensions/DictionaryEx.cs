using System.Collections.Generic;

namespace Sudoku.Extensions
{
	public static class DictionaryEx
	{
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
