using System.Runtime.CompilerServices;

namespace System.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Math"/>.
	/// </summary>
	/// <seealso cref="Math"/>
	public static class MathEx
	{
		/// <summary>
		/// Get the minimal one of three values.
		/// </summary>
		/// <param name="a">The first value.</param>
		/// <param name="b">The second value.</param>
		/// <param name="c">The third value.</param>
		/// <returns>Which is the minimal value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Min(int a, int b, int c) => a < b ? a < c ? a : c : b < c ? b : c;

		/// <summary>
		/// Get the maximum one of three values.
		/// </summary>
		/// <param name="a">The first value.</param>
		/// <param name="b">The second value.</param>
		/// <param name="c">The third value.</param>
		/// <returns>Which is the maximum value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Max(int a, int b, int c) => a > b ? a > c ? a : c : b > c ? b : c;
	}
}
