namespace Sudoku.Analytics;

public partial class Hub
{
	/// <summary>
	/// Provides a way to determine whether a fish pattern is Sashimi or not.
	/// </summary>
	public static class Sashimi
	{
		/// <summary>
		/// Check whether the fish is sashimi.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="baseSets">The base sets.</param>
		/// <param name="fins">All fins.</param>
		/// <param name="digit">The digit.</param>
		/// <returns>
		/// <para>A <see cref="bool"/> value indicating that.</para>
		/// </returns>
		[Cached]
		public static bool? IsSashimi(in Grid grid, House[] baseSets, in CellMap fins, Digit digit)
		{
			if (!fins)
			{
				return null;
			}

			// VARIABLE_DECLARATION_BEGIN
			_ = grid is { CandidatesMap: var __CandidatesMap };
			// VARIABLE_DECLARATION_END

			var isSashimi = false;
			foreach (var baseSet in baseSets)
			{
				if ((HousesMap[baseSet] & ~fins & __CandidatesMap[digit]).Count == 1)
				{
					isSashimi = true;
					break;
				}
			}
			return isSashimi;
		}
	}
}
