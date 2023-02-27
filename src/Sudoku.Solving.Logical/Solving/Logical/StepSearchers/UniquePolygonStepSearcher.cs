namespace Sudoku.Solving.Logical.StepSearchers;

[StepSearcher]
[StepSearcherRunningOptions(StepSearcherRunningOptions.OnlyForStandardSudoku)]
internal sealed unsafe partial class UniquePolygonStepSearcher : IUniquePolygonStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(scoped ref LogicalAnalysisContext context)
	{
		if (EmptyCells.Count < 7)
		{
			return null;
		}

		scoped ref readonly var grid = ref context.Grid;
		var accumulator = context.Accumulator!;
		var onlyFindOne = context.OnlyFindOne;
		for (var i = 0; i < (EmptyCells.Count == 7 ? BdpTemplatesSize3Count : BdpTemplatesSize4Count); i++)
		{
			var pattern = IUniquePolygonStepSearcher.Patterns[i];
			if ((EmptyCells & pattern.Map) != pattern.Map)
			{
				// The pattern contains non-empty cells.
				continue;
			}

			var map = pattern.Map;
			var ((p11, p12), (p21, p22), (c1, c2, c3, c4)) = pattern;
			var cornerMask1 = (short)(grid.GetCandidates(p11) | grid.GetCandidates(p12));
			var cornerMask2 = (short)(grid.GetCandidates(p21) | grid.GetCandidates(p22));
			var centerMask = (short)((short)(grid.GetCandidates(c1) | grid.GetCandidates(c2)) | grid.GetCandidates(c3));
			if (map.Count == 8)
			{
				centerMask |= grid.GetCandidates(c4);
			}

			if (CheckType1(accumulator, grid, pattern, onlyFindOne, cornerMask1, cornerMask2, centerMask, map) is { } type1Step)
			{
				return type1Step;
			}
			if (CheckType2(accumulator, grid, pattern, onlyFindOne, cornerMask1, cornerMask2, centerMask, map) is { } type2Step)
			{
				return type2Step;
			}
			if (CheckType3(accumulator, grid, pattern, onlyFindOne, cornerMask1, cornerMask2, centerMask, map) is { } type3Step)
			{
				return type3Step;
			}
			if (CheckType4(accumulator, grid, pattern, onlyFindOne, cornerMask1, cornerMask2, centerMask, map) is { } type4Step)
			{
				return type4Step;
			}
		}

		return null;
	}


	private static IStep? CheckType1(
		ICollection<IStep> accumulator, scoped in Grid grid, UniquePolygon pattern,
		bool findOnlyOne, short cornerMask1, short cornerMask2, short centerMask, scoped in CellMap map)
	{
		var orMask = (short)((short)(cornerMask1 | cornerMask2) | centerMask);
		if (PopCount((uint)orMask) != (pattern.IsHeptagon ? 4 : 5))
		{
			goto ReturnNull;
		}

		// Iterate on each combination.
		foreach (var digits in orMask.GetAllSets().GetSubsets(pattern.IsHeptagon ? 3 : 4))
		{
			short tempMask = 0;
			foreach (var digit in digits)
			{
				tempMask |= (short)(1 << digit);
			}

			var otherDigit = TrailingZeroCount(orMask & ~tempMask);
			var mapContainingThatDigit = map & CandidatesMap[otherDigit];
			if (mapContainingThatDigit is not [var elimCell])
			{
				continue;
			}

			var elimMask = (short)(grid.GetCandidates(elimCell) & tempMask);
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
			foreach (var cell in map)
			{
				if (mapContainingThatDigit.Contains(cell))
				{
					continue;
				}

				foreach (var digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
				}
			}

			var step = new UniquePolygonType1Step(
				ImmutableArray.CreateRange(conclusions),
				ImmutableArray.Create(View.Empty | candidateOffsets),
				map,
				tempMask
			);
			if (findOnlyOne)
			{
				return step;
			}

			accumulator.Add(step);
		}

	ReturnNull:
		return null;
	}

	private static IStep? CheckType2(
		ICollection<IStep> accumulator, scoped in Grid grid, UniquePolygon pattern,
		bool findOnlyOne, short cornerMask1, short cornerMask2, short centerMask, scoped in CellMap map)
	{
		var orMask = (short)((short)(cornerMask1 | cornerMask2) | centerMask);
		if (PopCount((uint)orMask) != (pattern.IsHeptagon ? 4 : 5))
		{
			goto ReturnNull;
		}

		// Iterate on each combination.
		foreach (var digits in orMask.GetAllSets().GetSubsets(pattern.IsHeptagon ? 3 : 4))
		{
			short tempMask = 0;
			foreach (var digit in digits)
			{
				tempMask |= (short)(1 << digit);
			}

			var otherDigit = TrailingZeroCount(orMask & ~tempMask);
			var mapContainingThatDigit = map & CandidatesMap[otherDigit];
			var elimMap = (mapContainingThatDigit.PeerIntersection - map) & CandidatesMap[otherDigit];
			if (!elimMap)
			{
				continue;
			}

			var conclusions = new List<Conclusion>();
			foreach (var cell in elimMap)
			{
				conclusions.Add(new(Elimination, cell, otherDigit));
			}

			var candidateOffsets = new List<CandidateViewNode>();
			foreach (var cell in map)
			{
				foreach (var digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add(
						new(
							digit == otherDigit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
							cell * 9 + digit
						)
					);
				}
			}

			var step = new UniquePolygonType2Step(
				ImmutableArray.CreateRange(conclusions),
				ImmutableArray.Create(View.Empty | candidateOffsets),
				map,
				tempMask,
				otherDigit
			);
			if (findOnlyOne)
			{
				return step;
			}

			accumulator.Add(step);
		}

	ReturnNull:
		return null;
	}

	private static IStep? CheckType3(
		ICollection<IStep> accumulator, scoped in Grid grid, UniquePolygon pattern,
		bool findOnlyOne, short cornerMask1, short cornerMask2, short centerMask, scoped in CellMap map)
	{
		var orMask = (short)((short)(cornerMask1 | cornerMask2) | centerMask);
		foreach (var houseIndex in map.Houses)
		{
			var currentMap = HousesMap[houseIndex] & map;
			var otherCellsMap = map - currentMap;
			var otherMask = grid.GetDigitsUnion(otherCellsMap);

			foreach (var digits in orMask.GetAllSets().GetSubsets(pattern.IsHeptagon ? 3 : 4))
			{
				short tempMask = 0;
				foreach (var digit in digits)
				{
					tempMask |= (short)(1 << digit);
				}
				if (otherMask != tempMask)
				{
					continue;
				}

				// Iterate on the cells by the specified size.
				var iterationCellsMap = (HousesMap[houseIndex] - currentMap) & EmptyCells;
				var otherDigitsMask = (short)(orMask & ~tempMask);
				for (var size = PopCount((uint)otherDigitsMask) - 1; size < iterationCellsMap.Count; size++)
				{
					foreach (var combination in iterationCellsMap & size)
					{
						var comparer = grid.GetDigitsUnion(combination);
						if ((tempMask & comparer) != 0 || PopCount((uint)tempMask) - 1 != size
							|| (tempMask & otherDigitsMask) != otherDigitsMask)
						{
							continue;
						}

						// Type 3 found.
						// Now check eliminations.
						var conclusions = new List<Conclusion>();
						foreach (var digit in comparer)
						{
							if ((iterationCellsMap & CandidatesMap[digit]) is not (var cells and not []))
							{
								continue;
							}

							foreach (var cell in cells)
							{
								conclusions.Add(new(Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<CandidateViewNode>();
						foreach (var cell in currentMap)
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
						foreach (var cell in otherCellsMap)
						{
							foreach (var digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
							}
						}
						foreach (var cell in combination)
						{
							foreach (var digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
							}
						}

						var step = new UniquePolygonType3Step(
							ImmutableArray.CreateRange(conclusions),
							ImmutableArray.Create(
								View.Empty
									| candidateOffsets
									| new HouseViewNode(DisplayColorKind.Normal, houseIndex)
							),
							map,
							tempMask,
							combination,
							otherDigitsMask
						);
						if (findOnlyOne)
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

	private static IStep? CheckType4(
		ICollection<IStep> accumulator, scoped in Grid grid, UniquePolygon pattern,
		bool findOnlyOne, short cornerMask1, short cornerMask2, short centerMask, scoped in CellMap map)
	{
		// The type 4 may be complex and terrible to process.
		// All houses that the pattern lies in should be checked.
		var orMask = (short)((short)(cornerMask1 | cornerMask2) | centerMask);
		foreach (var houseIndex in map.Houses)
		{
			var currentMap = HousesMap[houseIndex] & map;
			var otherCellsMap = map - currentMap;
			var otherMask = grid.GetDigitsUnion(otherCellsMap);

			// Iterate on each possible digit combination.
			// For example, if values are { 1, 2, 3 }, then all combinations taken 2 values
			// are { 1, 2 }, { 2, 3 } and { 1, 3 }.
			foreach (var digits in orMask.GetAllSets().GetSubsets(pattern.IsHeptagon ? 3 : 4))
			{
				short tempMask = 0;
				foreach (var digit in digits)
				{
					tempMask |= (short)(1 << digit);
				}
				if (otherMask != tempMask)
				{
					continue;
				}

				// Iterate on each combination.
				// Only one digit should be eliminated, and other digits should form a "conjugate house".
				// In a so-called conjugate house, the digits can only appear in these cells in this house.
				foreach (var combination in (tempMask & orMask).GetAllSets().GetSubsets(currentMap.Count - 1))
				{
					short combinationMask = 0;
					var combinationMap = CellMap.Empty;
					var flag = false;
					foreach (var digit in combination)
					{
						if (ValuesMap[digit] && HousesMap[houseIndex])
						{
							flag = true;
							break;
						}

						combinationMask |= (short)(1 << digit);
						combinationMap |= CandidatesMap[digit] & HousesMap[houseIndex];
					}
					if (flag)
					{
						// The house contains digit value, which is not a normal pattern.
						continue;
					}

					if (combinationMap != currentMap)
					{
						// If not equal, the map may contains other digits in this house.
						// Therefore the conjugate house can't form.
						continue;
					}

					// Type 4 forms. Now check eliminations.
					var finalDigits = (short)(tempMask & ~combinationMask);
					var possibleCandMaps = CellMap.Empty;
					foreach (var finalDigit in finalDigits)
					{
						possibleCandMaps |= CandidatesMap[finalDigit];
					}
					if ((combinationMap & possibleCandMaps) is not (var elimMap and not []))
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					foreach (var cell in elimMap)
					{
						foreach (var digit in finalDigits)
						{
							if (CandidatesMap[digit].Contains(cell))
							{
								conclusions.Add(new(Elimination, cell, digit));
							}
						}
					}

					var candidateOffsets = new List<CandidateViewNode>();
					foreach (var cell in currentMap)
					{
						foreach (var digit in (short)(grid.GetCandidates(cell) & combinationMask))
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
						}
					}
					foreach (var cell in otherCellsMap)
					{
						foreach (var digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
						}
					}

					var step = new UniquePolygonType4Step(
						ImmutableArray.CreateRange(conclusions),
						ImmutableArray.Create(
							View.Empty
								| candidateOffsets
								| new HouseViewNode(DisplayColorKind.Normal, houseIndex)
						),
						map,
						otherMask,
						currentMap,
						combinationMask
					);
					if (findOnlyOne)
					{
						return step;
					}

					accumulator.Add(step);
				}
			}
		}

		return null;
	}
}
