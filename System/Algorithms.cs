using System.Collections.Generic;
using System.Extensions;
using System.Runtime.CompilerServices;

namespace System
{
	/// <summary>
	/// Provides all algorithm processing methods.
	/// </summary>
	public static class Algorithms
	{
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
		[CLSCompliant(false)]
		public static unsafe void Sort<T>(this T[] @this, delegate* managed<in T, in T, int> comparer)
		{
			q(0, @this.Length - 1, @this, comparer);

			static void q(int l, int r, T[] @this, delegate* managed<in T, in T, int> comparer)
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
		/// Get all mask combinations.
		/// </summary>
		/// <param name="value">The mask.</param>
		/// <returns>The result list.</returns>
		public static short[] GetMaskSubsets(short value)
		{
			short[][] maskSubsets = new short[9][];
			for (int size = 1; size <= 9; size++)
			{
				maskSubsets[size - 1] = GetMaskSubsets(value, size);
			}

			short[] result = new short[9];
			for (int i = 0; i < 9; i++)
			{
				short[] target = maskSubsets[i];
				result[i] = CreateBitsInt16(target);
			}

			return result;
		}

		/// <summary>
		/// Get all mask combinations.
		/// </summary>
		/// <param name="value">The mask.</param>
		/// <param name="size">The size.</param>
		/// <returns>The result list.</returns>
		public static short[] GetMaskSubsets(short value, int size)
		{
			var listToIterate = value.GetAllSets().GetSubsets(size);
			short[] result = new short[listToIterate.Count];
			int index = 0;
			foreach (var target in listToIterate)
			{
				result[index++] = CreateBitsInt16(target);
			}

			return result;
		}

		/// <summary>
		/// Create a <see cref="short"/> value, whose set bits are specified in the parameter
		/// <paramref name="values"/>.
		/// </summary>
		/// <param name="values">The values.</param>
		/// <returns>The mask result.</returns>
		/// <remarks>
		/// For example, if the <paramref name="values"/> are <c>{ 3, 6 }</c>, the return value
		/// will be <c>1 &lt;&lt; 3 | 1 &lt;&lt; 6</c>.
		/// </remarks>
		public static short CreateBitsInt16(int[] values)
		{
			short result = 0;
			foreach (int value in values)
			{
				result |= (short)(1 << value);
			}

			return result;
		}

		/// <inheritdoc cref="CreateBitsInt16(int[])"/>
		public static short CreateBitsInt16(short[] values)
		{
			short result = 0;
			foreach (int value in values)
			{
				result |= (short)(1 << value);
			}

			return result;
		}

		/// <summary>
		/// To swap the two variables using pointers when the pointee is an <see langword="unmanaged"/> type.
		/// </summary>
		/// <typeparam name="TUnmanaged">The type of the variable.</typeparam>
		/// <param name="left">The left variable.</param>
		/// <param name="right">The right variable.</param>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe void Swap<TUnmanaged>(TUnmanaged* left, TUnmanaged* right)
			where TUnmanaged : unmanaged
		{
			var temp = *left;
			*left = *right;
			*right = temp;
		}

		/// <summary>
		/// Get the minimal one of three values.
		/// </summary>
		/// <param name="a">The first value.</param>
		/// <param name="b">The second value.</param>
		/// <param name="c">The third value.</param>
		/// <returns>Which is the most minimal one.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Min(int a, int b, int c) => a > b ? a > c ? a : c : b > c ? b : c;

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
		public static unsafe IEnumerable<int[]> GetExtractedCombinations(this int[][] array)
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
}
