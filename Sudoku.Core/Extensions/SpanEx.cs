using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Span{T}"/> and <see cref="ReadOnlySpan{T}"/>.
	/// </summary>
	/// <seealso cref="Span{T}"/>
	/// <seealso cref="ReadOnlySpan{T}"/>
	[DebuggerStepThrough]
	public static class SpanEx
	{
		/// <summary>
		/// Sum up the number of all elements satisfied the specified condition.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The elements.</param>
		/// <param name="predicate">The condition.</param>
		/// <returns>The number of elements satisfied the condition.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CountWhen<T>(this Span<T> @this, Predicate<T> predicate) =>
			((ReadOnlySpan<T>)@this).CountWhen(predicate);

		/// <summary>
		/// Sum up the number of all elements satisfied the specified condition.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The elements.</param>
		/// <param name="predicate">The condition.</param>
		/// <returns>The number of elements satisfied the condition.</returns>
		public static int CountWhen<T>(this ReadOnlySpan<T> @this, Predicate<T> predicate)
		{
			int result = 0;
			foreach (var element in @this)
			{
				if (predicate(element))
				{
					result++;
				}
			}

			return result;
		}


		/// <summary>
		/// Get the specified element with the specified index of all elements that
		/// satisfied the specified condition.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The elements.</param>
		/// <param name="index">The index.</param>
		/// <param name="predicate">The condition.</param>
		/// <returns>The result.</returns>
		[return: MaybeNull]
		public static T ElementAt<T>(this Span<T> @this, int index, Predicate<T> predicate)
		{
			int count = 0;
			foreach (var element in @this)
			{
				if (predicate(element) && ++count == index)
				{
					return element;
				}
			}

			return default!;
		}
	}
}
