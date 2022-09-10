namespace Sudoku.Solving.Prototypes;

/// <summary>
/// Defines a steps searcher that searches for cell-linking loop steps.
/// </summary>
public partial interface ICellLinkingLoopStepSearcher : IStepSearcher
{
	/// <summary>
	/// Try to gather all possible loops which should satisfy the specified condition.
	/// </summary>
	/// <param name="digit">The digit used.</param>
	/// <returns>
	/// Returns a list of array of candidates used in the loop, as the data of possible found loops.
	/// </returns>
	protected static sealed unsafe GuardianDataInfo[] GatherGuardianLoops(int digit)
	{
		delegate*<in CellMap, bool> condition = &GuardianOrBivalueOddagonSatisfyingPredicate;

		var result = new List<GuardianDataInfo>();
		foreach (var cell in CandidatesMap[digit])
		{
			DepthFirstSearching_Guardian(cell, cell, 0, CellMap.Empty + cell, CellMap.Empty, digit, condition, result);
		}

		return result.Distinct().ToArray();
	}

	/// <summary>
	/// Try to gather all possible loops being used in technique unique loops,
	/// which should satisfy the specified condition.
	/// </summary>
	/// <param name="digitsMask">The digits used.</param>
	/// <returns>
	/// Returns a list of array of candidates used in the loop, as the data of possible found loops.
	/// </returns>
	protected static sealed unsafe UniqueLoopDataInfo[] GatherUniqueLoops(short digitsMask)
	{
		delegate*<in CellMap, bool> condition = &UniqueLoopSatisfyingPredicate;

		var result = new List<UniqueLoopDataInfo>();
		int d1 = TrailingZeroCount(digitsMask), d2 = digitsMask.GetNextSet(d1);

		// This limitation will miss the incomplete structures, I may modify it later.
		foreach (var cell in CandidatesMap[d1] & CandidatesMap[d2])
		{
			DepthFirstSearching_UniqueLoop(
				cell, cell, 0, CellMap.Empty + cell, digitsMask,
				CandidatesMap[d1] & CandidatesMap[d2], condition, result);
		}

		return result.Distinct().ToArray();
	}

	/// <summary>
	/// Try to gather all possible loops being used in technique bi-value oddagons,
	/// which should satisfy the specified condition.
	/// </summary>
	/// <param name="digitsMask">The digits used.</param>
	/// <returns>
	/// Returns a list of array of candidates used in the loop, as the data of possible found loops.
	/// </returns>
	protected static sealed unsafe BivalueOddagonDataInfo[] GatherBivalueOddagons(short digitsMask)
	{
		delegate*<in CellMap, bool> condition = &GuardianOrBivalueOddagonSatisfyingPredicate;

		var result = new List<BivalueOddagonDataInfo>();
		int d1 = TrailingZeroCount(digitsMask), d2 = digitsMask.GetNextSet(d1);

		// This limitation will miss the incomplete structures, I may modify it later.
		foreach (var cell in CandidatesMap[d1] & CandidatesMap[d2])
		{
			DepthFirstSearching_BivalueOddagon(
				cell, cell, 0, CellMap.Empty + cell, digitsMask,
				CandidatesMap[d1] & CandidatesMap[d2], condition, result);
		}

		return result.Distinct().ToArray();
	}

	/// <summary>
	/// Defines a templating method that can determine whether a loop is a valid unique loop.
	/// </summary>
	/// <param name="loop">The loop to be checked.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	private static bool UniqueLoopSatisfyingPredicate(in CellMap loop)
		=> loop is { Count: var length, RowMask: var r, ColumnMask: var c, BlockMask: var b }
		&& (length & 1) == 0 && length >= 6
		&& length >> 1 is var halfLength
		&& PopCount((uint)r) == halfLength && PopCount((uint)c) == halfLength && PopCount((uint)b) == halfLength;

	/// <summary>
	/// Defines a templating method that can determine whether a loop is a valid bi-value oddagon.
	/// </summary>
	/// <param name="loop">The loop to be checked.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	private static bool GuardianOrBivalueOddagonSatisfyingPredicate(in CellMap loop)
		=> loop.Count is var l && (l & 1) != 0 && l >= 5;


	static unsafe partial void DepthFirstSearching_Guardian(int startCell, int lastCell, int lastHouse, scoped in CellMap currentLoop, scoped in CellMap currentGuardians, int digit, delegate*<in CellMap, bool> condition, List<GuardianDataInfo> result);
	static unsafe partial void DepthFirstSearching_UniqueLoop(int startCell, int lastCell, int lastHouse, scoped in CellMap currentLoop, short digitsMask, scoped in CellMap fullCells, delegate*<in CellMap, bool> condition, List<UniqueLoopDataInfo> result);
	static unsafe partial void DepthFirstSearching_BivalueOddagon(int startCell, int lastCell, int lastHouse, scoped in CellMap currentLoop, short digitsMask, scoped in CellMap fullCells, delegate*<in CellMap, bool> condition, List<BivalueOddagonDataInfo> result);
}

partial interface ICellLinkingLoopStepSearcher
{
	/// <summary>
	/// Checks for guardian loops using recursion.
	/// </summary>
	static unsafe partial void DepthFirstSearching_Guardian(
		int startCell, int lastCell, int lastHouse, scoped in CellMap currentLoop,
		scoped in CellMap currentGuardians, int digit, delegate*<in CellMap, bool> condition,
		List<GuardianDataInfo> result)
	{
		foreach (var houseType in HouseTypes)
		{
			var house = lastCell.ToHouseIndex(houseType);
			if ((lastHouse >> house & 1) != 0)
			{
				continue;
			}

			var cellsToBeChecked = CandidatesMap[digit] & HousesMap[house];
			if (cellsToBeChecked.Count < 2 || (currentLoop & HousesMap[house]).Count > 2)
			{
				continue;
			}

			foreach (var tempCell in cellsToBeChecked)
			{
				if (tempCell == lastCell)
				{
					continue;
				}

				var housesUsed = 0;
				foreach (var tempHouseType in HouseTypes)
				{
					if (tempCell.ToHouseIndex(tempHouseType) == lastCell.ToHouseIndex(tempHouseType))
					{
						housesUsed |= 1 << lastCell.ToHouseIndex(tempHouseType);
					}
				}

				var tempGuardians = (CandidatesMap[digit] & HousesMap[house]) - tempCell - lastCell;
				if (tempCell == startCell && condition(currentLoop)
					&& (+(currentGuardians | tempGuardians) & CandidatesMap[digit]) is not [])
				{
					result.Add(new(currentLoop, currentGuardians | tempGuardians, digit));

					// Exit the current of this recursion frame.
					return;
				}

				if ((currentLoop | currentGuardians).Contains(tempCell)
					|| (+(currentGuardians | tempGuardians) & CandidatesMap[digit]) is []
					|| (HousesMap[house] & currentLoop).Count > 1)
				{
					continue;
				}

				DepthFirstSearching_Guardian(
					startCell, tempCell, lastHouse | housesUsed, currentLoop + tempCell,
					currentGuardians | tempGuardians, digit, condition, result);
			}
		}
	}

	/// <summary>
	/// Checks for unique loops using recursion.
	/// </summary>
	static unsafe partial void DepthFirstSearching_UniqueLoop(
		int startCell, int lastCell, int lastHouse, scoped in CellMap currentLoop, short digitsMask,
		scoped in CellMap fullCells, delegate*<in CellMap, bool> condition, List<UniqueLoopDataInfo> result)
	{
		foreach (var houseType in HouseTypes)
		{
			var house = lastCell.ToHouseIndex(houseType);
			if ((lastHouse >> house & 1) != 0)
			{
				continue;
			}

			var cellsToBeChecked = fullCells & HousesMap[house];
			if (cellsToBeChecked.Count < 2 || (currentLoop & HousesMap[house]).Count > 2)
			{
				continue;
			}

			foreach (var tempCell in cellsToBeChecked)
			{
				if (tempCell == lastCell)
				{
					continue;
				}

				var housesUsed = 0;
				foreach (var tempHouseType in HouseTypes)
				{
					if (tempCell.ToHouseIndex(tempHouseType) == lastCell.ToHouseIndex(tempHouseType))
					{
						housesUsed |= 1 << lastCell.ToHouseIndex(tempHouseType);
					}
				}

				if (tempCell == startCell && condition(currentLoop))
				{
					result.Add(new(currentLoop, digitsMask));

					// Exit the current of this recursion frame.
					return;
				}

				if ((HousesMap[house] & currentLoop).Count > 1)
				{
					continue;
				}

				DepthFirstSearching_UniqueLoop(
					startCell, tempCell, lastHouse | housesUsed, currentLoop + tempCell,
					digitsMask, fullCells, condition, result);
			}
		}
	}

	/// <summary>
	/// Checks for bi-value oddagon loops using recursion.
	/// </summary>
	static unsafe partial void DepthFirstSearching_BivalueOddagon(
		int startCell, int lastCell, int lastHouse, scoped in CellMap currentLoop, short digitsMask,
		scoped in CellMap fullCells, delegate*<in CellMap, bool> condition, List<BivalueOddagonDataInfo> result)
	{
		foreach (var houseType in HouseTypes)
		{
			var house = lastCell.ToHouseIndex(houseType);
			if ((lastHouse >> house & 1) != 0)
			{
				continue;
			}

			var cellsToBeChecked = fullCells & HousesMap[house];
			if (cellsToBeChecked.Count < 2 || (currentLoop & HousesMap[house]).Count > 2)
			{
				continue;
			}

			foreach (var tempCell in cellsToBeChecked)
			{
				if (tempCell == lastCell)
				{
					continue;
				}

				var housesUsed = 0;
				foreach (var tempHouseType in HouseTypes)
				{
					if (tempCell.ToHouseIndex(tempHouseType) == lastCell.ToHouseIndex(tempHouseType))
					{
						housesUsed |= 1 << lastCell.ToHouseIndex(tempHouseType);
					}
				}

				if (tempCell == startCell && condition(currentLoop))
				{
					result.Add(new(currentLoop, digitsMask));

					// Exit the current of this recursion frame.
					return;
				}

				if ((HousesMap[house] & currentLoop).Count > 1)
				{
					continue;
				}

				DepthFirstSearching_BivalueOddagon(
					startCell, tempCell, lastHouse | housesUsed, currentLoop + tempCell,
					digitsMask, fullCells, condition, result);
			}
		}
	}
}
