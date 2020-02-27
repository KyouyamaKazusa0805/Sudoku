using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="IBag{T}"/>.
	/// </summary>
	/// <seealso cref="IBag{T}"/>
	[DebuggerStepThrough]
	public static class BagEx
	{
		/// <summary>
		/// Adds an object to the end of the <see cref="IBag{T}"/> when
		/// the specified list does not contain the specified element.
		/// </summary>
		/// <typeparam name="T">
		/// The type of all elements. Should be not <see langword="null"/>.
		/// </typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
		/// <param name="item">The item to add.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddIfDoesNotContain<T>(this IBag<T> @this, T item)
			where T : notnull
		{
			if (!@this.Contains(item))
			{
				@this.Add(item);
			}
		}
	}
}
