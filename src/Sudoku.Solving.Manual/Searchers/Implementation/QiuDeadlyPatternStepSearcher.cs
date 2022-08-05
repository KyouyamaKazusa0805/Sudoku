namespace Sudoku.Solving.Manual.Searchers;

[StepSearcher]
internal sealed unsafe partial class QiuDeadlyPatternStepSearcher : IQiuDeadlyPatternStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		for (int i = 0, length = IQiuDeadlyPatternStepSearcher.Patterns.Length; i < length; i++)
		{
			bool isRow = i < length >> 1;
			if (
				IQiuDeadlyPatternStepSearcher.Patterns[i] is not
				{
					Pair: [var pairFirst, var pairSecond] pair,
					Square: var square,
					BaseLine: var baseLine
				} pattern
			)
			{
				continue;
			}

			// To check whether both two pair cells are empty.
			if (!EmptyCells.Contains(pairFirst) || !EmptyCells.Contains(pairSecond))
			{
				continue;
			}

			// Step 1: To determine whether the distinction degree of base line is 1.
			short appearedDigitsMask = 0, distinctionMask = 0;
			int appearedParts = 0;
			for (int j = 0, house = isRow ? 18 : 9; j < 9; j++, house++)
			{
				var houseMap = HouseMaps[house];
				if ((baseLine & houseMap) is { Count: not 0 } tempMap)
				{
					f(grid, tempMap, ref appearedDigitsMask, ref distinctionMask, ref appearedParts);
				}
				else if ((square & houseMap) is { Count: not 0 } squareMap)
				{
					// Don't forget to record the square cells.
					f(grid, squareMap, ref appearedDigitsMask, ref distinctionMask, ref appearedParts);
				}


				static void f(
					scoped in Grid grid, scoped in Cells map, scoped ref short appearedDigitsMask,
					scoped ref short distinctionMask, scoped ref int appearedParts)
				{
					bool flag = false;
					int[] offsets = map.ToArray();
					int c1 = offsets[0], c2 = offsets[1];
					if (!EmptyCells.Contains(c1))
					{
						int d1 = grid[c1];
						distinctionMask ^= (short)(1 << d1);
						appearedDigitsMask |= (short)(1 << d1);

						flag = true;
					}
					if (!EmptyCells.Contains(c2))
					{
						int d2 = grid[c2];
						distinctionMask ^= (short)(1 << d2);
						appearedDigitsMask |= (short)(1 << d2);

						flag = true;
					}

					appearedParts += flag ? 1 : 0;
				}
			}

			if (!IsPow2(distinctionMask) || appearedParts != PopCount((uint)appearedDigitsMask))
			{
				continue;
			}

			short pairMask = grid.GetDigitsUnion(pair);

			// Iterate on each combination.
			for (int size = 2, count = PopCount((uint)pairMask); size < count; size++)
			{
				foreach (int[] digits in pairMask.GetAllSets().GetSubsets(size))
				{
					// Step 2: To determine whether the digits in pair cells
					// will only appears in square cells.
					var tempMap = Cells.Empty;
					foreach (int digit in digits)
					{
						tempMap |= CandidatesMap[digit];
					}
					var appearingMap = tempMap & square;
					if (appearingMap.Count != 4)
					{
						continue;
					}

					bool flag = false;
					foreach (int digit in digits)
					{
						if ((square & CandidatesMap[digit]) is [])
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						continue;
					}

					short comparer = 0;
					foreach (int digit in digits)
					{
						comparer |= (short)(1 << digit);
					}
					short otherDigitsMask = (short)(pairMask & ~comparer);
					if (appearingMap == (tempMap & HouseMaps[TrailingZeroCount(square.BlockMask)]))
					{
						// Qdp forms.
						// Now check each type.
						if (CheckType1(accumulator, grid, isRow, pair, square, baseLine, pattern, comparer, otherDigitsMask, onlyFindOne) is { } type1Step)
						{
							return type1Step;
						}
						if (CheckType2(accumulator, grid, isRow, pair, square, baseLine, pattern, comparer, otherDigitsMask, onlyFindOne) is { } type2Step)
						{
							return type2Step;
						}
						if (CheckType3(accumulator, grid, isRow, pair, square, baseLine, pattern, comparer, otherDigitsMask, onlyFindOne) is { } type3Step)
						{
							return type3Step;
						}
					}
				}
			}

			if (CheckType4(accumulator, isRow, pair, square, baseLine, pattern, pairMask, onlyFindOne) is { } type4Step)
			{
				return type4Step;
			}
			if (CheckLockedType(accumulator, grid, isRow, pair, square, baseLine, pattern, pairMask, onlyFindOne) is { } typeLockedStep)
			{
				return typeLockedStep;
			}
		}

		return null;
	}

	private static IStep? CheckType1(
		ICollection<IStep> accumulator, scoped in Grid grid, bool isRow, scoped in Cells pair,
		scoped in Cells square, scoped in Cells baseLine, scoped in QiuDeadlyPattern pattern,
		short comparer, short otherDigitsMask, bool onlyFindOne)
	{
		if (!IsPow2(otherDigitsMask))
		{
			return null;
		}

		int extraDigit = TrailingZeroCount(otherDigitsMask);
		var map = pair & CandidatesMap[extraDigit];
		if (map is not [var elimCell])
		{
			return null;
		}

		short mask = (short)(grid.GetCandidates(elimCell) & ~(1 << extraDigit));
		if (mask == 0)
		{
			return null;
		}

		var conclusions = new List<Conclusion>();
		foreach (int digit in mask)
		{
			conclusions.Add(new(ConclusionType.Elimination, elimCell, digit));
		}

		var cellsMap = square | pair;
		var cellOffsets = new CellViewNode[cellsMap.Count];
		int i = 0;
		foreach (int cell in cellsMap)
		{
			cellOffsets[i++] = new(DisplayColorKind.Normal, cell);
		}

		var candidateOffsets = new List<CandidateViewNode>();
		foreach (int digit in comparer)
		{
			foreach (int cell in square & CandidatesMap[digit])
			{
				candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
			}
		}
		int anotherCellInPair = (pair - map)[0];
		foreach (int digit in grid.GetCandidates(anotherCellInPair))
		{
			candidateOffsets.Add(new(DisplayColorKind.Normal, anotherCellInPair * 9 + digit));
		}

		short lineMask = isRow ? baseLine.RowMask : baseLine.ColumnMask;
		int offset = isRow ? 9 : 18;
		var step = new QiuDeadlyPatternType1Step(
			ImmutableArray.CreateRange(conclusions),
			ImmutableArray.Create(
				View.Empty
					| cellOffsets
					| candidateOffsets
					| from pos in lineMask.GetAllSets() select new HouseViewNode(DisplayColorKind.Normal, pos + offset)
			),
			pattern,
			elimCell * 9 + extraDigit
		);
		if (onlyFindOne)
		{
			return step;
		}

		accumulator.Add(step);

		return null;
	}

	private static IStep? CheckType2(
		ICollection<IStep> accumulator, scoped in Grid grid, bool isRow,
		scoped in Cells pair, scoped in Cells square, scoped in Cells baseLine, scoped in QiuDeadlyPattern pattern,
		short comparer, short otherDigitsMask, bool onlyFindOne)
	{
		if (!IsPow2(otherDigitsMask))
		{
			return null;
		}

		int extraDigit = TrailingZeroCount(otherDigitsMask);
		var map = pair & CandidatesMap[extraDigit];
		if ((!map & CandidatesMap[extraDigit]) is not { Count: not 0 } elimMap)
		{
			return null;
		}

		var conclusions = new List<Conclusion>();
		foreach (int cell in elimMap)
		{
			conclusions.Add(new(ConclusionType.Elimination, cell, extraDigit));
		}

		var cellsMap = square | pair;
		var cellOffsets = new CellViewNode[cellsMap.Count];
		int i = 0;
		foreach (int cell in cellsMap)
		{
			cellOffsets[i++] = new(DisplayColorKind.Normal, cell);
		}
		var candidateOffsets = new List<CandidateViewNode>();
		foreach (int digit in comparer)
		{
			foreach (int cell in square & CandidatesMap[digit])
			{
				candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
			}
		}
		foreach (int cell in pair)
		{
			foreach (int digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add(
					new(digit == extraDigit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, cell * 9 + digit)
				);
			}
		}

		short lineMask = isRow ? baseLine.RowMask : baseLine.ColumnMask;
		int offset = isRow ? 9 : 18;
		var step = new QiuDeadlyPatternType2Step(
			ImmutableArray.CreateRange(conclusions),
			ImmutableArray.Create(
				View.Empty
					| cellOffsets
					| candidateOffsets
					| from pos in lineMask.GetAllSets() select new HouseViewNode(DisplayColorKind.Normal, pos + offset)
			),
			pattern,
			extraDigit
		);
		if (onlyFindOne)
		{
			return step;
		}

		accumulator.Add(step);

		return null;
	}

	private static IStep? CheckType3(
		ICollection<IStep> accumulator, scoped in Grid grid, bool isRow,
		scoped in Cells pair, scoped in Cells square, scoped in Cells baseLine,
		scoped in QiuDeadlyPattern pattern, short comparer, short otherDigitsMask, bool onlyFindOne)
	{
		foreach (int houseIndex in pair.CoveredHouses)
		{
			var allCellsMap = (HouseMaps[houseIndex] & EmptyCells) - pair;
			for (
				int size = PopCount((uint)otherDigitsMask) - 1, length = allCellsMap.Count;
				size < length;
				size++
			)
			{
				foreach (var cells in allCellsMap & size)
				{
					short mask = grid.GetDigitsUnion(cells);
					if ((mask & comparer) != comparer || PopCount((uint)mask) != size + 1)
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					foreach (int digit in mask)
					{
						foreach (int cell in allCellsMap - cells & CandidatesMap[digit])
						{
							conclusions.Add(new(ConclusionType.Elimination, cell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var cellsMap = square | pair;
					var cellOffsets = new CellViewNode[cellsMap.Count];
					int i = 0;
					foreach (int cell in cellsMap)
					{
						cellOffsets[i++] = new(DisplayColorKind.Normal, cell);
					}
					var candidateOffsets = new List<CandidateViewNode>();
					foreach (int digit in comparer)
					{
						foreach (int cell in square & CandidatesMap[digit])
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
						}
					}
					foreach (int cell in pair)
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

					short lineMask = isRow ? baseLine.RowMask : baseLine.ColumnMask;
					int offset = isRow ? 9 : 18;
					var step = new QiuDeadlyPatternType3Step(
						ImmutableArray.CreateRange(conclusions),
						ImmutableArray.Create(
							View.Empty
								| cellOffsets
								| candidateOffsets
								| from pos in lineMask.GetAllSets() select new HouseViewNode(DisplayColorKind.Normal, pos + offset)
						),
						pattern,
						mask,
						cells,
						true
					);
					if (onlyFindOne)
					{
						return step;
					}

					accumulator.Add(step);
				}
			}
		}

		return null;
	}

	private static IStep? CheckType4(
		ICollection<IStep> accumulator, bool isRow, scoped in Cells pair, scoped in Cells square,
		scoped in Cells baseLine, scoped in QiuDeadlyPattern pattern, short comparer, bool onlyFindOne)
	{
		foreach (int houseIndex in pair.CoveredHouses)
		{
			foreach (int digit in comparer)
			{
				if ((CandidatesMap[digit] & HouseMaps[houseIndex]) != pair)
				{
					continue;
				}

				short otherDigitsMask = (short)(comparer & ~(1 << digit));
				bool flag = false;
				foreach (int d in otherDigitsMask)
				{
					if ((ValuesMap[d] & HouseMaps[houseIndex]) is not []
						|| (HouseMaps[houseIndex] & CandidatesMap[d]) != square)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					continue;
				}

				int elimDigit = TrailingZeroCount(comparer & ~(1 << digit));
				var elimMap = pair & CandidatesMap[elimDigit];
				if (elimMap is [])
				{
					continue;
				}

				var conclusions = new List<Conclusion>();
				foreach (int cell in elimMap)
				{
					conclusions.Add(new(ConclusionType.Elimination, cell, elimDigit));
				}

				var cellsMap = square | pair;
				var cellOffsets = new CellViewNode[cellsMap.Count];
				int i = 0;
				foreach (int cell in cellsMap)
				{
					cellOffsets[i++] = new(DisplayColorKind.Normal, cell);
				}
				var candidateOffsets = new List<CandidateViewNode>();
				foreach (int d in comparer)
				{
					foreach (int cell in square & CandidatesMap[d])
					{
						candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + d));
					}
				}
				foreach (int cell in pair)
				{
					candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
				}

				short lineMask = isRow ? baseLine.RowMask : baseLine.ColumnMask;
				int offset = isRow ? 9 : 18;
				var step = new QiuDeadlyPatternType4Step(
					ImmutableArray.CreateRange(conclusions),
					ImmutableArray.Create(
						View.Empty
							| cellOffsets
							| candidateOffsets
							| from pos in lineMask.GetAllSets() select new HouseViewNode(DisplayColorKind.Normal, pos + offset)
					),
					pattern,
					new(pair, digit)
				);
				if (onlyFindOne)
				{
					return step;
				}

				accumulator.Add(step);
			}
		}

		return null;
	}

	private static IStep? CheckLockedType(
		ICollection<IStep> accumulator, scoped in Grid grid, bool isRow,
		scoped in Cells pair, scoped in Cells square, scoped in Cells baseLine,
		scoped in QiuDeadlyPattern pattern, short comparer, bool onlyFindOne)
	{
		// Firstly, we should check the cells in the block that the square cells lying on.
		int block = TrailingZeroCount(square.BlockMask);
		var otherCellsMap = (HouseMaps[block] & EmptyCells) - square;
		var tempMap = Cells.Empty;
		scoped var pairDigits = comparer.GetAllSets();

		bool flag = false;
		foreach (int digit in pairDigits)
		{
			if ((ValuesMap[digit] & HouseMaps[block]) is not [])
			{
				flag = true;
				break;
			}

			tempMap |= CandidatesMap[digit];
		}
		if (flag)
		{
			return null;
		}

		otherCellsMap &= tempMap;
		if (otherCellsMap is { Count: 0 or > 5 })
		{
			return null;
		}

		// May be in one house or span two houses. Now we check for this case.
		var candidates = new List<int>();
		foreach (int cell in otherCellsMap)
		{
			foreach (int digit in pairDigits)
			{
				if (grid.Exists(cell, digit) is true)
				{
					candidates.Add(cell * 9 + digit);
				}
			}
		}

		if (!new Candidates(candidates) is not { Count: not 0 } elimMap)
		{
			return null;
		}

		var conclusions = new List<Conclusion>();
		foreach (int candidate in elimMap)
		{
			if (grid.Exists(candidate / 9, candidate % 9) is true)
			{
				conclusions.Add(new(ConclusionType.Elimination, candidate));
			}
		}
		if (conclusions.Count == 0)
		{
			return null;
		}

		var cellsMap = square | pair;
		var cellOffsets = new CellViewNode[cellsMap.Count];
		int i = 0;
		foreach (int cell in cellsMap)
		{
			cellOffsets[i++] = new(DisplayColorKind.Normal, cell);
		}
		var candidateOffsets = new List<CandidateViewNode>();
		foreach (int d in comparer)
		{
			foreach (int cell in square & CandidatesMap[d])
			{
				candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + d));
			}
		}
		foreach (int cell in pair)
		{
			foreach (int digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
			}
		}
		foreach (int candidate in candidates)
		{
			candidateOffsets.Add(new(DisplayColorKind.Auxiliary2, candidate));
		}

		short lineMask = isRow ? baseLine.RowMask : baseLine.ColumnMask;
		int offset = isRow ? 9 : 18;
		var step = new QiuDeadlyPatternLockedTypeStep(
			ImmutableArray.CreateRange(conclusions),
			ImmutableArray.Create(
				View.Empty
					| cellOffsets
					| candidateOffsets
					| from pos in lineMask.GetAllSets() select new HouseViewNode(DisplayColorKind.Normal, pos + offset)
			),
			pattern,
			candidates
		);
		if (onlyFindOne)
		{
			return step;
		}

		accumulator.Add(step);

		return null;
	}
}
