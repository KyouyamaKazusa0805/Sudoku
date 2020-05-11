using System.Collections.Generic;
using System.Diagnostics;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="long"/>.
	/// </summary>
	/// <seealso cref="long"/>
	[DebuggerStepThrough]
	public static class Int64Ex
	{
		/// <summary>
		/// Indicates whether the specified value is the power of two.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public static bool IsPowerOfTwo(this long @this) => @this != 0 && (@this & (@this - 1)) == 0;

		/// <summary>
		/// Get the total number of set bits of the binary representation
		/// of the specified value.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <returns>An <see cref="int"/> value indicating that.</returns>
		public static int CountSet(this long @this)
		{
			int count;
			for (count = 0; @this != 0; @this &= @this - 1, count++) ;
			return count;
		}

		/// <summary>
		/// Find all offsets of set bits of the binary representation of a specified value.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <returns>All offsets.</returns>
		public static IEnumerable<int> GetAllSets(this long @this)
		{
			for (int i = 0; i < 64; i++, @this >>= 1)
			{
				if ((@this & 1) != 0)
				{
					yield return i;
				}
			}
		}

		/// <summary>
		/// Find a index of the binary representation of a value
		/// after the specified index, whose bit is set <see langword="true"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <param name="index">The index.</param>
		/// <returns>The index.</returns>
		public static int GetNextSet(this long @this, int index)
		{
			for (int i = index + 1; i < 64; i++)
			{
				if ((@this & 1 << i) != 0)
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
		public static int SetAt(this long @this, int order)
		{
			for (int i = 0, count = -1; i < 64; i++, @this >>= 1)
			{
				if ((@this & 1) != 0 && ++count == order)
				{
					return i;
				}
			}

			return -1;
		}
	}
}
