using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Array"/>.
	/// </summary>
	/// <seealso cref="Array"/>
	public static class ArrayEx
	{
		/// <summary>
		/// Get all combinations that each sub-array only choose one.
		/// </summary>
		/// <param name="array">The jigsaw array.</param>
		/// <returns>
		/// All combinations that each sub-array choose one. For example, if the array is
		/// <c>{ { 1, 2, 3 }, { 1, 3 }, { 1, 4, 7, 10 } }</c>, all combinations are:
		/// <list type="table">
		/// <item><c>{ 1, 1, 1 }</c>, <c>{ 1, 1, 4 }</c>, <c>{ 1, 1, 7 }</c>, <c>{ 1, 1, 10 }</c>,</item>
		/// <item><c>{ 1, 3, 1 }</c>, <c>{ 1, 3, 4 }</c>, <c>{ 1, 3, 7 }</c>, <c>{ 1, 3, 10 }</c>,</item>
		/// <item><c>{ 2, 1, 1 }</c>, <c>{ 2, 1, 4 }</c>, <c>{ 2, 1, 7 }</c>, <c>{ 2, 1, 10 }</c>,</item>
		/// <item><c>{ 2, 3, 1 }</c>, <c>{ 2, 3, 4 }</c>, <c>{ 2, 3, 7 }</c>, <c>{ 2, 3, 10 }</c>,</item>
		/// <item><c>{ 3, 1, 1 }</c>, <c>{ 3, 1, 4 }</c>, <c>{ 3, 1, 7 }</c>, <c>{ 3, 1, 10 }</c>,</item>
		/// <item><c>{ 3, 3, 1 }</c>, <c>{ 3, 3, 4 }</c>, <c>{ 3, 3, 7 }</c>, <c>{ 3, 3, 10 }</c></item>
		/// </list>
		/// 24 cases in total.
		/// </returns>
		/// <remarks>
		/// Please note that each return values unit (an array) contains the same number of elements
		/// with the whole array.
		/// </remarks>
		[SkipLocalsInit]
		public static IEnumerable<int[]> GetCombinations(this int[][] array)
		{
			unsafe
			{
				int length = array.GetLength(0), resultCount = 1;
				int* tempArray = stackalloc int[length];
				for (int i = 0; i < length; i++)
				{
					tempArray[i] = -1;
					resultCount *= array[i].Length;
				}

				int[][] result = new int[resultCount][];
				int m = -1, n = -1;
				do
				{
					if (m < length - 1)
					{
						m++;
					}

					int* value = tempArray + m;
					(*value)++;
					if (*value > array[m].Length - 1)
					{
						*value = -1;
						m -= 2; // Backtrack.
					}

					if (m == length - 1)
					{
						n++;
						result[n] = new int[m + 1];
						for (int i = 0; i <= m; i++)
						{
							result[n][i] = array[i][tempArray[i]];
						}
					}
				} while (m >= -1);

				return result;
			}
		}

		/// <summary>
		/// Sort the specified array.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The array.</param>
		/// <param name="comparer">The method to compare two elements.</param>
		/// <remarks>
		/// If you want to use this method, please note that the <typeparamref name="T"/> may not be the built-in
		/// types such as <see cref="int"/>, <see cref="float"/> or so on, because they can use operators directly.
		/// </remarks>
		public static unsafe void Sort<T>(this T[] @this, delegate*<in T, in T, int> comparer)
		{
			q(0, @this.Length - 1, @this, comparer);

			static void q(int l, int r, T[] @this, delegate*<in T, in T, int> comparer)
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

					q(l, i, @this, comparer);
					q(i + 1, r, @this, comparer);
				}
			}
		}

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
			g(@this.Length, count, count, stackalloc int[count], @this, result);
			return result;

			static void g(
				int last, int count, int index, in Span<int> tempArray, in T[] @this, in IList<T[]> resultList)
			{
				for (int i = last; i >= index; i--)
				{
					tempArray[index - 1] = i - 1;
					if (index > 1)
					{
						g(i - 1, count, index - 1, tempArray, @this, resultList);
					}
					else
					{
						var temp = new T[count];
						for (int j = 0; j < tempArray.Length; j++)
						{
							temp[j] = @this[tempArray[j]];
						}

						resultList.Add(temp);
					}
				}
			}
		}
	}
}
