using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="IDictionary{TKey, TValue}"/>
	/// </summary>
	/// <seealso cref="IDictionary{TKey, TValue}"/>
	[DebuggerStepThrough]
	public static class DictionaryEx
	{
		/// <summary>
		/// Adds an element with the provided key and value to the
		/// <see cref="IDictionary{TKey, TValue}"/> when the key does
		/// not contain in the dictionary.
		/// </summary>
		/// <typeparam name="TKey">
		/// The type of keys. The key should be not <see langword="null"/>.
		/// </typeparam>
		/// <typeparam name="TValue">The type of elements.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The dictionary.</param>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddIfKeyDoesNotContain<TKey, TValue>(
			this IDictionary<TKey, TValue> @this, TKey key, TValue value)
			where TKey : IEquatable<TKey>
		{
			if (!@this.ContainsKey(key))
			{
				@this.Add(key, value);
			}
		}
	}
}
