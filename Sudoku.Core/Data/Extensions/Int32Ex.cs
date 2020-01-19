using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="int"/>.
	/// </summary>
	/// <seealso cref="int"/>
	public static class Int32Ex
	{
		/// <summary>
		/// The return value table used in <see cref="FindFirstSet(int)"/>.
		/// </summary>
		/// <seealso cref="FindFirstSet(int)"/>
		private static readonly byte[] Table =
		{
			0, 1, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4,
			5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
			6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8
		};


		/// <summary>
		/// Find the first offset of set bit of the binary representation of the specified value.
		/// </summary>
		/// <param name="this">The value.</param>
		/// <returns>An <see cref="int"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FindFirstSet(this int @this)
		{
			int x = @this & -@this;
			int a = x <= 0xffff ? (x <= 0xff ? 0 : 8) : (x <= 0xffffff ? 16 : 24);
			return Table[x >> a] + a - 1;
		}

		/// <summary>
		/// Get the total number of set bits of the binary representation of a specified value.
		/// </summary>
		/// <param name="this">The value.</param>
		/// <returns>An <see cref="int"/> value indicating that.</returns>
		public static int CountSet(this int @this)
		{
			int count;
			for (count = 0; @this != 0; @this &= @this - 1, count++) ;
			return count;
		}

		/// <summary>
		/// Find all offsets of set bits of the binary representation of a specified value.
		/// </summary>
		/// <param name="this">The value.</param>
		/// <returns>All offsets.</returns>
		public static IEnumerable<int> GetAllSets(this int @this)
		{
			for (int i = 0; i < 32; i++, @this >>= 1)
			{
				if ((@this & 1) != 0)
				{
					yield return i;
				}
			}
		}

		/// <summary>
		/// Reverse all bits in a specified value.
		/// </summary>
		/// <param name="this">(ref parameter) The value.</param>
		/// <remarks>
		/// Note that the value is passed by reference though the
		/// method is an extension method, and returns nothing.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReverseBits(this ref int @this)
		{
			@this = ((@this >> 1) & 0x55555555) | ((@this & 0x55555555) << 1);
			@this = ((@this >> 2) & 0x33333333) | ((@this & 0x33333333) << 2);
			@this = ((@this >> 4) & 0x0f0f0f0f) | ((@this & 0x0f0f0f0f) << 4);
			@this = ((@this >> 8) & 0x00ff00ff) | ((@this & 0x00ff00ff) << 8);
			@this = (@this >> 16) | (@this << 16);
		}
	}
}
