namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher that searches for fish steps.
/// </summary>
public interface IFishStepSearcher : IStepSearcher
{
	/// <summary>
	/// Indicates the maximum size the searcher can search for.
	/// </summary>
	public abstract int MaxSize { get; set; }


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
	/// <description>The fish is a sashimi finned fish.</description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>The fish is a normal finned fish.</description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term>
	/// <description>The fish doesn't contain any fin.</description>
	/// </item>
	/// </list>
	/// </returns>
	protected static sealed bool? IsSashimi(int[] baseSets, scoped in Cells fins, int digit)
	{
		if (fins is [])
		{
			return null;
		}

		bool isSashimi = false;
		foreach (int baseSet in baseSets)
		{
			if ((HouseMaps[baseSet] - fins & CandidatesMap[digit]).Count == 1)
			{
				isSashimi = true;
				break;
			}
		}

		return isSashimi;
	}
}
