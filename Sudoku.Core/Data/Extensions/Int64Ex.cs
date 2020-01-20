namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="long"/>.
	/// </summary>
	/// <seealso cref="long"/>
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
	}
}
