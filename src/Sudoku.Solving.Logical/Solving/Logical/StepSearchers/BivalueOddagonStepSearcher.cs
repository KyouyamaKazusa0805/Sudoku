namespace Sudoku.Solving.Logical.StepSearchers;

[StepSearcher]
[StepSearcherRunningOptions(StepSearcherRunningOptions.HighMemoryAllocation)]
internal sealed unsafe partial class BivalueOddagonStepSearcher : IBivalueOddagonStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(scoped ref LogicalAnalysisContext context)
	{
		if (BivalueCells.Count < 4)
		{
			return null;
		}

		var resultAccumulator = new List<BivalueOddagonStep>();

		// Now iterate on each bi-value cells as the start cell to get all possible unique loops,
		// making it the start point to execute the recursion.
		scoped ref readonly var grid = ref context.Grid;
		var onlyFindOne = context.OnlyFindOne;
		var resultList = default(IOrderedEnumerable<BivalueOddagonStep>);
		foreach (var cell in BivalueCells)
		{
			var mask = grid.GetCandidates(cell);
			int d1 = TrailingZeroCount(mask), d2 = mask.GetNextSet(d1);
			var comparer = (short)(1 << d1 | 1 << d2);
			var foundData = Cached.GatherBivalueOddagons(comparer);
			if (foundData.Length == 0)
			{
				continue;
			}

			foreach (var (currentLoop, digitsMask) in foundData)
			{
				var extraCellsMap = currentLoop - BivalueCells;
				switch (extraCellsMap.Count)
				{
					case 0:
					{
						throw new InvalidOperationException("The current grid has no solution.");
					}
					case not 1:
					{
						// Type 2, 3.
						// Here use default label to ensure the order of
						// the handling will be 1->2->3.
						if (CheckType2(resultAccumulator, grid, d1, d2, currentLoop, extraCellsMap, comparer, onlyFindOne) is { } step2)
						{
							return step2;
						}

						if (extraCellsMap.Count == 2)
						{
							if (CheckType3(resultAccumulator, grid, d1, d2, currentLoop, extraCellsMap, comparer, onlyFindOne) is { } step3)
							{
								return step3;
							}
						}

						break;
					}
				}
			}

			if (resultAccumulator.Count == 0)
			{
				continue;
			}

			resultList =
				from step in IDistinctableStep<BivalueOddagonStep>.Distinct(resultAccumulator)
				orderby step.Loop.Count, step.TechniqueCode
				select step;

			if (context.OnlyFindOne)
			{
				return resultList.First();
			}

			context.Accumulator.AddRange(resultList);
		}

		return null;
	}

	private IStep? CheckType2(
		ICollection<BivalueOddagonStep> accumulator, scoped in Grid grid, int d1, int d2, scoped in CellMap loop,
		scoped in CellMap extraCellsMap, short comparer, bool onlyFindOne)
	{
		var mask = (short)(grid.GetDigitsUnion(extraCellsMap) & ~comparer);
		if (!IsPow2(mask))
		{
			goto ReturnNull;
		}

		var extraDigit = TrailingZeroCount(mask);
		if (extraCellsMap % CandidatesMap[extraDigit] is not (var elimMap and not []))
		{
			goto ReturnNull;
		}

		var candidateOffsets = new List<CandidateViewNode>();
		foreach (var cell in loop)
		{
			foreach (var digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add(
					new(digit == extraDigit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, cell * 9 + digit)
				);
			}
		}

		var step = new BivalueOddagonType2Step(
			from cell in elimMap select new Conclusion(Elimination, cell, extraDigit),
			new[] { View.Empty | candidateOffsets },
			loop,
			d1,
			d2,
			extraDigit
		);

		if (onlyFindOne)
		{
			return step;
		}

		accumulator.Add(step);

	ReturnNull:
		return null;
	}

	private IStep? CheckType3(
		ICollection<BivalueOddagonStep> accumulator, scoped in Grid grid, int d1, int d2, scoped in CellMap loop,
		scoped in CellMap extraCellsMap, short comparer, bool onlyFindOne)
	{
		var notSatisfiedType3 = false;
		foreach (var cell in extraCellsMap)
		{
			var mask = grid.GetCandidates(cell);
			if ((mask & comparer) == 0 || mask == comparer)
			{
				notSatisfiedType3 = true;
				break;
			}
		}

		if (!extraCellsMap.InOneHouse || notSatisfiedType3)
		{
			goto ReturnNull;
		}

		var m = grid.GetDigitsUnion(extraCellsMap);
		if ((m & comparer) != comparer)
		{
			goto ReturnNull;
		}

		var otherDigitsMask = (short)(m & ~comparer);
		foreach (var house in extraCellsMap.CoveredHouses)
		{
			if ((ValuesMap[d1] | ValuesMap[d2]) & HousesMap[house])
			{
				goto ReturnNull;
			}

			var otherCells = (HousesMap[house] & EmptyCells) - loop;
			for (var size = PopCount((uint)otherDigitsMask) - 1; size < otherCells.Count; size++)
			{
				foreach (var cells in otherCells & size)
				{
					var mask = grid.GetDigitsUnion(cells);
					if (PopCount((uint)mask) != size + 1 || (mask & otherDigitsMask) != otherDigitsMask)
					{
						continue;
					}

					if ((HousesMap[house] & EmptyCells) - cells - loop is not (var elimMap and not []))
					{
						continue;
					}

					var conclusions = new List<Conclusion>(16);
					foreach (var digit in mask)
					{
						foreach (var cell in elimMap & CandidatesMap[digit])
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<CandidateViewNode>();
					foreach (var cell in loop)
					{
						foreach (var digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(
								new(
									(otherDigitsMask >> digit & 1) != 0
										? DisplayColorKind.Auxiliary1
										: DisplayColorKind.Normal,
									cell * 9 + digit
								)
							);
						}
					}
					foreach (var cell in cells)
					{
						foreach (var digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
						}
					}

					var step = new BivalueOddagonType3Step(
						conclusions.ToArray(),
						new[] { View.Empty | candidateOffsets | new HouseViewNode(DisplayColorKind.Normal, house) },
						loop,
						d1,
						d2,
						cells,
						mask
					);

					if (onlyFindOne)
					{
						return step;
					}

					accumulator.Add(step);
				}
			}
		}

	ReturnNull:
		return null;
	}
}

/// <summary>
/// Represents a cached gathering operation set.
/// </summary>
file static unsafe class Cached
{
	/// <summary>
	/// Try to gather all possible loops being used in technique bi-value oddagons,
	/// which should satisfy the specified condition.
	/// </summary>
	/// <param name="digitsMask">The digits used.</param>
	/// <returns>
	/// Returns a list of array of candidates used in the loop, as the data of possible found loops.
	/// </returns>
	public static BivalueOddagon[] GatherBivalueOddagons(short digitsMask)
	{
		delegate*<in CellMap, bool> condition = &GuardianOrBivalueOddagonSatisfyingPredicate;

		var result = new List<BivalueOddagon>();
		var d1 = TrailingZeroCount(digitsMask);
		var d2 = digitsMask.GetNextSet(d1);

		// This limitation will miss the incomplete structures, I may modify it later.
		foreach (var cell in CandidatesMap[d1] & CandidatesMap[d2])
		{
			DepthFirstSearching_BivalueOddagon(cell, cell, 0, CellsMap[cell], digitsMask, CandidatesMap[d1] & CandidatesMap[d2], condition, result);
		}

		return result.Distinct().ToArray();
	}

	/// <summary>
	/// Checks for bi-value oddagon loops using recursion.
	/// </summary>
	private static void DepthFirstSearching_BivalueOddagon(
		int startCell,
		int lastCell,
		int lastHouse,
		scoped in CellMap currentLoop,
		short digitsMask,
		scoped in CellMap fullCells,
		delegate*<in CellMap, bool> condition,
		List<BivalueOddagon> result
	)
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
					digitsMask, fullCells, condition, result
				);
			}
		}
	}

	/// <summary>
	/// Defines a templating method that can determine whether a loop is a valid bi-value oddagon.
	/// </summary>
	/// <param name="loop">The loop to be checked.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	private static bool GuardianOrBivalueOddagonSatisfyingPredicate(scoped in CellMap loop) => loop.Count is var l && (l & 1) != 0 && l >= 5;
}
