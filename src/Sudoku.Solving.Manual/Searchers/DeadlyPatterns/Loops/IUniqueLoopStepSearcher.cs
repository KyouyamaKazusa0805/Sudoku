namespace Sudoku.Solving.Manual.Searchers;

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
		int visitedOddHouses = 0, visitedEvenHouses = 0;

		Unsafe.SkipInit(out bool isOdd);
		foreach (int cell in loopCells)
		{
			foreach (var houseType in Houses)
			{
				int houseIndex = cell.ToHouseIndex(houseType);
				if (isOdd)
				{
					if ((visitedOddHouses >> houseIndex & 1) != 0)
					{
						return false;
					}
					else
					{
						visitedOddHouses |= 1 << houseIndex;
					}
				}
				else
				{
					if ((visitedEvenHouses >> houseIndex & 1) != 0)
					{
						return false;
					}
					else
					{
						visitedEvenHouses |= 1 << houseIndex;
					}
				}
			}

			isOdd = !isOdd;
		}

		return visitedEvenHouses == visitedOddHouses;
	}
}
