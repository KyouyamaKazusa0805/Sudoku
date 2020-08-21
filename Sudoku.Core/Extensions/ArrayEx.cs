using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Array"/>.
	/// </summary>
	/// <seealso cref="Array"/>
	[DebuggerStepThrough]
	public static class ArrayEx
	{
		/// <summary>
		/// Sorts the elements in an entire <typeparamref name="T"/>[] using the default
		/// <see cref="IComparable{T}"/> generic interface implementation of each element
		/// of the <see cref="Array"/>.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The array.</param>
		/// <seealso cref="IComparable{T}"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Sort<T>(this T[] @this) => Array.Sort(@this);

		/// <summary>
		/// Sorts the elements in an <typeparamref name="T"/>[] using the specified <see cref="Comparison{T}"/>.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The array.</param>
		/// <param name="comparison">The comparison method.</param>
		/// <seealso cref="Comparison{T}"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Sort<T>(this T[] @this, Comparison<T> comparison) => Array.Sort(@this, comparison);

		/// <summary>
		/// Get all subsets from the specified number of the values to take.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The array.</param>
		/// <param name="count">The number of elements you want to take.</param>
		/// <returns>All subsets.</returns>
		public static IEnumerable<T[]> GetSubsets<T>(this T[] @this, int count)
		{
			if (count == 0)
			{
				return Array.Empty<T[]>();
			}

			var result = new List<T[]>();
			g(@this, @this.Length, count, count, new int[count]);

			return result;

			void g(T[] array, int last, int count, int m, int[] b)
			{
				for (int i = last; i >= m; i--)
				{
					b[m - 1] = i - 1;
					if (m > 1)
					{
						g(array, i - 1, count, m - 1, b);
					}
					else
					{
						var temp = new T[count];
						for (int j = 0; j < b.Length; j++)
						{
							temp[j] = array[b[j]];
						}

						result.Add(temp);
					}
				}
			}
		}
	}
}
