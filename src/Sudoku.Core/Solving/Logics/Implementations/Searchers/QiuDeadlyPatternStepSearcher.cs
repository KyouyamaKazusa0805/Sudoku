namespace Sudoku.Solving.Logics.Implementations.Searchers;

[StepSearcher]
[SukakuNotSupported]
internal sealed unsafe partial class QiuDeadlyPatternStepSearcher : IQiuDeadlyPatternStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		for (int i = 0, length = IQiuDeadlyPatternStepSearcher.Patterns.Length; i < length; i++)
		{
			var isRow = i < length >> 1;
			if (IQiuDeadlyPatternStepSearcher.Patterns[i] is not
				{
					Pair: [var pairFirst, var pairSecond] pair,
					Square: var square,
					BaseLine: var baseLine
				} pattern)
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
			var appearedParts = 0;
			for (int j = 0, house = isRow ? 18 : 9; j < 9; j++, house++)
			{
				var houseMap = HousesMap[house];
				if ((baseLine & houseMap) is var tempMap and not [])
				{
					f(grid, tempMap, ref appearedDigitsMask, ref distinctionMask, ref appearedParts);
				}
				else if ((square & houseMap) is var squareMap and not [])
				{
					// Don't forget to record the square cells.
					f(grid, squareMap, ref appearedDigitsMask, ref distinctionMask, ref appearedParts);
				}


				static void f(
					scoped in Grid grid, scoped in CellMap map, scoped ref short appearedDigitsMask,
					scoped ref short distinctionMask, scoped ref int appearedParts)
				{
					var flag = false;
					var offsets = map.ToArray();
					int c1 = offsets[0], c2 = offsets[1];
					if (!EmptyCells.Contains(c1))
					{
						var d1 = grid[c1];
						distinctionMask ^= (short)(1 << d1);
						appearedDigitsMask |= (short)(1 << d1);

						flag = true;
					}
					if (!EmptyCells.Contains(c2))
					{
						var d2 = grid[c2];
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

			var pairMask = grid.GetDigitsUnion(pair);

			// Iterate on each combination.
			for (int size = 2, count = PopCount((uint)pairMask); size < count; size++)
			{
				foreach (var digits in pairMask.GetAllSets().GetSubsets(size))
				{
					// Step 2: To determine whether the digits in pair cells
					// will only appears in square cells.
					var tempMap = CellMap.Empty;
					foreach (var digit in digits)
					{
						tempMap |= CandidatesMap[digit];
					}
					var appearingMap = tempMap & square;
					if (appearingMap.Count != 4)
					{
						continue;
					}

					var flag = false;
					foreach (var digit in digits)
					{
						if (!(square & CandidatesMap[digit]))
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
					foreach (var digit in digits)
					{
						comparer |= (short)(1 << digit);
					}
					var otherDigitsMask = (short)(pairMask & ~comparer);
					if (appearingMap == (tempMap & HousesMap[TrailingZeroCount(square.BlockMask)]))
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
		ICollection<IStep> accumulator, scoped in Grid grid, bool isRow, scoped in CellMap pair,
		scoped in CellMap square, scoped in CellMap baseLine, scoped in QiuDeadlyPattern pattern,
		short comparer, short otherDigitsMask, bool onlyFindOne)
	{
		if (!IsPow2(otherDigitsMask))
		{
			return null;
		}

		var extraDigit = TrailingZeroCount(otherDigitsMask);
		var map = pair & CandidatesMap[extraDigit];
		if (map is not [var elimCell])
		{
			return null;
		}

		var mask = (short)(grid.GetCandidates(elimCell) & ~(1 << extraDigit));
		if (mask == 0)
		{
			return null;
		}

		var conclusions = new List<Conclusion>();
		foreach (var digit in mask)
		{
			conclusions.Add(new(Elimination, elimCell, digit));
		}

		var cellMap = square | pair;
		var cellOffsets = new CellViewNode[cellMap.Count];
		var i = 0;
		foreach (var cell in cellMap)
		{
			cellOffsets[i++] = new(DisplayColorKind.Normal, cell);
		}

		var candidateOffsets = new List<CandidateViewNode>();
		foreach (var digit in comparer)
		{
			foreach (var cell in square & CandidatesMap[digit])
			{
				candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
			}
		}
		var anotherCellInPair = (pair - map)[0];
		foreach (var digit in grid.GetCandidates(anotherCellInPair))
		{
			candidateOffsets.Add(new(DisplayColorKind.Normal, anotherCellInPair * 9 + digit));
		}

		var lineMask = isRow ? baseLine.RowMask : baseLine.ColumnMask;
		var offset = isRow ? 9 : 18;
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
		scoped in CellMap pair, scoped in CellMap square, scoped in CellMap baseLine, scoped in QiuDeadlyPattern pattern,
		short comparer, short otherDigitsMask, bool onlyFindOne)
	{
		if (!IsPow2(otherDigitsMask))
		{
			return null;
		}

		var extraDigit = TrailingZeroCount(otherDigitsMask);
		var map = pair & CandidatesMap[extraDigit];
		if ((map.PeerIntersection & CandidatesMap[extraDigit]) is not (var elimMap and not []))
		{
			return null;
		}

		var conclusions = new List<Conclusion>();
		foreach (var cell in elimMap)
		{
			conclusions.Add(new(Elimination, cell, extraDigit));
		}

		var cellMap = square | pair;
		var cellOffsets = new CellViewNode[cellMap.Count];
		var i = 0;
		foreach (var cell in cellMap)
		{
			cellOffsets[i++] = new(DisplayColorKind.Normal, cell);
		}
		var candidateOffsets = new List<CandidateViewNode>();
		foreach (var digit in comparer)
		{
			foreach (var cell in square & CandidatesMap[digit])
			{
				candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
			}
		}
		foreach (var cell in pair)
		{
			foreach (var digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add(
					new(digit == extraDigit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, cell * 9 + digit)
				);
			}
		}

		var lineMask = isRow ? baseLine.RowMask : baseLine.ColumnMask;
		var offset = isRow ? 9 : 18;
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
		scoped in CellMap pair, scoped in CellMap square, scoped in CellMap baseLine,
		scoped in QiuDeadlyPattern pattern, short comparer, short otherDigitsMask, bool onlyFindOne)
	{
		foreach (var houseIndex in pair.CoveredHouses)
		{
			var allCellsMap = (HousesMap[houseIndex] & EmptyCells) - pair;
			for (
				int size = PopCount((uint)otherDigitsMask) - 1, length = allCellsMap.Count;
				size < length;
				size++
			)
			{
				foreach (var cells in allCellsMap & size)
				{
					var mask = grid.GetDigitsUnion(cells);
					if ((mask & comparer) != comparer || PopCount((uint)mask) != size + 1)
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					foreach (var digit in mask)
					{
						foreach (var cell in allCellsMap - cells & CandidatesMap[digit])
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var cellMap = square | pair;
					var cellOffsets = new CellViewNode[cellMap.Count];
					var i = 0;
					foreach (var cell in cellMap)
					{
						cellOffsets[i++] = new(DisplayColorKind.Normal, cell);
					}
					var candidateOffsets = new List<CandidateViewNode>();
					foreach (var digit in comparer)
					{
						foreach (var cell in square & CandidatesMap[digit])
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
						}
					}
					foreach (var cell in pair)
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

					var lineMask = isRow ? baseLine.RowMask : baseLine.ColumnMask;
					var offset = isRow ? 9 : 18;
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
		ICollection<IStep> accumulator, bool isRow, scoped in CellMap pair, scoped in CellMap square,
		scoped in CellMap baseLine, scoped in QiuDeadlyPattern pattern, short comparer, bool onlyFindOne)
	{
		foreach (var houseIndex in pair.CoveredHouses)
		{
			foreach (var digit in comparer)
			{
				if ((CandidatesMap[digit] & HousesMap[houseIndex]) != pair)
				{
					continue;
				}

				var otherDigitsMask = (short)(comparer & ~(1 << digit));
				var flag = false;
				foreach (var d in otherDigitsMask)
				{
					if ((ValuesMap[d] & HousesMap[houseIndex]) is not []
						|| (HousesMap[houseIndex] & CandidatesMap[d]) != square)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					continue;
				}

				var elimDigit = TrailingZeroCount(comparer & ~(1 << digit));
				var elimMap = pair & CandidatesMap[elimDigit];
				if (!elimMap)
				{
					continue;
				}

				var conclusions = new List<Conclusion>();
				foreach (var cell in elimMap)
				{
					conclusions.Add(new(Elimination, cell, elimDigit));
				}

				var cellMap = square | pair;
				var cellOffsets = new CellViewNode[cellMap.Count];
				var i = 0;
				foreach (var cell in cellMap)
				{
					cellOffsets[i++] = new(DisplayColorKind.Normal, cell);
				}
				var candidateOffsets = new List<CandidateViewNode>();
				foreach (var d in comparer)
				{
					foreach (var cell in square & CandidatesMap[d])
					{
						candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + d));
					}
				}
				foreach (var cell in pair)
				{
					candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
				}

				var lineMask = isRow ? baseLine.RowMask : baseLine.ColumnMask;
				var offset = isRow ? 9 : 18;
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
		scoped in CellMap pair, scoped in CellMap square, scoped in CellMap baseLine,
		scoped in QiuDeadlyPattern pattern, short comparer, bool onlyFindOne)
	{
		// Firstly, we should check the cells in the block that the square cells lying on.
		var block = TrailingZeroCount(square.BlockMask);
		var otherCellsMap = (HousesMap[block] & EmptyCells) - square;
		var tempMap = CellMap.Empty;
		scoped var pairDigits = comparer.GetAllSets();

		var flag = false;
		foreach (var digit in pairDigits)
		{
			if (ValuesMap[digit] && HousesMap[block])
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
		if (otherCellsMap is [] or { Count: > 5 })
		{
			return null;
		}

		// May be in one house or span two houses. Now we check for this case.
		var candidates = new List<int>();
		foreach (var cell in otherCellsMap)
		{
			foreach (var digit in pairDigits)
			{
				if (CandidatesMap[digit].Contains(cell))
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
		foreach (var candidate in elimMap)
		{
			if (CandidatesMap[candidate % 9].Contains(candidate / 9))
			{
				conclusions.Add(new(Elimination, candidate));
			}
		}
		if (conclusions.Count == 0)
		{
			return null;
		}

		var cellMap = square | pair;
		var cellOffsets = new CellViewNode[cellMap.Count];
		var i = 0;
		foreach (var cell in cellMap)
		{
			cellOffsets[i++] = new(DisplayColorKind.Normal, cell);
		}
		var candidateOffsets = new List<CandidateViewNode>();
		foreach (var d in comparer)
		{
			foreach (var cell in square & CandidatesMap[d])
			{
				candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + d));
			}
		}
		foreach (var cell in pair)
		{
			foreach (var digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
			}
		}
		foreach (var candidate in candidates)
		{
			candidateOffsets.Add(new(DisplayColorKind.Auxiliary2, candidate));
		}

		var lineMask = isRow ? baseLine.RowMask : baseLine.ColumnMask;
		var offset = isRow ? 9 : 18;
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
