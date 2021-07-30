using Sudoku.Data;
using static Sudoku.Constants.Tables;

namespace Sudoku.Solving.Manual
{
	partial class FastProperties
	{
		/// <summary>
		/// Check whether the fish is sashimi one.
		/// </summary>
		/// <param name="baseSets">The base sets.</param>
		/// <param name="fins">All fins.</param>
		/// <param name="digit">The digit.</param>
		/// <returns>
		/// A <see cref="bool"/> value indicating that. All cases are below:
		/// <list type="table">
		/// <item>
		/// <term><see langword="true"/></term>
		/// <description>If the fish is sashimi.</description>
		/// </item>
		/// <item>
		/// <term><see langword="false"/></term>
		/// <description>If the fish is a normal finned fish.</description>
		/// </item>
		/// <item>
		/// <term><see langword="null"/></term>
		/// <description>If the fish doesn't contain any fin.</description>
		/// </item>
		/// </list>
		/// </returns>
		internal static bool? IsSashimi(int[] baseSets, in Cells fins, int digit)
		{
			if (fins.IsEmpty)
			{
				return null;
			}

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
