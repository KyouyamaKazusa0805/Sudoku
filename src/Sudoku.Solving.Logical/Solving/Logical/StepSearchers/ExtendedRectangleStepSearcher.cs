namespace Sudoku.Solving.Logical.StepSearchers;

[StepSearcher]
[SukakuNotSupported]
internal sealed unsafe partial class ExtendedRectangleStepSearcher : IExtendedRectangleStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(scoped in LogicalAnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		var accumulator = context.Accumulator!;
		var onlyFindOne = context.OnlyFindOne;
		foreach (var (allCellsMap, pairs, size) in IExtendedRectangleStepSearcher.PatternInfos)
		{
			if ((EmptyCells & allCellsMap) != allCellsMap)
			{
				continue;
			}

			// Check each pair.
			// Ensures all pairs should contains same digits
			// and the kind of digits must be greater than 2.
			var checkKindsFlag = true;
			foreach (var (l, r) in pairs)
			{
				var tempMask = (short)(grid.GetCandidates(l) & grid.GetCandidates(r));
				if (tempMask == 0 || (tempMask & tempMask - 1) == 0)
				{
					checkKindsFlag = false;
					break;
				}
			}
			if (!checkKindsFlag)
			{
				// Failed to check.
				continue;
			}

			// Check the mask of cells from two houses.
			short m1 = 0, m2 = 0;
			foreach (var (l, r) in pairs)
			{
				m1 |= grid.GetCandidates(l);
				m2 |= grid.GetCandidates(r);
			}

			var resultMask = (short)(m1 | m2);
			short normalDigits = 0, extraDigits = 0;
			foreach (var digit in resultMask)
			{
				var count = 0;
				foreach (var (l, r) in pairs)
				{
					if (((grid.GetCandidates(l) & grid.GetCandidates(r)) >> digit & 1) != 0)
					{
						// Both two cells contain same digit.
						count++;
					}
				}

				(count >= 2 ? ref normalDigits : ref extraDigits) |= (short)(1 << digit);
			}

			if (PopCount((uint)normalDigits) != size)
			{
				// The number of normal digits are not enough.
				continue;
			}

			if (PopCount((uint)resultMask) == size + 1)
			{
				// Possible type 1 or 2 found.
				// Now check extra cells.
				var extraDigit = TrailingZeroCount(extraDigits);
				var extraCellsMap = allCellsMap & CandidatesMap[extraDigit];
				if (!extraCellsMap)
				{
					continue;
				}

				if (extraCellsMap.Count == 1)
				{
					if (CheckType1(accumulator, grid, allCellsMap, extraCellsMap, normalDigits, extraDigit, onlyFindOne) is { } step1)
					{
						return step1;
					}
				}

				if (CheckType2(accumulator, grid, allCellsMap, extraCellsMap, normalDigits, extraDigit, onlyFindOne) is { } step2)
				{
					return step2;
				}
			}
			else
			{
				var extraCellsMap = CellMap.Empty;
				foreach (var cell in allCellsMap)
				{
					foreach (var digit in extraDigits)
					{
						if (grid[cell, digit])
						{
							extraCellsMap.Add(cell);
							break;
						}
					}
				}

				if (!extraCellsMap.InOneHouse)
				{
					continue;
				}

				if (CheckType3Naked(accumulator, grid, allCellsMap, normalDigits, extraDigits, extraCellsMap, onlyFindOne) is { } step3)
				{
					return step3;
				}

				if (CheckType14(accumulator, grid, allCellsMap, normalDigits, extraCellsMap, onlyFindOne) is { } step14)
				{
					return step14;
				}
			}
		}

		return null;
	}

	/// <summary>
	/// Check type 1.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="allCellsMap">The map of all cells used.</param>
	/// <param name="extraCells">The extra cells map.</param>
	/// <param name="normalDigits">The normal digits mask.</param>
	/// <param name="extraDigit">The extra digit.</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searches for one step.</param>
	/// <returns>The first found step if worth.</returns>
	private IStep? CheckType1(
		ICollection<IStep> accumulator, scoped in Grid grid, scoped in CellMap allCellsMap,
		scoped in CellMap extraCells, short normalDigits, int extraDigit, bool onlyFindOne)
	{
		var (conclusions, candidateOffsets) = (new List<Conclusion>(), new List<CandidateViewNode>());
		foreach (var cell in allCellsMap)
		{
			if (cell == extraCells[0])
			{
				foreach (var digit in grid.GetCandidates(cell))
				{
					if (digit != extraDigit)
					{
						conclusions.Add(new(Elimination, cell, digit));
					}
				}
			}
			else
			{
				foreach (var digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
				}
			}
		}

		if (conclusions.Count == 0)
		{
			goto ReturnNull;
		}

		var step = new ExtendedRectangleType1Step(
			ImmutableArray.CreateRange(conclusions),
			ImmutableArray.Create(View.Empty | candidateOffsets),
			allCellsMap,
			normalDigits
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
	/// <param name="allCellsMap">The map of all cells used.</param>
	/// <param name="extraCells">The extra cells map.</param>
	/// <param name="normalDigits">The normal digits mask.</param>
	/// <param name="extraDigit">The extra digit.</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searches for one step.</param>
	/// <returns>The first found step if worth.</returns>
	private IStep? CheckType2(
		ICollection<IStep> accumulator, scoped in Grid grid, scoped in CellMap allCellsMap,
		scoped in CellMap extraCells, short normalDigits, int extraDigit, bool onlyFindOne)
	{
		if ((extraCells.PeerIntersection & CandidatesMap[extraDigit]) is not (var elimMap and not []))
		{
			goto ReturnNull;
		}

		var candidateOffsets = new List<CandidateViewNode>();
		foreach (var cell in allCellsMap)
		{
			foreach (var digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add(
					new(digit == extraDigit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, cell * 9 + digit)
				);
			}
		}

		var step = new ExtendedRectangleType2Step(
			from cell in elimMap select new Conclusion(Elimination, cell, extraDigit),
			ImmutableArray.Create(View.Empty | candidateOffsets),
			allCellsMap,
			normalDigits,
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
	/// <param name="allCellsMap">The map of all cells used.</param>
	/// <param name="normalDigits">The normal digits mask.</param>
	/// <param name="extraDigits">The extra digits mask.</param>
	/// <param name="extraCellsMap">The map of extra cells.</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searches for one step.</param>
	/// <returns>The first found step if worth.</returns>
	private IStep? CheckType3Naked(
		ICollection<IStep> accumulator, scoped in Grid grid, scoped in CellMap allCellsMap,
		short normalDigits, short extraDigits, scoped in CellMap extraCellsMap, bool onlyFindOne)
	{
		foreach (var houseIndex in extraCellsMap.CoveredHouses)
		{
			var otherCells = (HousesMap[houseIndex] & EmptyCells) - allCellsMap;
			for (int size = 1, length = otherCells.Count; size < length; size++)
			{
				foreach (var cells in otherCells & size)
				{
					var mask = grid.GetDigitsUnion(cells);
					if ((mask & extraDigits) != extraDigits || PopCount((uint)mask) != size + 1)
					{
						continue;
					}

					var elimMap = (HousesMap[houseIndex] & EmptyCells) - allCellsMap - cells;
					if (!elimMap)
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
					foreach (var cell in allCellsMap - extraCellsMap)
					{
						foreach (var digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
						}
					}
					foreach (var cell in extraCellsMap)
					{
						foreach (var digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(
								new(
									(mask >> digit & 1) != 0 ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
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

					var step = new ExtendedRectangleType3Step(
						ImmutableArray.CreateRange(conclusions),
						ImmutableArray.Create(View.Empty | candidateOffsets | new HouseViewNode(0, houseIndex)),
						allCellsMap,
						normalDigits,
						cells,
						mask,
						houseIndex
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

	/// <summary>
	/// Check type 4 and a part of type 1 that the method
	/// <see cref="CheckType1(ICollection{IStep}, in Grid, in CellMap, in CellMap, short, int, bool)"/>
	/// cannot be found.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="allCellsMap">The map of all cells used.</param>
	/// <param name="normalDigits">The normal digits mask.</param>
	/// <param name="extraCellsMap">The map of extra cells.</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searches for one step.</param>
	/// <returns>The first found step if worth.</returns>
	private IStep? CheckType14(
		ICollection<IStep> accumulator, scoped in Grid grid, scoped in CellMap allCellsMap,
		short normalDigits, scoped in CellMap extraCellsMap, bool onlyFindOne)
	{
		switch (extraCellsMap)
		{
			case [var extraCell]:
			{
				// Type 1 found.
				// Check eliminations.
				var conclusions = new List<Conclusion>();
				foreach (var digit in normalDigits)
				{
					if (CandidatesMap[digit].Contains(extraCell))
					{
						conclusions.Add(new(Elimination, extraCell, digit));
					}
				}

				if (conclusions.Count == 0)
				{
					goto ReturnNull;
				}

				// Gather all highlight candidates.
				var candidateOffsets = new List<CandidateViewNode>();
				foreach (var cell in allCellsMap)
				{
					if (cell == extraCell)
					{
						continue;
					}

					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
					}
				}

				var step = new ExtendedRectangleType1Step(
					ImmutableArray.CreateRange(conclusions),
					ImmutableArray.Create(View.Empty | candidateOffsets),
					allCellsMap,
					normalDigits
				);

				if (onlyFindOne)
				{
					return step;
				}

				accumulator.Add(step);

				break;
			}
			case [var extraCell1, var extraCell2]:
			{
				// Type 4.
				short m1 = grid.GetCandidates(extraCell1), m2 = grid.GetCandidates(extraCell2);
				var conjugateMask = (short)(m1 & m2 & normalDigits);
				if (conjugateMask == 0)
				{
					goto ReturnNull;
				}

				foreach (var conjugateDigit in conjugateMask)
				{
					foreach (var houseIndex in extraCellsMap.CoveredHouses)
					{
						var map = HousesMap[houseIndex] & extraCellsMap;
						if (map != extraCellsMap || map != (CandidatesMap[conjugateDigit] & HousesMap[houseIndex]))
						{
							continue;
						}

						var elimDigits = (short)(normalDigits & ~(1 << conjugateDigit));
						var conclusions = new List<Conclusion>();
						foreach (var digit in elimDigits)
						{
							foreach (var cell in extraCellsMap & CandidatesMap[digit])
							{
								conclusions.Add(new(Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<CandidateViewNode>();
						foreach (var cell in allCellsMap - extraCellsMap)
						{
							foreach (var digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
							}
						}
						foreach (var cell in extraCellsMap)
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + conjugateDigit));
						}

						var step = new ExtendedRectangleType4Step(
							ImmutableArray.CreateRange(conclusions),
							ImmutableArray.Create(View.Empty | candidateOffsets | new HouseViewNode(0, houseIndex)),
							allCellsMap,
							normalDigits,
							new(extraCellsMap, conjugateDigit)
						);

						if (onlyFindOne)
						{
							return step;
						}

						accumulator.Add(step);
					}
				}

				break;
			}
		}

	ReturnNull:
		return null;
	}
}
