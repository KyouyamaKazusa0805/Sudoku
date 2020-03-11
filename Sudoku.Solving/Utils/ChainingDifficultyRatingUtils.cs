namespace Sudoku.Solving.Utils
{
	/// <summary>
	/// Provides extension methods of rating a chain.
	/// </summary>
	public static class ChainingDifficultyRatingUtils
	{
		/// <summary>
		/// Get extra difficulty rating.
		/// </summary>
		/// <param name="length">The length.</param>
		/// <returns>The difficulty.</returns>
		public static decimal GetExtraDifficultyByLength(int length)
		{
			decimal added = 0;
			int ceil = 4;
			for (bool isOdd = false; length > ceil; isOdd = !isOdd)
			{
				added += .1M;
				ceil = isOdd ? (ceil << 2) / 3 : ceil * 3 >> 1;
			}
			return added;

			// I have seen the code of Sudoku Explainer.
			// The calculation formula (older one) is:
			#region Obsolete code
			//int[] steps =
			//{
			//	4, 6, 8, 12, 16, 24, 32, 48, 64, 96, 128,
			//	192, 256, 384, 512, 768, 1024, 1536, 2048,
			//	3072, 4096, 6144, 8192
			//};
			//decimal added = 0;
			//for (int index = 0; index < steps.Length && length > steps[index]; index++)
			//{
			//	added += .1M;
			//}
			//return added;
			#endregion
		}
	}
}
