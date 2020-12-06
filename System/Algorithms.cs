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
		public static int Min(int a, int b, int c) => Math.Min(Math.Min(a, b), c);
	}
}
