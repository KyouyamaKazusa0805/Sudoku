using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="short"/>.
	/// </summary>
	/// <seealso cref="short"/>
	[DebuggerStepThrough]
	public static class Int16Ex
	{
		/// <summary>
		/// Find the first offset of set bit of the binary representation of the specified value.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <returns>
		/// An <see cref="int"/> value indicating that. If the value is 0, this method
		/// will always return 0.
		/// </returns>
		/// <seealso cref="Int32Ex.FindFirstSet(int)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FindFirstSet(this short @this) => Int32Ex.FindFirstSet(@this);

		/// <summary>
		/// Get the total number of set bits of the binary representation of a specified value.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <returns>An <see cref="int"/> value indicating that.</returns>
		/// <seealso cref="Int32Ex.CountSet(int)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CountSet(this short @this) => Int32Ex.CountSet(@this);

		/// <summary>
		/// Find a index of the binary representation of a value
		/// after the specified index, whose bit is set <see langword="true"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <param name="index">The index.</param>
		/// <returns>The index.</returns>
		public static int GetNextSetBit(this short @this, int index)
		{
			for (int i = index + 1; i < 16; i++)
			{
				if ((@this & (1 << i)) != 0)
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Get an <see cref="int"/> value, indicating that the absolute position of
		/// all set bits with the specified set bit order.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <param name="order">The number of the order of set bits.</param>
		/// <returns>The position.</returns>
		public static int GetSetBitIndex(this short @this, int order)
		{
			for (int i = 0, count = 0; i < 16; i++, @this >>= 1)
			{
				if ((@this & 1) != 0 && ++count == order)
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Find all offsets of set bits of the binary representation of a specified value.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
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

		/// <summary>
		/// <para>Reverse all bits in a specified value.</para>
		/// <para>
		/// Note that the value is passed by <b>reference</b> though the
		/// method is an extension method, and returns nothing.
		/// </para>
		/// </summary>
		/// <param name="this">(<see langword="this ref"/> parameter) The value.</param>
		public static void ReverseBits(this ref short @this)
		{
			@this = (short)(((@this >> 1) & 0x5555) | ((@this & 0x5555) << 1));
			@this = (short)(((@this >> 2) & 0x3333) | ((@this & 0x3333) << 2));
			@this = (short)(((@this >> 4) & 0x0f0f) | ((@this & 0x0f0f) << 4));
			@this = (short)((@this >> 8) | (@this << 8));
		}
	}
}
