using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System
{
	/// <summary>
	/// Provides all algorithm processing methods.
	/// </summary>
	public static class Algorithms
	{
		/// <summary>
		/// To swap the two variables.
		/// </summary>
		/// <typeparam name="T">The type of the variable.</typeparam>
		/// <param name="left">The left variable.</param>
		/// <param name="right">The right variable.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Swap<T>([MaybeNull] ref T left, [MaybeNull] ref T right)
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
			int result = int.MaxValue;
			foreach (int value in array)
			{
				if (value <= result)
				{
					result = value;
				}
			}

			return result;
		}
	}
}
