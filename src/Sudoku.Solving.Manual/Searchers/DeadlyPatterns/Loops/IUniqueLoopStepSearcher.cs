namespace Sudoku.Solving.Manual.Searchers;

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
public interface IUniqueLoopStepSearcher : IDeadlyPatternStepSearcher, ICellLinkingLoopStepSearcher
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
		foreach (int cell in loopCells)
		{
			foreach (var houseType in HouseTypes)
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

[StepSearcher]
internal sealed unsafe partial class UniqueLoopStepSearcher : IUniqueLoopStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		if (BivalueCells.Count < 5)
		{
			return null;
		}

		var resultAccumulator = new List<UniqueLoopStep>();

		// Now iterate on each bi-value cells as the start cell to get all possible unique loops,
		// making it the start point to execute the recursion.
		IOrderedEnumerable<UniqueLoopStep> resultList = null!;
		foreach (int cell in BivalueCells)
		{
			short mask = grid.GetCandidates(cell);
			int d1 = TrailingZeroCount(mask), d2 = mask.GetNextSet(d1);
			short comparer = (short)(1 << d1 | 1 << d2);

			var foundData = ICellLinkingLoopStepSearcher.GatherUniqueLoops(comparer);
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

			resultList =
				from step in IDistinctableStep<UniqueLoopStep>.Distinct(resultAccumulator)
				orderby step.Loop.Count
				select step;

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
		ICollection<UniqueLoopStep> accumulator, int d1, int d2, scoped in Cells loop,
		scoped in Cells extraCellsMap, bool onlyFindOne)
	{
		int extraCell = extraCellsMap[0];
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
		foreach (int cell in loop - extraCell)
		{
			candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d1));
			candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d2));
		}

		var step = new UniqueLoopType1Step(
			ImmutableArray.CreateRange(conclusions),
			ImmutableArray.Create(View.Empty | candidateOffsets),
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
		ICollection<UniqueLoopStep> accumulator, scoped in Grid grid, int d1, int d2, scoped in Cells loop,
		scoped in Cells extraCellsMap, short comparer, bool onlyFindOne)
	{
		short mask = (short)(grid.GetDigitsUnion(extraCellsMap) & ~comparer);
		if (!IsPow2(mask))
		{
			return null;
		}

		int extraDigit = TrailingZeroCount(mask);
		var elimMap = extraCellsMap % CandidatesMap[extraDigit];
		if (elimMap is [])
		{
			goto ReturnNull;
		}

		var candidateOffsets = new List<CandidateViewNode>();
		foreach (int cell in loop)
		{
			foreach (int digit in grid.GetCandidates(cell))
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
			ImmutableArray.Create(View.Empty | candidateOffsets),
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
		ICollection<UniqueLoopStep> accumulator, scoped in Grid grid, int d1, int d2, scoped in Cells loop,
		scoped in Cells extraCellsMap, short comparer, bool onlyFindOne)
	{
		bool notSatisfiedType3 = false;
		foreach (int cell in extraCellsMap)
		{
			short mask = grid.GetCandidates(cell);
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

		short m = grid.GetDigitsUnion(extraCellsMap);
		if ((m & comparer) != comparer)
		{
			goto ReturnNull;
		}

		short otherDigitsMask = (short)(m & ~comparer);
		foreach (int houseIndex in extraCellsMap.CoveredHouses)
		{
			if (((ValuesMap[d1] | ValuesMap[d2]) & HouseMaps[houseIndex]) is not [])
			{
				continue;
			}

			var otherCells = (HouseMaps[houseIndex] & EmptyCells) - loop;
			for (int size = PopCount((uint)otherDigitsMask) - 1, count = otherCells.Count; size < count; size++)
			{
				foreach (int[] cells in otherCells & size)
				{
					short mask = grid.GetDigitsUnion(cells);
					if (PopCount((uint)mask) != size + 1 || (mask & otherDigitsMask) != otherDigitsMask)
					{
						continue;
					}

					if ((HouseMaps[houseIndex] & EmptyCells) - (Cells)cells - loop is not (var elimMap and not []))
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					foreach (int digit in mask)
					{
						foreach (int cell in elimMap & CandidatesMap[digit])
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<CandidateViewNode>();
					foreach (int cell in loop)
					{
						foreach (int digit in grid.GetCandidates(cell))
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
					foreach (int cell in cells)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
						}
					}

					var step = new UniqueLoopType3Step(
						ImmutableArray.CreateRange(conclusions),
						ImmutableArray.Create(
							View.Empty
								| candidateOffsets
								| new HouseViewNode(DisplayColorKind.Normal, houseIndex)
						),
						d1,
						d2,
						loop,
						mask,
						(Cells)cells
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
		ICollection<UniqueLoopStep> accumulator, scoped in Grid grid, int d1, int d2, scoped in Cells loop,
		scoped in Cells extraCellsMap, short comparer, bool onlyFindOne)
	{
		if (!extraCellsMap.InOneHouse)
		{
			goto ReturnNull;
		}

		var digitPairs = stackalloc[] { (d1, d2), (d2, d1) };
		foreach (int houseIndex in extraCellsMap.CoveredHouses)
		{
			for (int digitPairIndex = 0; digitPairIndex < 2; digitPairIndex++)
			{
				var (digit, otherDigit) = digitPairs[digitPairIndex];
				var map = HouseMaps[houseIndex] & CandidatesMap[digit];
				if (map != (HouseMaps[houseIndex] & loop))
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
				foreach (int cell in loop - extraCellsMap)
				{
					foreach (int d in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d));
					}
				}
				foreach (int cell in extraCellsMap)
				{
					candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
				}

				var step = new UniqueLoopType4Step(
					ImmutableArray.CreateRange(conclusions),
					ImmutableArray.Create(
						View.Empty
							| candidateOffsets
							| new HouseViewNode(DisplayColorKind.Normal, houseIndex)
					),
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
