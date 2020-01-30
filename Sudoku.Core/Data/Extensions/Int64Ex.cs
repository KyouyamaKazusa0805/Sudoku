using System.Collections.Generic;
using System.Diagnostics;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="long"/>.
	/// </summary>
	/// <seealso cref="long"/>
	[DebuggerStepThrough]
	public static class Int64Ex
	{
		/// <summary>
		/// Get the total number of set bits of the binary representation
		/// of the specified value.
		/// </summary>
		/// <param name="this">The value.</param>
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
		/// <param name="this">The value.</param>
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
	}
}
