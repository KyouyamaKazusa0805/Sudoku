using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Sudoku.DocComments;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="byte"/>.
	/// </summary>
	/// <seealso cref="byte"/>
	public static class ByteEx
	{
		/// <inheritdoc cref="Integer.IsPowerOfTwo(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPowerOfTwo(this byte @this) => @this != 0 && (@this & (@this - 1)) == 0;

		/// <inheritdoc cref="Integer.FindFirstSet(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FindFirstSet(this byte @this) =>
			@this == 0 ? -1 : BitOperations.TrailingZeroCount(@this);

		/// <inheritdoc cref="Integer.PopCount(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int PopCount(this byte @this) => BitOperations.PopCount(@this);

		/// <inheritdoc cref="Integer.GetNextSet(Integer, int)"/>
		public static int GetNextSet(this byte @this, int index)
		{
			for (int i = index + 1; i < 8; i++)
			{
				if ((@this & 1 << i) != 0)
				{
					return i;
				}
			}

			return -1;
		}

		/// <inheritdoc cref="Integer.SetAt(Integer, int)"/>
		public static int SetAt(this byte @this, int order)
		{
			for (int i = 0, count = -1; i < 8; i++, @this >>= 1)
			{
				if ((@this & 1) != 0 && ++count == order)
				{
					return i;
				}
			}

			return -1;
		}

		/// <inheritdoc cref="Integer.GetAllSets(Integer)"/>
		public static IEnumerable<int> GetAllSets(this byte @this)
		{
			if (@this == 0)
			{
				yield break;
			}

			for (int i = 0; i < 8; i++, @this >>= 1)
			{
				if ((@this & 1) != 0)
				{
					yield return i;
				}
			}
		}

		/// <inheritdoc cref="Integer.GetEnumerator(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerator<int> GetEnumerator(this byte @this) => @this.GetAllSets().GetEnumerator();

		/// <inheritdoc cref="Integer.ReverseBits(ref Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReverseBits(this ref byte @this)
		{
			@this = (byte)(@this >> 1 & 0x55 | (@this & 0x55) << 1);
			@this = (byte)(@this >> 2 & 0x33 | (@this & 0x33) << 2);
			@this = (byte)(@this >> 4 | @this << 4);
		}
	}
}
