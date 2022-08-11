namespace Sudoku.Solving.Manual.Searchers;

[StepSearcher]
internal sealed unsafe partial class BivalueOddagonStepSearcher : IBivalueOddagonStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		if (BivalueCells.Count < 4)
		{
			return null;
		}

		var resultAccumulator = new List<BivalueOddagonStep>();
		var loops = new List<(Cells, IEnumerable<LinkViewNode>)>();
		var tempLoop = new List<int>(14);
		var loopMap = Cells.Empty;

		// Now iterate on each bi-value cells as the start cell to get all possible unique loops,
		// making it the start point to execute the recursion.
		IOrderedEnumerable<BivalueOddagonStep> resultList = default!;
		foreach (int cell in BivalueCells)
		{
			short mask = grid.GetCandidates(cell);
			int d1 = TrailingZeroCount(mask), d2 = mask.GetNextSet(d1);

			loopMap.Clear();
			loops.Clear();
			tempLoop.Clear();

			IUniqueLoopOrBivalueOddagonStepSearcher.SearchForPossibleLoopPatterns(
				grid, d1, d2, cell, (HouseType)255, 0, 2, ref loopMap,
				tempLoop, () => isValid(ref loopMap), loops
			);

			if (loops.Count == 0)
			{
				continue;
			}

			short comparer = (short)(1 << d1 | 1 << d2);
			foreach (var (currentLoop, links) in loops)
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
						if (CheckType1(resultAccumulator, d1, d2, currentLoop, links, extraCellsMap, onlyFindOne) is { } step1)
						{
							return step1;
						}

						break;
					}
					default:
					{
						// Type 2, 3.
						// Here use default label to ensure the order of
						// the handling will be 1->2->3.
						if (CheckType2(resultAccumulator, grid, d1, d2, currentLoop, links, extraCellsMap, comparer, onlyFindOne) is { } step2)
						{
							return step2;
						}

						if (extraCellsMap.Count == 2)
						{
							if (CheckType3(resultAccumulator, grid, d1, d2, currentLoop, links, extraCellsMap, comparer, onlyFindOne) is { } step3)
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

			if (onlyFindOne)
			{
				return resultList.First();
			}

			accumulator.AddRange(resultList);
		}

		return null;


		static bool isValid(scoped ref Cells cells)
		{
			foreach (int house in cells.Houses)
			{
				if ((cells & HouseMaps[house]).Count >= 3)
				{
					return false;
				}
			}

			return true;
		}
	}

	private IStep? CheckType1(
		ICollection<BivalueOddagonStep> accumulator, int d1, int d2, scoped in Cells loop,
		IEnumerable<LinkViewNode> links, scoped in Cells extraCellsMap, bool onlyFindOne)
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

		var candidateOffsets = new List<CandidateViewNode>(30);
		foreach (int cell in loop - extraCell)
		{
			candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d1));
			candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d2));
		}

		var step = new BivalueOddagonType1Step(
			ImmutableArray.CreateRange(conclusions),
			ImmutableArray.Create(View.Empty | candidateOffsets | links),
			loop,
			d1,
			d2,
			extraCell
		);

		if (onlyFindOne)
		{
			return step;
		}

		accumulator.Add(step);

	ReturnNull:
		return null;
	}

	private IStep? CheckType2(
		ICollection<BivalueOddagonStep> accumulator, scoped in Grid grid, int d1, int d2, scoped in Cells loop,
		IEnumerable<LinkViewNode> links, scoped in Cells extraCellsMap, short comparer, bool onlyFindOne)
	{
		short mask = (short)(grid.GetDigitsUnion(extraCellsMap) & ~comparer);
		if (!IsPow2(mask))
		{
			goto ReturnNull;
		}

		int extraDigit = TrailingZeroCount(mask);
		if (extraCellsMap % CandidatesMap[extraDigit] is not (var elimMap and not []))
		{
			goto ReturnNull;
		}

		var candidateOffsets = new List<CandidateViewNode>();
		foreach (int cell in loop)
		{
			foreach (int digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add(
					new(digit == extraDigit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, cell * 9 + digit)
				);
			}
		}

		var step = new BivalueOddagonType2Step(
			ImmutableArray.Create(
				from cell in elimMap select new Conclusion(Elimination, cell, extraDigit)
			),
			ImmutableArray.Create(View.Empty | candidateOffsets | links),
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
		ICollection<BivalueOddagonStep> accumulator, scoped in Grid grid, int d1, int d2, scoped in Cells loop,
		IEnumerable<LinkViewNode> links, scoped in Cells extraCellsMap, short comparer, bool onlyFindOne)
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
		foreach (int house in extraCellsMap.CoveredHouses)
		{
			if (((ValuesMap[d1] | ValuesMap[d2]) & HouseMaps[house]) is not [])
			{
				goto ReturnNull;
			}

			var otherCells = (HouseMaps[house] & EmptyCells) - loop;
			for (int size = PopCount((uint)otherDigitsMask) - 1, count = otherCells.Count; size < count; size++)
			{
				foreach (var cells in otherCells & size)
				{
					short mask = grid.GetDigitsUnion(cells);
					if (PopCount((uint)mask) != size + 1 || (mask & otherDigitsMask) != otherDigitsMask)
					{
						continue;
					}

					if ((HouseMaps[house] & EmptyCells) - cells - loop is not (var elimMap and not []))
					{
						continue;
					}

					var conclusions = new List<Conclusion>(16);
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

					var step = new BivalueOddagonType3Step(
						ImmutableArray.CreateRange(conclusions),
						ImmutableArray.Create(
							View.Empty
								| candidateOffsets
								| new HouseViewNode(DisplayColorKind.Normal, house)
								| links
						),
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
