using System.Numerics;
using System.Runtime.CompilerServices;
using Sudoku.DocComments;

namespace System.Extensions
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

		/// <inheritdoc cref="Integer.IsOdd(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsOdd(this short @this) => (@this & 1) != 0;

		/// <inheritdoc cref="Integer.IsEven(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsEven(this short @this) => (@this & 1) == 0;

		/// <inheritdoc cref="Integer.ContainsBit(Integer, int)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ContainsBit(this short @this, int bitPosition) => (@this >> bitPosition & 1) != 0;

		/// <inheritdoc cref="Integer.Overlaps(Integer, Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Overlaps(this short @this, short other) => (@this & other) != 0;

		/// <inheritdoc cref="Integer.ExceptOverlaps(Integer, Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ExceptOverlaps(this short @this, short other) => (@this & ~other) != 0;

		/// <inheritdoc cref="Integer.FindFirstSet(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FindFirstSet(this short @this) => BitOperations.TrailingZeroCount(@this);

		/// <inheritdoc cref="Integer.PopCount(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int PopCount(this short @this) => BitOperations.PopCount((uint)@this);

		/// <inheritdoc cref="Integer.GetNextSet(Integer, int)"/>
		public static int GetNextSet(this short @this, int index)
		{
			for (int i = index + 1; i < 16; i++)
			{
				if (@this.ContainsBit(i))
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
				if (@this.IsOdd() && ++count == order)
				{
					return i;
				}
			}

			return -1;
		}

		/// <inheritdoc cref="Integer.GetAllSets(Integer)"/>
		public static ReadOnlySpan<int> GetAllSets(this short @this)
		{
			if (@this == 0)
			{
				return ReadOnlySpan<int>.Empty;
			}

			int length = @this.PopCount();
			var resultSpan = (stackalloc int[length]);
			for (byte i = 0, p = 0; i < 16; i++, @this >>= 1)
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
		public static ReadOnlySpan<int>.Enumerator GetEnumerator(this short @this)
		{
			var span = @this.GetAllSets();
			return span.GetEnumerator();
		}

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
