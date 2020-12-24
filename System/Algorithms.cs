using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using System.Runtime.CompilerServices;
using UnsafeOperations = System.Runtime.CompilerServices.Unsafe;

namespace System
{
	/// <summary>
	/// Provides all algorithm processing methods.
	/// </summary>
	public static class Algorithms
	{
		/// <summary>
		/// Get all mask combinations.
		/// </summary>
		/// <param name="value">The mask.</param>
		/// <returns>The result list.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static short[] GetMaskSubsets(short value) => (
			from target in from size in Enumerable.Range(1, 9) select GetMaskSubsets(value, size)
			select CreateBitsInt16(target)).ToArray();

		/// <summary>
		/// Get all mask combinations.
		/// </summary>
		/// <param name="value">The mask.</param>
		/// <param name="size">The size.</param>
		/// <returns>The result list.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static short[] GetMaskSubsets(short value, int size) => (
			from target in value.GetAllSets().ToArray().GetSubsets(size)
			select CreateBitsInt16(target)).ToArray();

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
		/// To swap the two variables.
		/// </summary>
		/// <typeparam name="T">The type of the variable.</typeparam>
		/// <param name="left">The left variable.</param>
		/// <param name="right">The right variable.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Swap<T>(ref T? left, ref T? right)
		{
			var temp = left;
			left = right;
			right = temp;
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
		/// Check which enumeration field is less.
		/// </summary>
		/// <typeparam name="TEnum">The type of the enumeration field to compare.</typeparam>
		/// <param name="left">The left one.</param>
		/// <param name="right">The right one.</param>
		/// <returns>The comparsion result.</returns>
		public static TEnum Min<TEnum>(TEnum left, TEnum right) where TEnum : unmanaged, Enum
		{
			unsafe
			{
				switch (sizeof(TEnum))
				{
					case 1:
					case 2:
					case 4:
					{
						int l = UnsafeOperations.As<TEnum, int>(ref left);
						int r = UnsafeOperations.As<TEnum, int>(ref right);
						return l < r ? left : right;
					}
					case 8:
					{
						long l = UnsafeOperations.As<TEnum, long>(ref left);
						long r = UnsafeOperations.As<TEnum, long>(ref right);
						return l < r ? left : right;
					}
					default:
					{
						return default;
					}
				}
			}
		}

		/// <summary>
		/// Check which enumeration field is greater.
		/// </summary>
		/// <typeparam name="TEnum">The type of the enumeration field to compare.</typeparam>
		/// <param name="left">The left one.</param>
		/// <param name="right">The right one.</param>
		/// <returns>The comparsion result.</returns>
		public static TEnum Max<TEnum>(TEnum left, TEnum right) where TEnum : unmanaged, Enum
		{
			unsafe
			{
				switch (sizeof(TEnum))
				{
					case 1:
					case 2:
					case 4:
					{
						int l = UnsafeOperations.As<TEnum, int>(ref left);
						int r = UnsafeOperations.As<TEnum, int>(ref right);
						return l > r ? left : right;
					}
					case 8:
					{
						long l = UnsafeOperations.As<TEnum, long>(ref left);
						long r = UnsafeOperations.As<TEnum, long>(ref right);
						return l > r ? left : right;
					}
					default:
					{
						return default;
					}
				}
			}
		}

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
		public static IEnumerable<int[]> GetCombinations(int[][] array)
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
	}
}
