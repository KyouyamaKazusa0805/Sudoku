using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="IList{T}"/>.
	/// </summary>
	/// <seealso cref="IList{T}"/>
	[DebuggerStepThrough]
	public static class ListEx
	{
		/// <summary>
		/// Remove the last element of the specified list, which is equivalent to code:
		/// <code>
		/// list.RemoveAt(list.Count - 1);
		/// </code>
		/// or
		/// <code>
		/// list.RemoveAt(^1); // Call extension method 'RemoveAt'.
		/// </code>
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
		/// <seealso cref="RemoveAt{T}(IList{T}, Index)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void RemoveLastElement<T>(this IList<T> @this) => @this.RemoveAt(@this.Count - 1);

		/// <summary>
		/// Remove at the element in the specified index.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
		/// <param name="index">The index to remove.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void RemoveAt<T>(this IList<T> @this, Index index) => @this.RemoveAt(index.GetOffset(@this.Count));

		/// <summary>
		/// Remove duplicate element in the list.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Distinct<T>(this IList<T> @this) where T : notnull
		{
			var tempList = Enumerable.Distinct(@this);
			@this.Clear();
			@this.AddRange(tempList);
		}
	}
}
