namespace Sudoku.Solving.Manual.Searchers.DeadlyPatterns.Loops;

/// <summary>
/// Defines a step searcher that searches for unique loop steps.
/// </summary>
public unsafe interface IUniqueLoopStepSearcher : IDeadlyPatternStepSearcher, IUniqueLoopOrBivalueOddagonStepSearcher
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
			for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
			{
				int region = cell.ToRegion(label);
				if (isOdd)
				{
					if ((visitedOddRegions >> region & 1) != 0)
					{
						return false;
					}
					else
					{
						visitedOddRegions |= 1 << region;
					}
				}
				else
				{
					if ((visitedEvenRegions >> region & 1) != 0)
					{
						return false;
					}
					else
					{
						visitedEvenRegions |= 1 << region;
					}
				}
			}

			isOdd = !isOdd;
		}

		return visitedEvenRegions == visitedOddRegions;
	}
}
