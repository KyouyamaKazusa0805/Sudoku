using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Span{T}"/>.
	/// </summary>
	/// <seealso cref="Span{T}"/>
	[DebuggerStepThrough]
	public static class SpanEx
	{
		/// <summary>
		/// Returns a number that represents how many elements in the specified
		/// <see cref="Span{T}"/> sequence satisfy a condition.
		/// </summary>
		/// <typeparam name="T">The element of the span instance.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The span.</param>
		/// <param name="predicate">The condition to satisfy.</param>
		/// <returns>The number of elements satisfying the condition.</returns>
		/// <remarks>
		/// This method is provides because <see cref="Span{T}"/> cannot support
		/// <see cref="IEnumerable{T}"/> interface but can be used in <see langword="foreach"/>
		/// loop. If the span has implemented <see cref="IEnumerable{T}"/>,
		/// all span instance can use <see langword="foreach"/> loop or use extension method
		/// <see cref="Enumerable.Count{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>.
		/// </remarks>
		/// <seealso cref="Enumerable.Count{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
		public static int CountWhen<T>(this Span<T> @this, Predicate<T> predicate)
		{
			int count = 0;
			foreach (var element in @this)
			{
				if (predicate(element))
				{
					count++;
				}
			}

			return count;
		}
	}
}
