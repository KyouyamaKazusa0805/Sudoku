﻿namespace Sudoku.Solving.Logical.StepSearchers;

[StepSearcher]
[StepSearcherRunningOptions(StepSearcherRunningOptions.OnlyForStandardSudoku | StepSearcherRunningOptions.HighMemoryAllocation)]
internal sealed unsafe partial class UniqueLoopStepSearcher : IUniqueLoopStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(scoped ref LogicalAnalysisContext context)
	{
		if (BivalueCells.Count < 5)
		{
			return null;
		}

		var resultAccumulator = new List<UniqueLoopStep>();

		// Now iterate on each bi-value cells as the start cell to get all possible unique loops,
		// making it the start point to execute the recursion.
		var resultList = (IOrderedEnumerable<UniqueLoopStep>)null!;
		scoped ref readonly var grid = ref context.Grid;
		var accumulator = context.Accumulator!;
		var onlyFindOne = context.OnlyFindOne;
		foreach (var cell in BivalueCells)
		{
			var mask = grid.GetCandidates(cell);
			int d1 = TrailingZeroCount(mask), d2 = mask.GetNextSet(d1);
			var comparer = (short)(1 << d1 | 1 << d2);

			var foundData = Cached.GatherUniqueLoops(comparer);
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
						// The puzzle is invalid - it doesn't contain any possible correct solution.
						// Although the puzzle is invalid, we can also use other step searchers to solve
						// this strange puzzle.
						return IInvalidStep.Instance;
					}
					case 1:
					{
						if (CheckType1(resultAccumulator, d1, d2, currentLoop, extraCellsMap, onlyFindOne) is { } step1)
						{
							return step1;
						}

						break;
					}
					default:
					{
						// Type 2, 3, 4.
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
							if (CheckType4(resultAccumulator, grid, d1, d2, currentLoop, extraCellsMap, comparer, onlyFindOne) is { } step4)
							{
								return step4;
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

			resultList = from step in IDistinctableStep<UniqueLoopStep>.Distinct(resultAccumulator) orderby step.Loop.Count select step;

			if (onlyFindOne)
			{
				return resultList.First();
			}

			accumulator.AddRange(resultList);
		}

		return null;
	}

	/// <summary>
	/// Check type 1.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="d1">The digit 1.</param>
	/// <param name="d2">The digit 2.</param>
	/// <param name="loop">The loop.</param>
	/// <param name="extraCellsMap">The extra cells map.</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searching for one step is okay.</param>
	/// <returns>The step is worth.</returns>
	private IStep? CheckType1(
		ICollection<UniqueLoopStep> accumulator, int d1, int d2, scoped in CellMap loop,
		scoped in CellMap extraCellsMap, bool onlyFindOne)
	{
		var extraCell = extraCellsMap[0];
		var conclusions = new List<Conclusion>(2);
		if (CandidatesMap[d1].Contains(extraCell))
		{
			conclusions.Add(new(Elimination, extraCell, d1));
		}
		if (CandidatesMap[d2].Contains(extraCell))
		{
			conclusions.Add(new(Elimination, extraCell, d2));
		}
		if (conclusions.Count == 0)
		{
			goto ReturnNull;
		}

		var candidateOffsets = new List<CandidateViewNode>();
		foreach (var cell in loop - extraCell)
		{
			candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d1));
			candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d2));
		}

		var step = new UniqueLoopType1Step(
			conclusions.ToArray(),
			new[] { View.Empty | candidateOffsets },
			d1,
			d2,
			loop
		);

		if (onlyFindOne)
		{
			return step;
		}

		accumulator.Add(step);

	ReturnNull:
		return null;
	}

	/// <summary>
	/// Check type 2.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="d1">The digit 1.</param>
	/// <param name="d2">The digit 2.</param>
	/// <param name="loop">The loop.</param>
	/// <param name="extraCellsMap">The extra cells map.</param>
	/// <param name="comparer">The comparer mask (equals to <c><![CDATA[1 << d1 | 1 << d2]]></c>).</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searching for one step is okay.</param>
	/// <returns>The step is worth.</returns>
	private IStep? CheckType2(
		ICollection<UniqueLoopStep> accumulator, scoped in Grid grid, int d1, int d2, scoped in CellMap loop,
		scoped in CellMap extraCellsMap, short comparer, bool onlyFindOne)
	{
		var mask = (short)(grid.GetDigitsUnion(extraCellsMap) & ~comparer);
		if (!IsPow2(mask))
		{
			return null;
		}

		var extraDigit = TrailingZeroCount(mask);
		var elimMap = extraCellsMap % CandidatesMap[extraDigit];
		if (!elimMap)
		{
			goto ReturnNull;
		}

		var candidateOffsets = new List<CandidateViewNode>();
		foreach (var cell in loop)
		{
			foreach (var digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add(
					new(
						digit == extraDigit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
						cell * 9 + digit
					)
				);
			}
		}

		var step = new UniqueLoopType2Step(
			from cell in elimMap select new Conclusion(Elimination, cell, extraDigit),
			new[] { View.Empty | candidateOffsets },
			d1,
			d2,
			loop,
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

	/// <summary>
	/// Check type 3.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="d1">The digit 1.</param>
	/// <param name="d2">The digit 2.</param>
	/// <param name="loop">The loop.</param>
	/// <param name="extraCellsMap">The extra cells map.</param>
	/// <param name="comparer">The comparer mask (equals to <c><![CDATA[1 << d1 | 1 << d2]]></c>).</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searching for one step is okay.</param>
	/// <returns>The step is worth.</returns>
	private IStep? CheckType3(
		ICollection<UniqueLoopStep> accumulator, scoped in Grid grid, int d1, int d2, scoped in CellMap loop,
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
		foreach (var houseIndex in extraCellsMap.CoveredHouses)
		{
			if ((ValuesMap[d1] || ValuesMap[d2]) && HousesMap[houseIndex])
			{
				continue;
			}

			var otherCells = (HousesMap[houseIndex] & EmptyCells) - loop;
			for (var size = PopCount((uint)otherDigitsMask) - 1; size < otherCells.Count; size++)
			{
				foreach (int[] cells in otherCells & size)
				{
					var mask = grid.GetDigitsUnion(cells);
					if (PopCount((uint)mask) != size + 1 || (mask & otherDigitsMask) != otherDigitsMask)
					{
						continue;
					}

					if ((HousesMap[houseIndex] & EmptyCells) - (CellMap)cells - loop is not (var elimMap and not []))
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
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

					var step = new UniqueLoopType3Step(
						conclusions.ToArray(),
						new[] { View.Empty | candidateOffsets | new HouseViewNode(DisplayColorKind.Normal, houseIndex) },
						d1,
						d2,
						loop,
						mask,
						(CellMap)cells
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

	/// <summary>
	/// Check type 4.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="d1">The digit 1.</param>
	/// <param name="d2">The digit 2.</param>
	/// <param name="loop">The loop.</param>
	/// <param name="extraCellsMap">The extra cells map.</param>
	/// <param name="comparer">The comparer mask (equals to <c><![CDATA[1 << d1 | 1 << d2]]></c>).</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searching for one step is okay.</param>
	/// <returns>The step is worth.</returns>
	private IStep? CheckType4(
		ICollection<UniqueLoopStep> accumulator, scoped in Grid grid, int d1, int d2, scoped in CellMap loop,
		scoped in CellMap extraCellsMap, short comparer, bool onlyFindOne)
	{
		if (!extraCellsMap.InOneHouse)
		{
			goto ReturnNull;
		}

		var digitPairs = stackalloc[] { (d1, d2), (d2, d1) };
		foreach (var houseIndex in extraCellsMap.CoveredHouses)
		{
			for (var digitPairIndex = 0; digitPairIndex < 2; digitPairIndex++)
			{
				var (digit, otherDigit) = digitPairs[digitPairIndex];
				var map = HousesMap[houseIndex] & CandidatesMap[digit];
				if (map != (HousesMap[houseIndex] & loop))
				{
					continue;
				}

				int first = extraCellsMap[0], second = extraCellsMap[1];
				var conclusions = new List<Conclusion>(2);
				if (CandidatesMap[otherDigit].Contains(first))
				{
					conclusions.Add(new(Elimination, first, otherDigit));
				}
				if (CandidatesMap[otherDigit].Contains(second))
				{
					conclusions.Add(new(Elimination, second, otherDigit));
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>();
				foreach (var cell in loop - extraCellsMap)
				{
					foreach (var d in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d));
					}
				}
				foreach (var cell in extraCellsMap)
				{
					candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
				}

				var step = new UniqueLoopType4Step(
					conclusions.ToArray(),
					new[] { View.Empty | candidateOffsets | new HouseViewNode(DisplayColorKind.Normal, houseIndex) },
					d1,
					d2,
					loop,
					new(first, second, digit)
				);

				if (onlyFindOne)
				{
					return step;
				}

				accumulator.Add(step);
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
	/// Try to gather all possible loops being used in technique unique loops,
	/// which should satisfy the specified condition.
	/// </summary>
	/// <param name="digitsMask">The digits used.</param>
	/// <returns>
	/// Returns a list of array of candidates used in the loop, as the data of possible found loops.
	/// </returns>
	public static UniqueLoop[] GatherUniqueLoops(short digitsMask)
	{
		delegate*<in CellMap, bool> condition = &UniqueLoopSatisfyingPredicate;

		var result = new List<UniqueLoop>();
		var d1 = TrailingZeroCount(digitsMask);
		var d2 = digitsMask.GetNextSet(d1);

		// This limitation will miss the incomplete structures, I may modify it later.
		foreach (var cell in CandidatesMap[d1] & CandidatesMap[d2])
		{
			DepthFirstSearching_UniqueLoop(cell, cell, 0, CellsMap[cell], digitsMask, CandidatesMap[d1] & CandidatesMap[d2], condition, result);
		}

		return result.Distinct().ToArray();
	}

	/// <summary>
	/// Checks for unique loops using recursion.
	/// </summary>
	private static void DepthFirstSearching_UniqueLoop(
		int startCell,
		int lastCell,
		int lastHouse,
		scoped in CellMap currentLoop,
		short digitsMask,
		scoped in CellMap fullCells,
		delegate*<in CellMap, bool> condition,
		List<UniqueLoop> result
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

				DepthFirstSearching_UniqueLoop(
					startCell, tempCell, lastHouse | housesUsed, currentLoop + tempCell,
					digitsMask, fullCells, condition, result
				);
			}
		}
	}

	/// <summary>
	/// Defines a templating method that can determine whether a loop is a valid unique loop.
	/// </summary>
	/// <param name="loop">The loop to be checked.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	private static bool UniqueLoopSatisfyingPredicate(scoped in CellMap loop)
		=> loop is { Count: var length, RowMask: var r, ColumnMask: var c, BlockMask: var b }
		&& (length & 1) == 0 && length >= 6
		&& length >> 1 is var halfLength
		&& PopCount((uint)r) == halfLength && PopCount((uint)c) == halfLength && PopCount((uint)b) == halfLength;
}
