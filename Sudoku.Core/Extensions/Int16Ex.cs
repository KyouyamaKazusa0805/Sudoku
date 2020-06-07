using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="short"/>.
	/// </summary>
	/// <seealso cref="short"/>
	[DebuggerStepThrough]
	public static class Int16Ex
	{
		/// <include file='CoreDocComments.xml' path='comments/method[@name="IsPowerOfTwo"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPowerOfTwo(this short @this) => @this != 0 && (@this & (@this - 1)) == 0;

		/// <include file='CoreDocComments.xml' path='comments/method[@name="FindFirstSet"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FindFirstSet(this short @this) => Int32Ex.FindFirstSet(@this);

		/// <include file='CoreDocComments.xml' path='comments/method[@name="CountSet"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CountSet(this short @this) => Int32Ex.CountSet(@this);

		/// <include file='CoreDocComments.xml' path='comments/method[@name="GetNextSet"]'/>
		public static int GetNextSet(this short @this, int index)
		{
			for (int i = index + 1; i < 16; i++)
			{
				if ((@this & 1 << i) != 0)
				{
					return i;
				}
			}

			return -1;
		}

		/// <include file='CoreDocComments.xml' path='comments/method[@name="SetAt"]'/>
		public static int SetAt(this short @this, int order)
		{
			for (int i = 0, count = -1; i < 16; i++, @this >>= 1)
			{
				if ((@this & 1) != 0 && ++count == order)
				{
					return i;
				}
			}

			return -1;
		}

		/// <include file='CoreDocComments.xml' path='comments/method[@name="GetAllSets"]'/>
		public static IEnumerable<int> GetAllSets(this short @this)
		{
			if (@this == 0)
			{
				yield break;
			}

			for (int i = 0; i < 16; i++, @this >>= 1)
			{
				if ((@this & 1) != 0)
				{
					yield return i;
				}
			}
		}

		/// <include file='CoreDocComments.xml' path='comments/method[@name="GetEnumerator"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerator<int> GetEnumerator(this short @this) => @this.GetAllSets().GetEnumerator();

		/// <include file='CoreDocComments.xml' path='comments/method[@name="ReverseBits"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReverseBits(this ref short @this)
		{
			@this = (short)(@this >> 1 & 0x5555 | (@this & 0x5555) << 1);
			@this = (short)(@this >> 2 & 0x3333 | (@this & 0x3333) << 2);
			@this = (short)(@this >> 4 & 0x0F0F | (@this & 0x0F0F) << 4);
			@this = (short)(@this >> 8 | @this << 8);
		}
	}
}
