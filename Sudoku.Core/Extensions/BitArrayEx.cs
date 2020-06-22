using System.Collections;
using System.Diagnostics;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="BitArray"/>.
	/// </summary>
	/// <seealso cref="BitArray"/>
	[DebuggerStepThrough]
	public static class BitArrayEx
	{
		/// <summary>
		/// Get the cardinality of the specified <see cref="BitArray"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The array.</param>
		/// <returns>The total number of bits set <see langword="true"/>.</returns>
		public static int GetCardinality(this BitArray @this)
		{
			int[] integers = new int[((@this.Length >> 5) + 1)];
			@this.CopyTo(integers, 0);

			int result = 0;
			foreach (int integer in integers)
			{
				result += integer.CountSet();
			}

			return result;
		}
	}
}
