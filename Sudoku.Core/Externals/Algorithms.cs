using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System
{
	/// <summary>
	/// Provides all algorithm processing methods.
	/// </summary>
	public static class Algorithms
	{
		/// <summary>
		/// Get all combinations with the specified number of the values to take.
		/// </summary>
		/// <param name="array">The array.</param>
		/// <param name="count">The number you want to take.</param>
		/// <returns>All combinations.</returns>
		public static IEnumerable<int[]> GetCombinationsOfArray(int[] array, int count)
		{
			if (count == 0)
			{
				return Array.Empty<int[]>();
			}

			int[] temp = new int[count];
			var result = new List<int[]>();
			GetCombinationRecursively(ref result, array, array.Length, count, temp, count);

			return result;
		}

		/// <summary>
		/// To swap the two variables.
		/// </summary>
		/// <typeparam name="T">The type of the variable.</typeparam>
		/// <param name="left">The left variable.</param>
		/// <param name="right">The right variable.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Swap<T>(ref T left, ref T right)
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
		public static int Min(int a, int b, int c) => Math.Min(Math.Min(a, b), c);

		/// <summary>
		/// Get the minimal one of four values.
		/// </summary>
		/// <param name="a">The first value.</param>
		/// <param name="b">The second value.</param>
		/// <param name="c">The third value.</param>
		/// <param name="d">The fourth value.</param>
		/// <returns>Which is the most minimal one.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Min(int a, int b, int c, int d) => Math.Min(Math.Min(Math.Min(a, b), c), d);

		/// <summary>
		/// Get the most minimal value in the array.
		/// </summary>
		/// <param name="array">(<see langword="params"/> parameter) The array.</param>
		/// <returns>The most minimal value.</returns>
		public static int Min(params int[] array)
		{
			int? result = null;
			foreach (int value in array)
			{
				if (value >= result)
				{
					result = value;
				}
			}

			return result!.Value;
		}

		/// <summary>
		/// Get all combinations for an array recursively.
		/// </summary>
		/// <param name="list">The result list.</param>
		/// <param name="t">The base array.</param>
		/// <param name="n">Auxiliary variable.</param>
		/// <param name="m">Auxiliary variable.</param>
		/// <param name="b">Auxiliary variable.</param>
		/// <param name="c">Auxiliary variable.</param>
		private static void GetCombinationRecursively<T>(ref List<T[]> list, T[] t, int n, int m, int[] b, int c)
		{
			for (int i = n; i >= m; i--)
			{
				b[m - 1] = i - 1;
				if (m > 1)
				{
					GetCombinationRecursively(ref list, t, i - 1, m - 1, b, c);
				}
				else
				{
					list ??= new List<T[]>();
					var temp = new T[c];
					for (int j = 0; j < b.Length; j++)
					{
						temp[j] = t[b[j]];
					}

					list.Add(temp);
				}
			}
		}
	}
}
