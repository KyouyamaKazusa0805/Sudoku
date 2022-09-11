namespace Sudoku.Solving.Logics.Prototypes;

/// <summary>
/// Defines a step searcher that searches for almost locked sets steps.
/// </summary>
public interface IAlmostLockedSetsStepSearcher : IStepSearcher
{
	/// <inheritdoc cref="AlmostLockedSet.Gather(in Grid)"/>
	/// <remarks>
	/// Different with the original method <see cref="AlmostLockedSet.Gather(in Grid)"/>,
	/// this method will only uses the buffer to determine the info, which is unsafe
	/// when calling the method without having initialized those maps in the buffer type,
	/// <see cref="FastProperties"/>.
	/// </remarks>
	/// <seealso cref="AlmostLockedSet"/>
	/// <seealso cref="AlmostLockedSet.Gather(in Grid)"/>
	/// <seealso cref="FastProperties"/>
	protected internal static sealed AlmostLockedSet[] Gather(scoped in Grid grid)
	{
		// Get all bi-value-cell ALSes.
		var result = new List<AlmostLockedSet>();
		foreach (var cell in BivalueCells)
		{
			result.Add(new(grid.GetCandidates(cell), CellMap.Empty + cell, PeersMap[cell] & EmptyCells));
		}

		// Get all non-bi-value-cell ALSes.
		for (var houseIndex = 0; houseIndex < 27; houseIndex++)
		{
			if ((HousesMap[houseIndex] & EmptyCells) is not { Count: >= 3 } tempMap)
			{
				continue;
			}

			for (var size = 2; size <= tempMap.Count - 1; size++)
			{
				foreach (var map in tempMap & size)
				{
					var blockMask = map.BlockMask;
					if (IsPow2(blockMask) && houseIndex >= 9)
					{
						// All ALS cells lying on a box-row or a box-column
						// will be processed as a block ALS.
						continue;
					}

					// Get all candidates in these cells.
					short digitsMask = 0;
					foreach (var cell in map)
					{
						digitsMask |= grid.GetCandidates(cell);
					}
					if (PopCount((uint)digitsMask) - 1 != size)
					{
						continue;
					}

					var coveredLine = map.CoveredLine;
					result.Add(
						new(
							digitsMask,
							map,
							houseIndex < 9 && coveredLine is >= 9 and not InvalidValidOfTrailingZeroCountMethodFallback
								? ((HousesMap[houseIndex] | HousesMap[coveredLine]) & EmptyCells) - map
								: tempMap - map
						)
					);
				}
			}
		}

		return result.ToArray();
	}
}
