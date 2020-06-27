using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="HashSet{T}"/>.
	/// </summary>
	/// <seealso cref="HashSet{T}"/>
	[DebuggerStepThrough]
	public static class HashSetEx
	{
		/// <summary>
		/// Insert the specified item into the current list at the specified index.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
		/// <param name="index">The index.</param>
		/// <param name="item">The item.</param>
		public static void Insert<T>(this HashSet<T> @this, int index, [MaybeNull] T item)
			where T : notnull
		{
			var list = new List<T>(@this);
			list.Insert(index, item);

			@this.Clear();
			@this.AddRange(list);
		}

		/// <summary>
		/// Get the value at the specified index.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
		/// <param name="index">The index.</param>
		/// <returns>The element.</returns>
		/// <exception cref="IndexOutOfRangeException">
		/// Throws when <paramref name="index"/> is greater than the total length of the list.
		/// </exception>
		public static T Get<T>(this HashSet<T> @this, int index)
		{
			using var enumerator = @this.GetEnumerator();
			int i = 0;
			while (enumerator.MoveNext())
			{
				if (i++ == index)
				{
					return enumerator.Current;
				}
			}

			throw new IndexOutOfRangeException();
		}

		/// <summary>
		/// Remove the element at the specified index.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
		/// <param name="index">The index.</param>
		[return: NotNull]
		public static T RemoveAt<T>(this HashSet<T> @this, int index)
			where T : notnull
		{
			var list = new List<T>(@this);
			var result = list[index];
			list.RemoveAt(index);

			@this.Clear();
			@this.AddRange(list);

			return result;
		}
	}
}
