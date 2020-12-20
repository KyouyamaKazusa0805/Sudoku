using Sudoku.Data;
using static Sudoku.Constants.Processings;

namespace Sudoku.Solving.Manual.Fishes
{
	/// <summary>
	/// Encapsulates a <b>fish</b> technique searcher.
	/// </summary>
	public abstract class FishStepSearcher : StepSearcher
	{
		/// <summary>
		/// Check whether the fish is sashimi one.
		/// </summary>
		/// <param name="baseSets">The base sets.</param>
		/// <param name="fins">(<see langword="in"/> parameter) All fins.</param>
		/// <param name="digit">The digit.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		protected static bool IsSashimi(int[] baseSets, in Cells fins, int digit)
		{
			bool isSashimi = false;
			foreach (int baseSet in baseSets)
			{
				if ((RegionMaps[baseSet] - fins & CandMaps[digit]).Count == 1)
				{
					isSashimi = true;
					break;
				}
			}

			return isSashimi;
		}
	}
}
