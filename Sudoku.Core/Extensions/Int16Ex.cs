using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.DocComments;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="short"/>.
	/// </summary>
	/// <seealso cref="short"/>
	public static class Int16Ex
	{
		/// <inheritdoc cref="Integer.IsPowerOfTwo(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPowerOfTwo(this short @this) => @this != 0 && (@this & (@this - 1)) == 0;

		/// <inheritdoc cref="Integer.FindFirstSet(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FindFirstSet(this short @this) => Int32Ex.FindFirstSet(@this);

		/// <inheritdoc cref="Integer.CountSet(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CountSet(this short @this)
		{
#if PREFER_ZERO_BITS
			if (@this == 0)
			{
				return 0;
			}

			int count = 0;
			for (; @this != 0; @this &= (short)(@this - 1), count++) ;
			return count;
#else
			@this = (short)((@this & 0x5555) + ((@this >> 1) & 0x5555));
			@this = (short)((@this & 0x3333) + ((@this >> 2) & 0x3333));
			@this = (short)((@this & 0x0F0F) + ((@this >> 4) & 0x0F0F));
			@this = (short)((@this & 0x00FF) + ((@this >> 8) & 0x00FF));
			return @this;
#endif
		}

		/// <inheritdoc cref="Integer.GetNextSet(Integer, int)"/>
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

		/// <inheritdoc cref="Integer.SetAt(Integer, int)"/>
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

		/// <inheritdoc cref="Integer.GetAllSets(Integer)"/>
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

		/// <inheritdoc cref="Integer.GetEnumerator(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerator<int> GetEnumerator(this short @this) => @this.GetAllSets().GetEnumerator();

		/// <inheritdoc cref="Integer.ReverseBits(ref Integer)"/>
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
