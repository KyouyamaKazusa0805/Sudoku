namespace Sudoku.Solving.Logical.StepSearchers;

/// <summary>
/// Provides with a <b>Unique Loop</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Unique Loop Type 1</item>
/// <item>Unique Loop Type 2</item>
/// <item>Unique Loop Type 3</item>
/// <item>Unique Loop Type 4</item>
/// </list>
/// </summary>
public interface IUniqueLoopStepSearcher : IDeadlyPatternStepSearcher
{
	/// <summary>
	/// Checks whether the specified loop of cells is a valid loop.
	/// </summary>
	/// <param name="loopCells">The loop cells.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[Obsolete("Due to having re-impl'ed the logic, this method becomes useless because the argument type is not compatible with the newer one.", false)]
	protected internal static sealed bool IsValidLoop(IList<int> loopCells)
	{
		int visitedOddHouses = 0, visitedEvenHouses = 0;

		Unsafe.SkipInit(out bool isOdd);
		foreach (var cell in loopCells)
		{
			foreach (var houseType in HouseTypes)
			{
				var houseIndex = cell.ToHouseIndex(houseType);
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
