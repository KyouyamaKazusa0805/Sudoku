using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="long"/>.
	/// </summary>
	/// <seealso cref="long"/>
	[DebuggerStepThrough]
	public static class Int64Ex
	{
		/// <include file='CoreDocComments.xml' path='comments/method[@name="IsPowerOfTwo"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPowerOfTwo(this long @this) => @this != 0 && (@this & (@this - 1L)) == 0;

		/// <include file='CoreDocComments.xml' path='comments/method[@name="FindFirstSet"]'/>
		public static int FindFirstSet(this long @this)
		{
			if (@this == 0)
			{
				return 0;
			}

			int r = 0;
			if ((@this & 0xFFFFFFFFL) == 0) { @this >>= 32; r += 32; }
			if ((@this & 0xFFFFL) == 0) { @this >>= 16; r += 16; }
			if ((@this & 0xFFL) == 0) { @this >>= 8; r += 8; }
			if ((@this & 0xFL) == 0) { @this >>= 4; r += 4; }
			if ((@this & 3L) == 0) { @this >>= 2; r += 2; }
			if ((@this & 1L) == 0) { /*@this >>= 1;*/ r++; }

			return r;
		}

		/// <include file='CoreDocComments.xml' path='comments/method[@name="CountSet"]'/>
		public static int CountSet(this long @this)
		{
			int count;
			for (count = 0; @this != 0; @this &= @this - 1L, count++) ;
			return count;
		}

		/// <include file='CoreDocComments.xml' path='comments/method[@name="GetNextSet"]'/>
		public static int GetNextSet(this long @this, int index)
		{
			for (int i = index + 1; i < 64; i++)
			{
				if ((@this & 1L << i) != 0)
				{
					return i;
				}
			}

			return -1;
		}

		/// <include file='CoreDocComments.xml' path='comments/method[@name="SetAt"]'/>
		public static int SetAt(this long @this, int order)
		{
			for (int i = 0, count = -1; i < 64; i++, @this >>= 1)
			{
				if ((@this & 1L) != 0 && ++count == order)
				{
					return i;
				}
			}

			return -1;
		}

		/// <include file='CoreDocComments.xml' path='comments/method[@name="GetAllSets"]'/>
		public static IEnumerable<int> GetAllSets(this long @this)
		{
			if (@this == 0)
			{
				yield break;
			}

			for (int i = 0; i < 64; i++, @this >>= 1)
			{
				if ((@this & 1L) != 0)
				{
					yield return i;
				}
			}
		}

		/// <include file='CoreDocComments.xml' path='comments/method[@name="GetEnumerator"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerator<int> GetEnumerator(this long @this) => @this.GetAllSets().GetEnumerator();

		/// <include file='CoreDocComments.xml' path='comments/method[@name="ReverseBits"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[SuppressMessage("", "IDE0055:Fix formatting")]
		public static void ReverseBits(this ref long @this)
		{
			@this = @this >>  1 & 0x55555555_55555555L | (@this & 0x55555555_55555555L) <<  1;
			@this = @this >>  2 & 0x33333333_33333333L | (@this & 0x33333333_33333333L) <<  2;
			@this = @this >>  4 & 0x0F0F0F0F_0F0F0F0FL | (@this & 0x0F0F0F0F_0F0F0F0FL) <<  4;
			@this = @this >>  8 & 0x00FF00FF_00FF00FFL | (@this & 0x00FF00FF_00FF00FFL) <<  8;
			@this = @this >> 16 & 0x0000FFFF_0000FFFFL | (@this & 0x0000FFFF_0000FFFFL) << 16;
			@this = @this >> 32 | @this << 32;
		}
	}
}
