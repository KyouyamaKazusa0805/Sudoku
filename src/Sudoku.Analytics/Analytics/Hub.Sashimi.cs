namespace Sudoku.Analytics;

public partial class Hub
{
	/// <summary>
	/// Provides a way to determine whether a fish pattern is Sashimi or not.
	/// </summary>
	internal static class Sashimi
	{
		/// <summary>
		/// Check whether the fish is sashimi.
		/// </summary>
		/// <param name="baseSets">The base sets.</param>
		/// <param name="fins">All fins.</param>
		/// <param name="digit">The digit.</param>
		/// <returns>
		/// <para>A <see cref="bool"/> value indicating that.</para>
		/// <para>
		/// <inheritdoc cref="FishStep.IsSashimi" path="/remarks"/>
		/// </para>
		/// </returns>
		public static bool? IsSashimi(House[] baseSets, in CellMap fins, Digit digit)
		{
			if (!fins)
			{
				return null;
			}

			var isSashimi = false;
			foreach (var baseSet in baseSets)
			{
				if ((HousesMap[baseSet] & ~fins & CandidatesMap[digit]).Count == 1)
				{
					isSashimi = true;
					break;
				}
			}
			return isSashimi;
		}
	}
}
