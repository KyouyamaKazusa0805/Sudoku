using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="short"/>.
	/// </summary>
	/// <seealso cref="short"/>
	public static class Int16Ex
	{
		/// <summary>
		/// Find the first offset of set bit of the binary representation of the specified value.
		/// </summary>
		/// <param name="this">The value.</param>
		/// <returns>An <see cref="int"/> value indicating that.</returns>
		/// <seealso cref="Int32Ex.FindFirstSet(int)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FindFirstSet(this short @this) => Int32Ex.FindFirstSet(@this);

		/// <summary>
		/// Get the total number of set bits of the binary representation of a specified value.
		/// </summary>
		/// <param name="this">The value.</param>
		/// <returns>An <see cref="int"/> value indicating that.</returns>
		/// <seealso cref="Int32Ex.CountSet(int)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CountSet(this short @this) => Int32Ex.CountSet(@this);

		/// <summary>
		/// Find all offsets of set bits of the binary representation of a specified value.
		/// </summary>
		/// <param name="this">The value.</param>
		/// <returns>All offsets.</returns>
		public static IEnumerable<int> GetAllSets(this short @this)
		{
			for (int i = 0; i < 16; i++, @this >>= 1)
			{
				if ((@this & 1) != 0)
				{
					yield return i;
				}
			}
		}
	}
}
