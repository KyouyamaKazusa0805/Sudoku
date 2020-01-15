using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sudoku.Data.Extensions
{
	public static class Int32Ex
	{
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


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FindFirstSet(this int @this)
		{
			int x = @this & -@this;
			int a = x <= 0xffff ? (x <= 0xff ? 0 : 8) : (x <= 0xffffff ? 16 : 24);
			return Table[x >> a] + a - 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CountSet(this int @this)
		{
			int count;
			for (count = 0; @this != 0; @this &= @this - 1, count++) ;
			return count;
		}

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
