namespace Sudoku.Solving.Logical.StepSearchers;

[StepSearcher]
[StepSearcherRunningOptions(StepSearcherRunningOptions.OnlyForStandardSudoku)]
internal sealed unsafe partial class BivalueUniversalGraveStepSearcher : IBivalueUniversalGraveStepSearcher
{
	/// <inheritdoc/>
	[StepSearcherProperty]
	public bool SearchExtendedTypes { get; set; }


	/// <inheritdoc/>
	public IStep? GetAll(scoped in LogicalAnalysisContext context)
	{
		if (CheckForTrueCandidateTypes(context) is { } trueCandidateTypeFirstFoundStep)
		{
			return trueCandidateTypeFirstFoundStep;
		}
		if (CheckForFalseCandidateTypes(context) is { } falseCandidateTypeFirstFoundStep)
		{
			return falseCandidateTypeFirstFoundStep;
		}

		return null;
	}

	private IStep? CheckForTrueCandidateTypes(scoped in LogicalAnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		if (!IBivalueUniversalGraveStepSearcher.FindTrueCandidates(grid, out var trueCandidates))
		{
			return null;
		}

		switch (trueCandidates)
		{
			case []:
			{
				return IInvalidStep.Instance;
			}
			case [var trueCandidate]:
			{
				// BUG + 1 found.
				var step = new BivalueUniversalGraveType1Step(
					ImmutableArray.Create(new Conclusion(Assignment, trueCandidate)),
					ImmutableArray.Create(View.Empty | new CandidateViewNode(DisplayColorKind.Normal, trueCandidate))
				);
				if (context.OnlyFindOne)
				{
					return step;
				}

				context.Accumulator.Add(step);

				break;
			}
			default:
			{
				var onlyFindOne = context.OnlyFindOne;
				var accumulator = context.Accumulator!;

				if (CheckSingleDigit(trueCandidates))
				{
					if (CheckType2(accumulator, trueCandidates, onlyFindOne) is { } type2Step)
					{
						return type2Step;
					}
				}
				else
				{
					if (SearchExtendedTypes)
					{
						if (CheckMultiple(accumulator, trueCandidates, onlyFindOne) is { } typeMultipleStep)
						{
							return typeMultipleStep;
						}
						if (CheckXz(accumulator, grid, trueCandidates, onlyFindOne) is { } typeXzStep)
						{
							return typeXzStep;
						}
					}

					if (CheckType3Naked(accumulator, grid, trueCandidates, onlyFindOne) is { } type3Step)
					{
						return type3Step;
					}
					if (CheckType4(accumulator, trueCandidates, onlyFindOne) is { } type4Step)
					{
						return type4Step;
					}
				}

				break;
			}
		}

		return null;
	}

	private IStep? CheckForFalseCandidateTypes(scoped in LogicalAnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		var multivalueCells = EmptyCells - BivalueCells;
		if ((multivalueCells.PeerIntersection & EmptyCells) is not (var falseCandidatePossibleCells and not []))
		{
			// Optimization: The false candidates must lie in the intersection of all multi-value cells.
			// If the multi-value cells cannot confluent into any cells, no possible false candidates
			// will be found.
			return null;
		}

		foreach (var cell in falseCandidatePossibleCells)
		{
			foreach (var digit in grid.GetCandidates(cell))
			{
				var copied = grid;
				copied[cell] = digit;

				if (!IBivalueUniversalGraveStepSearcher.FormsPattern(copied))
				{
					continue;
				}

				var cellOffsets = new List<CellViewNode>(multivalueCells.Count);
				foreach (var multiValueCell in multivalueCells)
				{
					cellOffsets.Add(new(DisplayColorKind.Normal, multiValueCell));
				}

				var step = new BivalueUniversalGraveFalseCandidateTypeStep(
					ImmutableArray.Create(new Conclusion(Elimination, cell, digit)),
					ImmutableArray.Create(View.Empty | cellOffsets),
					cell * 9 + digit
				);
				if (context.OnlyFindOne)
				{
					return step;
				}

				context.Accumulator.Add(step);
			}
		}

		return null;
	}

	private IStep? CheckType2(ICollection<IStep> accumulator, IReadOnlyList<int> trueCandidates, bool onlyFindOne)
	{
		scoped var cells = (stackalloc int[trueCandidates.Count]);
		var i = 0;
		foreach (var candidate in trueCandidates)
		{
			cells[i++] = candidate / 9;
		}
		if (((CellMap)cells).PeerIntersection is not (var map and not []))
		{
			return null;
		}

		var digit = trueCandidates[0] % 9;
		if ((map & CandidatesMap[digit]) is not (var elimMap and not []))
		{
			return null;
		}

		var conclusions = new List<Conclusion>(elimMap.Count);
		foreach (var cell in elimMap)
		{
			conclusions.Add(new(Elimination, cell, digit));
		}

		var candidateOffsets = new List<CandidateViewNode>(trueCandidates.Count);
		foreach (var candidate in trueCandidates)
		{
			candidateOffsets.Add(new(DisplayColorKind.Normal, candidate));
		}

		// BUG type 2.
		var step = new BivalueUniversalGraveType2Step(
			ImmutableArray.CreateRange(conclusions),
			ImmutableArray.Create(View.Empty | candidateOffsets),
			digit,
			(CellMap)cells
		);
		if (onlyFindOne)
		{
			return step;
		}

		accumulator.Add(step);

		return null;
	}

	private IStep? CheckType3Naked(
		ICollection<IStep> accumulator, scoped in Grid grid,
		IReadOnlyList<int> trueCandidates, bool onlyFindOne)
	{
		// Check whether all true candidates lie in a same house.
		var map = CellMap.Empty + from c in trueCandidates group c by c / 9 into z select z.Key;
		if (!map.InOneHouse)
		{
			return null;
		}

		// Get the digit mask.
		short digitsMask = 0;
		foreach (var candidate in trueCandidates)
		{
			digitsMask |= (short)(1 << candidate % 9);
		}

		// Iterate on each house that the true candidates lying on.
		foreach (var house in map.CoveredHouses)
		{
			var houseMap = HousesMap[house];
			if ((houseMap & EmptyCells) - map is not (var otherCellsMap and not []))
			{
				continue;
			}

			// Iterate on each size.
			for (var size = 1; size < otherCellsMap.Count; size++)
			{
				foreach (var cells in otherCellsMap & size)
				{
					var mask = (short)(digitsMask | grid.GetDigitsUnion(cells));
					if (PopCount((uint)mask) != size + 1)
					{
						continue;
					}

					if (((houseMap - cells - map) & EmptyCells) is not (var elimMap and not []))
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					foreach (var cell in elimMap)
					{
						foreach (var digit in grid.GetCandidates(cell))
						{
							if ((mask >> digit & 1) != 0)
							{
								conclusions.Add(new(Elimination, cell, digit));
							}
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<CandidateViewNode>();
					foreach (var cand in trueCandidates)
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, cand));
					}
					foreach (var cell in cells)
					{
						foreach (var digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
						}
					}

					var step = new BivalueUniversalGraveType3Step(
						ImmutableArray.CreateRange(conclusions),
						ImmutableArray.Create(
							View.Empty
								| candidateOffsets
								| new HouseViewNode(DisplayColorKind.Normal, house)
						),
						trueCandidates,
						digitsMask,
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

	private IStep? CheckType4(ICollection<IStep> accumulator, IReadOnlyList<int> trueCandidates, bool onlyFindOne)
	{
		// Conjugate pairs should lie in two cells.
		var candsGroupByCell = from candidate in trueCandidates group candidate by candidate / 9;
		if (candsGroupByCell.Take(3).Count() != 2)
		{
			return null;
		}

		// Check two cell has same house.
		var cells = new List<int>();
		foreach (var candGroupByCell in candsGroupByCell)
		{
			cells.Add(candGroupByCell.Key);
		}

		var houses = (CellMap.Empty + cells).CoveredHouses;
		if (houses != 0)
		{
			return null;
		}

		// Check for each house.
		foreach (var house in houses)
		{
			// Add up all digits.
			var digits = new HashSet<int>();
			foreach (var candGroupByCell in candsGroupByCell)
			{
				foreach (var cand in candGroupByCell)
				{
					digits.Add(cand % 9);
				}
			}

			// Check whether exists a conjugate pair in this house.
			for (var conjuagtePairDigit = 0; conjuagtePairDigit < 9; conjuagtePairDigit++)
			{
				// Check whether forms a conjugate pair.
				var mask = (HousesMap[house] & CandidatesMap[conjuagtePairDigit]) / house;
				if (PopCount((uint)mask) != 2)
				{
					continue;
				}

				// Check whether the conjugate pair lies in current two cells.
				int first = TrailingZeroCount(mask), second = mask.GetNextSet(first);
				int c1 = HouseCells[house][first], c2 = HouseCells[house][second];
				if (c1 != cells[0] || c2 != cells[1])
				{
					continue;
				}

				// Check whether all digits contain that digit.
				if (digits.Contains(conjuagtePairDigit))
				{
					continue;
				}

				// BUG type 4 found.
				// Now add up all eliminations.
				var conclusions = new List<Conclusion>();
				foreach (var candGroupByCell in candsGroupByCell)
				{
					var cell = candGroupByCell.Key;
					short digitMask = 0;
					foreach (var cand in candGroupByCell)
					{
						digitMask |= (short)(1 << cand % 9);
					}

					// Bitwise not.
					foreach (var d in digitMask = (short)(~digitMask & Grid.MaxCandidatesMask))
					{
						if (conjuagtePairDigit == d || !CandidatesMap[d].Contains(cell))
						{
							continue;
						}

						conclusions.Add(new(Elimination, cell, d));
					}
				}

				// Check eliminations.
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new CandidateViewNode[trueCandidates.Count + 2];
				var i = 0;
				foreach (var candidate in trueCandidates)
				{
					candidateOffsets[i++] = new(DisplayColorKind.Normal, candidate);
				}
				candidateOffsets[^2] = new(DisplayColorKind.Auxiliary1, c1 * 9 + conjuagtePairDigit);
				candidateOffsets[^1] = new(DisplayColorKind.Auxiliary1, c2 * 9 + conjuagtePairDigit);

				// BUG type 4.
				short digitsMask = 0;
				foreach (var digit in digits)
				{
					digitsMask |= (short)(1 << digit);
				}

				var step = new BivalueUniversalGraveType4Step(
					ImmutableArray.CreateRange(conclusions),
					ImmutableArray.Create(
						View.Empty
							| candidateOffsets
							| new HouseViewNode(DisplayColorKind.Normal, house)
					),
					digitsMask,
					CellMap.Empty + cells,
					new(c1, c2, conjuagtePairDigit)
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

	private IStep? CheckMultiple(ICollection<IStep> accumulator, IReadOnlyList<int> trueCandidates, bool onlyFindOne)
	{
		if (trueCandidates.Count > 18)
		{
			return null;
		}

		if ((CandidateMap.Empty + trueCandidates).PeerIntersection is not { Count: var mapCount and not 0 } map)
		{
			return null;
		}

		// BUG + n found.
		// Check eliminations.
		var conclusions = new List<Conclusion>(mapCount);
		foreach (var candidate in map)
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

		var candidateOffsets = new CandidateViewNode[trueCandidates.Count];
		var i = 0;
		foreach (var candidate in trueCandidates)
		{
			candidateOffsets[i++] = new(DisplayColorKind.Normal, candidate);
		}

		// BUG + n.
		var step = new BivalueUniversalGraveMultipleStep(
			ImmutableArray.CreateRange(conclusions),
			ImmutableArray.Create(View.Empty | candidateOffsets),
			trueCandidates
		);
		if (onlyFindOne)
		{
			return step;
		}

		accumulator.Add(step);

		return null;
	}

	private IStep? CheckXz(
		ICollection<IStep> accumulator, scoped in Grid grid, IReadOnlyList<int> trueCandidates, bool onlyFindOne)
	{
		if (trueCandidates is not [var cand1, var cand2])
		{
			return null;
		}

		int c1 = cand1 / 9, c2 = cand2 / 9, d1 = cand1 % 9, d2 = cand2 % 9;
		var mask = (short)(1 << d1 | 1 << d2);
		foreach (var cell in (PeersMap[c1] ^ PeersMap[c2]) & BivalueCells)
		{
			if (grid.GetCandidates(cell) != mask)
			{
				continue;
			}

			// BUG-XZ found.
			var conclusions = new List<Conclusion>();
			var condition = (CellsMap[c1] + cell).InOneHouse;
			var anotherCell = condition ? c2 : c1;
			var anotherDigit = condition ? d2 : d1;
			foreach (var peer in (CellsMap[cell] + anotherCell).PeerIntersection)
			{
				if (CandidatesMap[anotherDigit].Contains(peer))
				{
					conclusions.Add(new(Elimination, peer, anotherDigit));
				}
			}
			if (conclusions.Count == 0)
			{
				continue;
			}

			var candidateOffsets = new CandidateViewNode[trueCandidates.Count];
			for (var i = 0; i < trueCandidates.Count; i++)
			{
				candidateOffsets[i] = new(DisplayColorKind.Normal, trueCandidates[i]);
			}

			var step = new BivalueUniversalGraveXzStep(
				ImmutableArray.CreateRange(conclusions),
				ImmutableArray.Create(
					View.Empty
						| new CellViewNode(DisplayColorKind.Normal, cell)
						| candidateOffsets
				),
				mask,
				CellsMap[c1] + c2,
				cell
			);
			if (onlyFindOne)
			{
				return step;
			}

			accumulator.Add(step);
		}

		return null;
	}


	/// <summary>
	/// Check whether all candidates in the list has same digit value.
	/// </summary>
	/// <param name="list">The list of all true candidates.</param>
	/// <returns>A <see cref="bool"/> indicating that.</returns>
	private static bool CheckSingleDigit(IReadOnlyList<int> list)
	{
		var i = 0;
		SkipInit(out int comparer);
		foreach (var cand in list)
		{
			if (i++ == 0)
			{
				comparer = cand % 9;
				continue;
			}

			if (comparer != cand % 9)
			{
				return false;
			}
		}

		return true;
	}
}
