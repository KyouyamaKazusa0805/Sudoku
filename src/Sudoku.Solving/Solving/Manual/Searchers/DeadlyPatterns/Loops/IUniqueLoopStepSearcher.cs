using Sudoku.Data;
using static Sudoku.Constants.Tables;

namespace Sudoku.Solving.Manual.Searchers.DeadlyPatterns.Loops;

/// <summary>
/// Defines a step searcher that searches for unique loop steps.
/// </summary>
public interface IUniqueLoopStepSearcher : IDeadlyPatternStepSearcher, IUniqueLoopOrBivalueOddagonStepSearcher
{
	/// <summary>
	/// Checks whether the specified loop of cells is a valid loop.
	/// </summary>
	/// <param name="loopCells">The loop cells.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	protected static bool IsValidLoop(IList<int> loopCells)
	{
		int visitedOddRegions = 0, visitedEvenRegions = 0;
		Unsafe.SkipInit(out bool isOdd);
		foreach (int cell in loopCells)
		{
			foreach (var region in Regions)
			{
				int regionIndex = cell.ToRegionIndex(region);
				if (isOdd)
				{
					if ((visitedOddRegions >> regionIndex & 1) != 0)
					{
						return false;
					}
					else
					{
						visitedOddRegions |= 1 << regionIndex;
					}
				}
				else
				{
					if ((visitedEvenRegions >> regionIndex & 1) != 0)
					{
						return false;
					}
					else
					{
						visitedEvenRegions |= 1 << regionIndex;
					}
				}
			}

			isOdd = !isOdd;
		}

		return visitedEvenRegions == visitedOddRegions;
	}
}
