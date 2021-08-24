namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher that searches for fish steps.
/// </summary>
public interface IFishStepSearcher : IStepSearcher
{
	/// <summary>
	/// Indicates the maximum size the searcher can search for.
	/// </summary>
	int MaxSize { get; set; }


	/// <summary>
	/// Check whether the fish is sashimi.
	/// </summary>
	/// <param name="baseSets">The base sets.</param>
	/// <param name="fins">All fins.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>
	/// A <see cref="bool"/> value indicating that. All cases are as belows:
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
	protected static bool? IsSashimi(int[] baseSets, in Cells fins, int digit)
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
