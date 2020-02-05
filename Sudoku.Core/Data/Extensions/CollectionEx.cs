using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="ICollection{T}"/>.
	/// </summary>
	/// <seealso cref="ICollection{T}"/>
	[DebuggerStepThrough]
	public static class CollectionEx
	{
		/// <summary>
		/// Adds the elements of the specified collection to the end of the
		/// <see cref="ICollection{T}"/>.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(extension method main parameter) The collection.</param>
		/// <param name="values">
		/// The values you want to add to the end of the collection.
		/// </param>
		public static void AddRange<T>(this ICollection<T> @this, IEnumerable<T> values)
		{
			foreach (var value in values)
				@this.Add(value);
		}

		/// <summary>
		/// Adds an object to the end of the <see cref="ICollection{T}"/> when
		/// the specified list does not contain the specified element.
		/// </summary>
		/// <typeparam name="T">The type of all elements.</typeparam>
		/// <param name="this">(extension method main parameter) The list.</param>
		/// <param name="item">The item to add.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddIfDoesNotContain<T>(this ICollection<T> @this, T item)
		{
			if (!@this.Contains(item))
				@this.Add(item);
		}
	}
}
