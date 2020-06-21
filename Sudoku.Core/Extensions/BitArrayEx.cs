using System.Collections;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="BitArray"/>.
	/// </summary>
	/// <seealso cref="BitArray"/>
	public static class BitArrayEx
	{
		/// <summary>
		/// Get the cardinality of the specified <see cref="BitArray"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The bit array.</param>
		/// <returns>The cardinality value.</returns>
		/// <remarks>
		/// For more information, please visit
		/// <a href="http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel">this link</a>.
		/// </remarks>
		public static int GetCardinality(this BitArray @this)
		{
			int count = 0;
			int[] ints = new int[(@this.Count >> 5) + 1];
			@this.CopyTo(ints, 0);
			foreach (int @int in ints)
			{
				count += @int.CountSet();
			}

			return count;
		}
	}
}
