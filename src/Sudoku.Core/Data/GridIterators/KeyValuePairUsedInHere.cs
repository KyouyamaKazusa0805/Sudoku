#if SUDOKU_GRID_LINQ

using System;
using System.Collections.Generic;

namespace Sudoku.Data.GridIterators
{
	/// <summary>
	/// Provides the inner data structure for a key value pair.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <typeparam name="TElement">The type of the element.</typeparam>
	/// <param name="Key">The key.</param>
	/// <param name="Value">The value.</param>
	internal sealed record KeyValuePairUsedInHere<TKey, TValue, TElement>(TKey Key, TValue Value)
		: IGroup<TKey, TElement> where TValue : IEnumerable<TElement>
	{
		/// <inheritdoc/>
		public IEnumerator<TElement> GetEnumerator() => Value.GetEnumerator();
	}


	/// <summary>
	/// Provides the inner data structure for a key value pair, with a convert method.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <typeparam name="TElement">The type of the element.</typeparam>
	/// <typeparam name="TConverted">The type that the element converted.</typeparam>
	/// <param name="Key">The key.</param>
	/// <param name="Value">The value.</param>
	/// <param name="Converter">The convert method.</param>
	internal sealed record KeyValuePairUsedInHere<TKey, TValue, TElement, TConverted>(
		TKey Key, TValue Value, Func<TElement, TConverted> Converter
	) : IGroup<TKey, TConverted> where TValue : IEnumerable<TElement>
	{
		/// <inheritdoc/>
		public IEnumerator<TConverted> GetEnumerator()
		{
			foreach (var value in Value)
			{
				yield return Converter(value);
			}
		}
	}
}

#endif