using System.Numerics;
using System.Runtime.CompilerServices;
using Sudoku.DocComments;

namespace System.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="long"/>.
	/// </summary>
	/// <seealso cref="long"/>
	public static class Int64Ex
	{
		/// <inheritdoc cref="Integer.IsPowerOfTwo(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPowerOfTwo(this long @this) => @this != 0 && (@this & (@this - 1L)) == 0;

		/// <inheritdoc cref="Integer.IsOdd(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsOdd(this long @this) => (@this & 1) != 0;

		/// <inheritdoc cref="Integer.IsEven(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsEven(this long @this) => (@this & 1) == 0;

		/// <inheritdoc cref="Integer.ContainsBit(Integer, int)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ContainsBit(this long @this, int bitPosition) => (@this >> bitPosition & 1) != 0;

		/// <inheritdoc cref="Integer.FindFirstSet(Integer)"/>
		public static int FindFirstSet(this long @this) => BitOperations.TrailingZeroCount(@this);

		/// <inheritdoc cref="Integer.PopCount(Integer)"/>
		public static int PopCount(this long @this) => BitOperations.PopCount((ulong)@this);

		/// <inheritdoc cref="Integer.GetNextSet(Integer, int)"/>
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

		/// <inheritdoc cref="Integer.SetAt(Integer, int)"/>
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

		/// <inheritdoc cref="Integer.GetAllSets(Integer)"/>
		public static ReadOnlySpan<int> GetAllSets(this long @this)
		{
			if (@this == 0)
			{
				return ReadOnlySpan<int>.Empty;
			}

			int length = @this.PopCount();
			var resultSpan = (stackalloc int[length]);
			for (byte i = 0, p = 0; i < 64; i++, @this >>= 1)
			{
				if (@this.IsOdd())
				{
					resultSpan[p++] = i;
				}
			}

			return new(resultSpan.ToArray());
		}

		/// <inheritdoc cref="Integer.GetEnumerator(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<int>.Enumerator GetEnumerator(this long @this) =>
			@this.GetAllSets().GetEnumerator();

		/// <inheritdoc cref="Integer.ReverseBits(ref Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReverseBits(this ref long @this)
		{
			@this = @this >> 1 & 0x55555555_55555555L | (@this & 0x55555555_55555555L) << 1;
			@this = @this >> 2 & 0x33333333_33333333L | (@this & 0x33333333_33333333L) << 2;
			@this = @this >> 4 & 0x0F0F0F0F_0F0F0F0FL | (@this & 0x0F0F0F0F_0F0F0F0FL) << 4;
			@this = @this >> 8 & 0x00FF00FF_00FF00FFL | (@this & 0x00FF00FF_00FF00FFL) << 8;
			@this = @this >> 16 & 0x0000FFFF_0000FFFFL | (@this & 0x0000FFFF_0000FFFFL) << 16;
			@this = @this >> 32 | @this << 32;
		}
	}
}
