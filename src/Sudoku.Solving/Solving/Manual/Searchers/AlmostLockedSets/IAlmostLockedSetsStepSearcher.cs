namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher that searches for almost locked sets steps.
/// </summary>
public interface IAlmostLockedSetsStepSearcher : IStepSearcher
{
	/// <summary>
	/// Gathers all possible <see cref="AlmostLockedSet"/>s in the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>All possible found <see cref="AlmostLockedSet"/>.</returns>
	/// <remarks>
	/// Different with the original method <see cref="AlmostLockedSet.Gather(in Grid)"/>,
	/// this method will only uses the buffer to determine the info, which is unsafe
	/// when calling the method without having initialized those maps in the buffer type,
	/// <see cref="FastProperties"/>.
	/// </remarks>
	/// <seealso cref="AlmostLockedSet"/>
	/// <seealso cref="AlmostLockedSet.Gather(in Grid)"/>
	/// <seealso cref="FastProperties"/>
	protected internal static AlmostLockedSet[] Gather(in Grid grid)
	{
		// Get all bi-value-cell ALSes.
		var result = new List<AlmostLockedSet>();
		foreach (int cell in BivalueMap)
		{
			result.Add(new(grid.GetCandidates(cell), Cells.Empty + cell, PeerMaps[cell] & EmptyMap));
		}

		// Get all non-bi-value-cell ALSes.
		for (int region = 0; region < 27; region++)
		{
			if ((RegionMaps[region] & EmptyMap) is not { Count: >= 3 } tempMap)
			{
				continue;
			}

			for (int size = 2; size <= tempMap.Count - 1; size++)
			{
				foreach (var map in tempMap & size)
				{
					short blockMask = map.BlockMask;
					if (IsPow2(blockMask) && region >= 9)
					{
						// All ALS cells lying on a box-row or a box-column
						// will be processed as a block ALS.
						continue;
					}

					// Get all candidates in these cells.
					short digitsMask = 0;
					foreach (int cell in map)
					{
						digitsMask |= grid.GetCandidates(cell);
					}
					if (PopCount((uint)digitsMask) - 1 != size)
					{
						continue;
					}

					int coveredLine = map.CoveredLine;
					result.Add(
						new(
							digitsMask,
							map,
							region < 9 && coveredLine is >= 9 and not InvalidFirstSet
								? ((RegionMaps[region] | RegionMaps[coveredLine]) & EmptyMap) - map
								: tempMap - map
						)
					);
				}
			}
		}

		return result.ToArray();
	}
}
