using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="byte"/>.
	/// </summary>
	/// <seealso cref="byte"/>
	[DebuggerStepThrough]
	public static class ByteEx
	{
		/// <summary>
		/// Find the first offset of set bit of the binary representation
		/// of the specified value. If the value is 0, this method
		/// will always return -1.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <returns>
		/// An <see cref="int"/> value indicating that.
		/// </returns>
		/// <seealso cref="Int32Ex.FindFirstSet(int)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FindFirstSet(this byte @this) => Int32Ex.FindFirstSet(@this);

		/// <summary>
		/// Get the total number of set bits of the binary representation of a specified value.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <returns>An <see cref="int"/> value indicating that.</returns>
		/// <seealso cref="Int32Ex.CountSet(int)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CountSet(this byte @this) => Int32Ex.CountSet(@this);

		/// <summary>
		/// Find a index of the binary representation of a value
		/// after the specified index, whose bit is set <see langword="true"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <param name="index">The index.</param>
		/// <returns>The index.</returns>
		public static int GetNextSet(this byte @this, int index)
		{
			for (int i = index + 1; i < 8; i++)
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
		public static int GetSetIndex(this byte @this, int order)
		{
			for (int i = 0, count = 0; i < 8; i++, @this >>= 1)
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
		public static IEnumerable<int> GetAllSets(this byte @this)
		{
			for (int i = 0; i < 8; i++, @this >>= 1)
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
		public static void ReverseBits(this ref byte @this)
		{
			@this = (byte)(((@this >> 1) & 0x55) | ((@this & 0x55) << 1));
			@this = (byte)(((@this >> 2) & 0x33) | ((@this & 0x33) << 2));
			@this = (byte)((@this >> 4) | (@this << 4));
		}
	}
}
