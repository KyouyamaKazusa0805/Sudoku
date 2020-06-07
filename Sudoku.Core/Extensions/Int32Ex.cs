using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="int"/>.
	/// </summary>
	/// <seealso cref="int"/>
	[DebuggerStepThrough]
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


		/// <include file='CoreDocComments.xml' path='comments/method[@name="IsPowerOfTwo"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPowerOfTwo(this int @this) => @this != 0 && (@this & (@this - 1)) == 0;

		/// <include file='CoreDocComments.xml' path='comments/method[@name="FindFirstSet"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FindFirstSet(this int @this)
		{
			int x = @this & -@this;
			int a = x <= 0xFFFF ? x <= 0xFF ? 0 : 8 : x <= 0xFFFFFF ? 16 : 24;
			return Table[x >> a] + a - 1;
		}

		/// <include file='CoreDocComments.xml' path='comments/method[@name="CountSet"]'/>
		public static int CountSet(this int @this)
		{
			if (@this == 0)
			{
				return 0;
			}

			int count = 0;
			for (; @this != 0; @this &= @this - 1, count++) ;
			return count;
		}

		/// <include file='CoreDocComments.xml' path='comments/method[@name="GetNextSet"]'/>
		public static int GetNextSet(this int @this, int index)
		{
			for (int i = index + 1; i < 32; i++)
			{
				if ((@this & 1 << i) != 0)
				{
					return i;
				}
			}

			return -1;
		}

		/// <include file='CoreDocComments.xml' path='comments/method[@name="SetAt"]'/>
		public static int SetAt(this int @this, int order)
		{
			for (int i = 0, count = -1; i < 32; i++, @this >>= 1)
			{
				if ((@this & 1) != 0 && ++count == order)
				{
					return i;
				}
			}

			return -1;
		}

		/// <include file='CoreDocComments.xml' path='comments/method[@name="GetAllSets"]'/>
		public static IEnumerable<int> GetAllSets(this int @this)
		{
			if (@this == 0)
			{
				yield break;
			}

			for (int i = 0; i < 32; i++, @this >>= 1)
			{
				if ((@this & 1) != 0)
				{
					yield return i;
				}
			}
		}

		/// <include file='CoreDocComments.xml' path='comments/method[@name="GetEnumerator"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerator<int> GetEnumerator(this int @this) => @this.GetAllSets().GetEnumerator();

		/// <include file='CoreDocComments.xml' path='comments/method[@name="ReverseBits"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReverseBits(this ref int @this)
		{
			@this = @this >> 1 & 0x55555555 | (@this & 0x55555555) << 1;
			@this = @this >> 2 & 0x33333333 | (@this & 0x33333333) << 2;
			@this = @this >> 4 & 0x0F0F0F0F | (@this & 0x0F0F0F0F) << 4;
			@this = @this >> 8 & 0x00FF00FF | (@this & 0x00FF00FF) << 8;
			@this = @this >> 16 | @this << 16;
		}
	}
}
