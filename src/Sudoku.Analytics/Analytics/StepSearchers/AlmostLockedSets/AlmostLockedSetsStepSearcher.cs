using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Metadata;
using Sudoku.Analytics.Steps;
using Sudoku.Rendering;
using Sudoku.Rendering.Nodes;

namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with an <b>Almost Locked Sets</b> step searcher.
/// </summary>
/// <param name="priority">
/// <inheritdoc cref="StepSearcher(int, int, StepSearcherRunningArea)" path="/param[@name='priority']"/>
/// </param>
/// <param name="level">
/// <inheritdoc cref="StepSearcher(int, int, StepSearcherRunningArea)" path="/param[@name='level']"/>
/// </param>
/// <param name="runningArea">
/// <inheritdoc cref="StepSearcher(int, int, StepSearcherRunningArea)" path="/param[@name='runningArea']"/>
/// </param>
public abstract class AlmostLockedSetsStepSearcher(
	int priority,
	int level,
	StepSearcherRunningArea runningArea = StepSearcherRunningArea.Searching | StepSearcherRunningArea.Gathering
) : StepSearcher(priority, level, runningArea)
{
	/// <inheritdoc cref="AlmostLockedSet.Gather(in Grid)"/>
	/// <remarks><b>This method uses <see cref="CachedFields"/>.</b></remarks>
	/// <seealso cref="CachedFields"/>
	protected internal static AlmostLockedSet[] GatherAlmostLockedSets(scoped in Grid @this)
	{
		// Get all bi-value-cell ALSes.
		var result = new List<AlmostLockedSet>();
		foreach (var cell in BivalueCells)
		{
			result.Add(new(@this.GetCandidates(cell), CellsMap[cell], PeersMap[cell] & EmptyCells));
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
				foreach (var map in tempMap.GetSubsets(size))
				{
					var blockMask = map.BlockMask;
					if (IsPow2(blockMask) && houseIndex >= 9)
					{
						// All ALS cells lying on a box-row or a box-column will be processed as a block ALS.
						continue;
					}

					// Get all candidates in these cells.
					var digitsMask = (Mask)0;
					foreach (var cell in map)
					{
						digitsMask |= @this.GetCandidates(cell);
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
							houseIndex < 9 && coveredLine is >= 9 and not InvalidTrailingZeroCountMethodFallback
								? ((HousesMap[houseIndex] | HousesMap[coveredLine]) & EmptyCells) - map
								: tempMap - map
						)
					);
				}
			}
		}

		return [.. result];
	}
}
