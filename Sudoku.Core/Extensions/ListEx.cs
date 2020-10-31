using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="IList{T}"/>.
	/// </summary>
	/// <seealso cref="IList{T}"/>
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
		/// <seealso cref="RemoveAt{T}(IList{T}, in Index)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void RemoveLastElement<T>(this IList<T?> @this) => @this.RemoveAt(@this.Count - 1);

		/// <summary>
		/// Remove at the element in the specified index.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
		/// <param name="index">(<see langword="in"/> parameter) The index to remove.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void RemoveAt<T>(this IList<T?> @this, in Index index) =>
			@this.RemoveAt(index.GetOffset(@this.Count));

		/// <summary>
		/// Sort the specified list.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
		/// <param name="comparer">The method to compare two elements.</param>
		/// <remarks>
		/// If you want to use this method, please note that the <typeparamref name="T"/> may not be the built-in
		/// types such as <see cref="int"/>, <see cref="float"/> or so on, because they can use operators directly.
		/// </remarks>
		public static unsafe void Sort<T>(this IList<T> @this, delegate*<in T, in T, int> comparer)
		{
			q(0, @this.Count - 1);

			void q(int l, int r)
			{
				if (l < r)
				{
					int i = l, j = r - 1;
					var middle = @this[(l + r) / 2];
					while (true)
					{
						while (i < r && comparer(@this[i], middle) < 0) i++;
						while (j > 0 && comparer(@this[j], middle) > 0) j--;
						if (i == j) break;

						var temp = @this[i];
						@this[i] = @this[j];
						@this[j] = temp;

						if (comparer(@this[i], @this[j]) == 0) j--;
					}

					q(l, i);
					q(i + 1, r);
				}
			}
		}

		/// <summary>
		/// Remove duplicate element in the list.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Distinct<T>(this IList<T> @this)
		{
			var tempList = Enumerable.Distinct(@this).ToList(); // Do not forget '.ToList'.
			@this.Clear();
			@this.AddRange(tempList);
		}
	}
}
