namespace Sudoku.Solving.Implementations.Searchers;

[StepSearcher]
[SukakuNotSupported]
internal sealed unsafe partial class UniqueMatrixStepSearcher : IUniqueMatrixStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		foreach (var pattern in IUniqueMatrixStepSearcher.Patterns)
		{
			if ((EmptyCells & pattern) != pattern)
			{
				continue;
			}

			var mask = grid.GetDigitsUnion(pattern);
			if (CheckType1(accumulator, grid, onlyFindOne, pattern, mask) is { } type1Step)
			{
				return type1Step;
			}
			if (CheckType2(accumulator, onlyFindOne, pattern, mask) is { } type2Step)
			{
				return type2Step;
			}
			if (CheckType3(accumulator, grid, onlyFindOne, pattern, mask) is { } type3Step)
			{
				return type3Step;
			}
			if (CheckType4(accumulator, grid, onlyFindOne, pattern, mask) is { } type4Step)
			{
				return type4Step;
			}
		}

		return null;
	}

	private IStep? CheckType1(
		ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne,
		scoped in CellMap pattern, short mask)
	{
		if (PopCount((uint)mask) != 5)
		{
			goto ReturnNull;
		}

		foreach (var digits in mask.GetAllSets().GetSubsets(4))
		{
			short digitsMask = 0;
			foreach (var digit in digits)
			{
				digitsMask |= (short)(1 << digit);
			}

			var extraDigit = TrailingZeroCount(mask & ~digitsMask);
			var extraDigitMap = CandidatesMap[extraDigit] & pattern;
			if (extraDigitMap is not [var elimCell])
			{
				continue;
			}

			var cellMask = grid.GetCandidates(elimCell);
			var elimMask = (short)(cellMask & ~(1 << extraDigit));
			if (elimMask == 0)
			{
				continue;
			}

			var conclusions = new List<Conclusion>(4);
			foreach (var digit in elimMask)
			{
				conclusions.Add(new(Elimination, elimCell, digit));
			}

			var candidateOffsets = new List<CandidateViewNode>();
			foreach (var digit in digits)
			{
				foreach (var cell in pattern - elimCell & CandidatesMap[digit])
				{
					candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
				}
			}

			var step = new UniqueMatrixType1Step(
				ImmutableArray.CreateRange(conclusions),
				ImmutableArray.Create(View.Empty | candidateOffsets),
				pattern,
				digitsMask,
				elimCell * 9 + extraDigit
			);
			if (onlyFindOne)
			{
				return step;
			}
			accumulator.Add(step);
		}

	ReturnNull:
		return null;
	}

	private IStep? CheckType2(
		ICollection<IStep> accumulator, bool onlyFindOne, scoped in CellMap pattern, short mask)
	{
		if (PopCount((uint)mask) != 5)
		{
			goto ReturnNull;
		}

		foreach (var digits in mask.GetAllSets().GetSubsets(4))
		{
			short digitsMask = 0;
			foreach (var digit in digits)
			{
				digitsMask |= (short)(1 << digit);
			}

			var extraDigit = TrailingZeroCount(mask & ~digitsMask);
			if (pattern % CandidatesMap[extraDigit] is not (var elimMap and not []))
			{
				continue;
			}

			var conclusions = new List<Conclusion>(4);
			foreach (var cell in elimMap)
			{
				conclusions.Add(new(Elimination, cell, extraDigit));
			}

			var candidateOffsets = new List<CandidateViewNode>();
			foreach (var digit in digits)
			{
				foreach (var cell in CandidatesMap[digit] & pattern)
				{
					candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
				}
			}
			foreach (var cell in CandidatesMap[extraDigit] & pattern)
			{
				candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + extraDigit));
			}

			var step = new UniqueMatrixType2Step(
				ImmutableArray.CreateRange(conclusions),
				ImmutableArray.Create(View.Empty | candidateOffsets),
				pattern,
				digitsMask,
				extraDigit
			);
			if (onlyFindOne)
			{
				return step;
			}
			accumulator.Add(step);
		}

	ReturnNull:
		return null;
	}

	private IStep? CheckType3(
		ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne,
		scoped in CellMap pattern, short mask)
	{
		foreach (var digits in mask.GetAllSets().GetSubsets(4))
		{
			short digitsMask = 0;
			foreach (var digit in digits)
			{
				digitsMask |= (short)(1 << digit);
			}

			var extraDigitsMask = (short)(mask & ~digitsMask);
			var tempMap = CellMap.Empty;
			foreach (var digit in extraDigitsMask)
			{
				tempMap |= CandidatesMap[digit];
			}
			if (tempMap.InOneHouse)
			{
				continue;
			}

			foreach (var house in tempMap.CoveredHouses)
			{
				var allCells = (HousesMap[house] & EmptyCells) - pattern;
				for (int size = PopCount((uint)extraDigitsMask) - 1, count = allCells.Count; size < count; size++)
				{
					foreach (var cells in allCells & size)
					{
						var tempMask = grid.GetDigitsUnion(cells);
						if (PopCount((uint)tempMask) != size + 1
							|| (tempMask & extraDigitsMask) != extraDigitsMask)
						{
							continue;
						}

						var conclusions = new List<Conclusion>();
						foreach (var digit in tempMask)
						{
							foreach (var cell in (allCells - cells) & CandidatesMap[digit])
							{
								conclusions.Add(new(Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<CandidateViewNode>();
						foreach (var cell in pattern)
						{
							foreach (var digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(
									new(
										(tempMask >> digit & 1) != 0
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

						var step = new UniqueMatrixType3Step(
							ImmutableArray.CreateRange(conclusions),
							ImmutableArray.Create(
								View.Empty
									| candidateOffsets
									| new HouseViewNode(DisplayColorKind.Normal, house)
							),
							pattern,
							digitsMask,
							extraDigitsMask,
							cells
						);
						if (onlyFindOne)
						{
							return step;
						}

						accumulator.Add(step);
					}
				}
			}
		}

		return null;
	}

	private IStep? CheckType4(
		ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne,
		scoped in CellMap pattern, short mask)
	{
		foreach (var digits in mask.GetAllSets().GetSubsets(4))
		{
			short digitsMask = 0;
			foreach (var digit in digits)
			{
				digitsMask |= (short)(1 << digit);
			}

			var extraDigitsMask = (short)(mask & ~digitsMask);
			var tempMap = CellMap.Empty;
			foreach (var digit in extraDigitsMask)
			{
				tempMap |= CandidatesMap[digit];
			}
			if (tempMap.InOneHouse)
			{
				continue;
			}

			foreach (var house in tempMap.CoveredHouses)
			{
				int d1 = -1, d2 = -1, count = 0;
				var compareMap = HousesMap[house] & pattern;
				foreach (var digit in digits)
				{
					if ((compareMap | HousesMap[house] & CandidatesMap[digit]) == compareMap)
					{
						switch (count++)
						{
							case 0:
							{
								d1 = digit;
								break;
							}
							case 1:
							{
								d2 = digit;
								goto Finally;
							}
						}
					}
				}

			Finally:
				var comparer = (short)(1 << d1 | 1 << d2);
				var otherDigitsMask = (short)(digitsMask & ~comparer);
				var conclusions = new List<Conclusion>();
				foreach (var digit in otherDigitsMask)
				{
					foreach (var cell in compareMap & CandidatesMap[digit])
					{
						conclusions.Add(new(Elimination, cell, digit));
					}
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>();
				foreach (var cell in pattern - compareMap)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
					}
				}
				foreach (var cell in compareMap & CandidatesMap[d1])
				{
					candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + d1));
				}
				foreach (var cell in compareMap & CandidatesMap[d2])
				{
					candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + d2));
				}

				var step = new UniqueMatrixType4Step(
					ImmutableArray.CreateRange(conclusions),
					ImmutableArray.Create(
						View.Empty
							| candidateOffsets
							| new HouseViewNode(DisplayColorKind.Normal, house)
					),
					pattern,
					digitsMask,
					d1,
					d2,
					compareMap
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
}
