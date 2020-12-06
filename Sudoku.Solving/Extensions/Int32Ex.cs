using System.Extensions;

namespace Sudoku.Solving.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="int"/>.
	/// </summary>
	/// <seealso cref="int"/>
	public static class Int32Ex
	{
		/// <summary>
		/// Get extra difficulty rating for a chain node sequence.
		/// </summary>
		/// <param name="length">(<see langword="this"/> parameter) The length.</param>
		/// <returns>The difficulty.</returns>
		public static decimal GetExtraDifficultyByLength(this int length)
		{
			decimal added = 0;
#if OBSOLETE
			// I have seen the code of Sudoku Explainer.
			// The calculation formula(older one) is:
			int[] steps =
			{
				4, 6, 8, 12, 16, 24, 32, 48, 64, 96, 128,
				192, 256, 384, 512, 768, 1024, 1536, 2048,
				3072, 4096, 6144, 8192
			};
			for (int index = 0; index < steps.Length && length > steps[index]; index++)
			{
				added += .1M;
			}
#else
			int ceil = 4;
			for (bool isOdd = false; length > ceil; isOdd.Flip())
			{
				added += .1M;
				ceil = isOdd ? ceil * 4 / 3 : ceil * 3 / 2;
			}
#endif
			return added;
		}
	}
}
