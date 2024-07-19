namespace Sudoku.Analytics.StepSearchers;

using TargetCandidatesGroup = BitStatusMapGrouping<CandidateMap, Candidate, CandidateMap.Enumerator, Cell>;

/// <summary>
/// Provides with a <b>Bi-value Universal Grave</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Basic types:
/// <list type="bullet">
/// <item>Bi-value Universal Grave Type 1</item>
/// <item>Bi-value Universal Grave Type 2</item>
/// <item>Bi-value Universal Grave Type 3</item>
/// <item>Bi-value Universal Grave Type 4</item>
/// </list>
/// </item>
/// <item>
/// Extended types:
/// <list type="bullet">
/// <item>Bi-value Universal Grave + n</item>
/// <item>Bi-value Universal Grave XZ</item>
/// </list>
/// </item>
/// <item>Bi-value Universal Grave False Candidate Type</item>
/// </list>
/// </summary>
[StepSearcher(
	"StepSearcherName_BivalueUniversalGraveStepSearcher",

	// Basic types
	Technique.BivalueUniversalGraveType1, Technique.BivalueUniversalGraveType2, Technique.BivalueUniversalGraveType3,
	Technique.BivalueUniversalGraveType4,

	// Extra types
	Technique.BivalueUniversalGraveXzRule, Technique.BivalueUniversalGraveXyWing,
	Technique.BivalueUniversalGravePlusN, Technique.BivalueUniversalGravePlusNForcingChains,
	Technique.BivalueUniversalGraveFalseCandidateType,

	SupportedSudokuTypes = SudokuType.Standard,
	SupportAnalyzingMultipleSolutionsPuzzle = false)]
public sealed partial class BivalueUniversalGraveStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates whether the searcher should call the extended BUG checker
	/// to search for all true candidates no matter how difficult searching.
	/// </summary>
	[SettingItemName(SettingItemNames.SearchExtendedBivalueUniversalGraveTypes)]
	public bool SearchExtendedTypes { get; set; }


	/// <inheritdoc/>
	protected internal override Step? Collect(ref AnalysisContext context)
	{
		ref readonly var grid = ref context.Grid;
		var emptyCellsCount = EmptyCells.Count;
		var candidatesCount = 0;
		foreach (var cell in EmptyCells)
		{
			candidatesCount += PopCount((uint)grid.GetCandidates(cell));
		}

		if (candidatesCount > (emptyCellsCount << 1) + PeersCount + 8)
		{
			// No possible eliminations can be found, regardless of types.
			return null;
		}

		if (CheckForTrueCandidateTypes(ref context) is { } trueCandidateTypeFirstFoundStep)
		{
			return trueCandidateTypeFirstFoundStep;
		}
		if (CheckForFalseCandidateTypes(ref context) is { } falseCandidateTypeFirstFoundStep)
		{
			return falseCandidateTypeFirstFoundStep;
		}

		return null;
	}

	/// <summary>
	/// Check for types that uses true candidates.
	/// </summary>
	/// <param name="context"><inheritdoc cref="Collect(ref AnalysisContext)" path="/param[@name='context']"/></param>
	/// <returns><inheritdoc cref="Collect(ref AnalysisContext)" path="/returns"/></returns>
	private Step? CheckForTrueCandidateTypes(ref AnalysisContext context)
	{
		ref readonly var grid = ref context.Grid;
		switch (TrueCandidate.GetAllTrueCandidates(in grid))
		{
			case []:
			{
				return null;
			}
			case [var trueCandidate]:
			{
				// BUG + 1 found.
				var step = new BivalueUniversalGraveType1Step(
					[new(Assignment, trueCandidate)],
					[[new CandidateViewNode(ColorIdentifier.Normal, trueCandidate)]],
					context.Options
				);
				if (context.OnlyFindOne)
				{
					return step;
				}

				context.Accumulator.Add(step);
				break;
			}
			case var trueCandidates:
			{
				var (onlyFindOne, accumulator) = (context.OnlyFindOne, context.Accumulator!);
				if (checkForSingleDigit(in trueCandidates))
				{
					if (CheckType2(accumulator, ref context, in trueCandidates, onlyFindOne) is { } type2Step)
					{
						return type2Step;
					}
				}
				else
				{
					if (SearchExtendedTypes)
					{
						if (CheckMultiple(accumulator, ref context, in trueCandidates, onlyFindOne) is { } typeMultipleStep)
						{
							return typeMultipleStep;
						}
						if (CheckXz(accumulator, in grid, ref context, in trueCandidates, onlyFindOne) is { } typeXzStep)
						{
							return typeXzStep;
						}
					}

					if (CheckType3Naked(accumulator, in grid, ref context, in trueCandidates, onlyFindOne) is { } type3Step)
					{
						return type3Step;
					}
					if (CheckType4(accumulator, ref context, in trueCandidates, onlyFindOne) is { } type4Step)
					{
						return type4Step;
					}
				}

				break;
			}
		}

		return null;


		static bool checkForSingleDigit(ref readonly CandidateMap trueCandidates)
		{
			var i = 0;
			Unsafe.SkipInit<Digit>(out var comparer);
			foreach (var trueCandidate in trueCandidates)
			{
				if (i++ == 0)
				{
					comparer = trueCandidate % 9;
					continue;
				}

				if (comparer != trueCandidate % 9)
				{
					return false;
				}
			}

			return true;
		}
	}

	/// <summary>
	/// Check for types that uses false candidates.
	/// </summary>
	/// <param name="context"><inheritdoc cref="Collect(ref AnalysisContext)" path="/param[@name='context']"/></param>
	/// <returns><inheritdoc cref="Collect(ref AnalysisContext)" path="/returns"/></returns>
	private BivalueUniversalGraveFalseCandidateTypeStep? CheckForFalseCandidateTypes(ref AnalysisContext context)
	{
		ref readonly var grid = ref context.Grid;
		var multivalueCells = EmptyCells & ~BivalueCells;
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
				copied.SetDigit(cell, digit);

				if (!formsDeadlyPattern(in copied))
				{
					continue;
				}

				var step = new BivalueUniversalGraveFalseCandidateTypeStep(
					[new(Elimination, cell, digit)],
					[[.. from multiValueCell in multivalueCells select new CellViewNode(ColorIdentifier.Normal, multiValueCell)]],
					context.Options,
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


		static bool formsDeadlyPattern(ref readonly Grid grid)
		{
			_ = grid is { BivalueCells: var bivalueCells, EmptyCells: var emptyCells, CandidatesMap: var candidatesMap };
			if (bivalueCells != emptyCells)
			{
				return false;
			}

			var housesCount = (stackalloc int[3]);
			foreach (var cell in emptyCells)
			{
				foreach (var digit in grid.GetCandidates(cell))
				{
					housesCount.Clear();
					foreach (var houseType in HouseTypes)
					{
						housesCount[(int)houseType] = (candidatesMap[digit] & HousesMap[cell.ToHouse(houseType)]).Count;
					}

					if (housesCount is not [2, 2, 2])
					{
						return false;
					}
				}
			}

			return true;
		}
	}

	/// <summary>
	/// Check for type 2.
	/// </summary>
	private BivalueUniversalGraveType2Step? CheckType2(
		List<Step> accumulator,
		ref AnalysisContext context,
		ref readonly CandidateMap trueCandidates,
		bool onlyFindOne
	)
	{
		var cells = (stackalloc Cell[trueCandidates.Count]);
		var i = 0;
		foreach (var candidate in trueCandidates)
		{
			cells[i++] = candidate / 9;
		}

		var cellsMap = cells.AsCellMap();
		if (cellsMap.PeerIntersection is not (var map and not []))
		{
			return null;
		}

		var digit = trueCandidates[0] % 9;
		if ((map & CandidatesMap[digit]) is not (var elimMap and not []))
		{
			return null;
		}

		// BUG type 2.
		var step = new BivalueUniversalGraveType2Step(
			[.. from cell in elimMap select new Conclusion(Elimination, cell, digit)],
			[[.. from candidate in trueCandidates select new CandidateViewNode(ColorIdentifier.Normal, candidate)]],
			context.Options,
			digit,
			in cellsMap
		);
		if (onlyFindOne)
		{
			return step;
		}

		accumulator.Add(step);

		return null;
	}

	/// <summary>
	/// Check for type 3 with naked subsets.
	/// </summary>
	private BivalueUniversalGraveType3Step? CheckType3Naked(
		List<Step> accumulator,
		ref readonly Grid grid,
		ref AnalysisContext context,
		ref readonly CandidateMap trueCandidates,
		bool onlyFindOne
	)
	{
		// Check whether all true candidates lie in a same house.
		var map = TargetCandidatesGroup.CreateMapByKeys(from c in trueCandidates group c by c / 9);
		if (!map.InOneHouse(out _))
		{
			return null;
		}

		// Get the digit mask.
		var digitsMask = (Mask)0;
		foreach (var candidate in trueCandidates)
		{
			digitsMask |= (Mask)(1 << candidate % 9);
		}

		// Iterate on each house that the true candidates lying on.
		foreach (var house in map.SharedHouses)
		{
			var houseMap = HousesMap[house];
			if ((houseMap & EmptyCells & ~map) is not (var otherCellsMap and not []))
			{
				continue;
			}

			// Iterate on each size.
			for (var size = 1; size < otherCellsMap.Count; size++)
			{
				foreach (ref readonly var cells in otherCellsMap & size)
				{
					var mask = (Mask)(digitsMask | grid[in cells]);
					if (PopCount((uint)mask) != size + 1)
					{
						continue;
					}

					if ((houseMap & ~cells & ~map & EmptyCells) is not (var elimMap and not []))
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
						candidateOffsets.Add(new(ColorIdentifier.Normal, cand));
					}
					foreach (var cell in cells)
					{
						foreach (var digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, cell * 9 + digit));
						}
					}

					var step = new BivalueUniversalGraveType3Step(
						[.. conclusions],
						[[.. candidateOffsets, new HouseViewNode(ColorIdentifier.Normal, house)]],
						context.Options,
						in trueCandidates,
						digitsMask,
						in cells,
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

	/// <summary>
	/// Check for type 4.
	/// </summary>
	private BivalueUniversalGraveType4Step? CheckType4(
		List<Step> accumulator,
		ref AnalysisContext context,
		ref readonly CandidateMap trueCandidates,
		bool onlyFindOne
	)
	{
		// Conjugate pairs should lie in two cells.
		var candsGroupByCell = from candidate in trueCandidates group candidate by candidate / 9;
		if (candsGroupByCell.Length != 2)
		{
			return null;
		}

		// Check two cell has same house.
		var cells = TargetCandidatesGroup.CreateMapByKeys(candsGroupByCell);
		var houses = cells.SharedHouses;
		if (houses != 0)
		{
			return null;
		}

		// Check for each house.
		foreach (var house in houses)
		{
			// Add up all digits.
			var digits = new HashSet<Digit>();
			foreach (var candGroupByCell in candsGroupByCell)
			{
				foreach (var cand in candGroupByCell)
				{
					digits.Add(cand % 9);
				}
			}

			// Check whether exists a conjugate pair in this house.
			for (var conjugatePairDigit = 0; conjugatePairDigit < 9; conjugatePairDigit++)
			{
				// Check whether forms a conjugate pair.
				var mask = (HousesMap[house] & CandidatesMap[conjugatePairDigit]) / house;
				if (PopCount((uint)mask) != 2)
				{
					continue;
				}

				// Check whether the conjugate pair lies in current two cells.
				var first = TrailingZeroCount(mask);
				var second = mask.GetNextSet(first);
				var c1 = HousesCells[house][first];
				var c2 = HousesCells[house][second];
				if (c1 != cells[0] || c2 != cells[1])
				{
					continue;
				}

				// Check whether all digits contain that digit.
				if (digits.Contains(conjugatePairDigit))
				{
					continue;
				}

				// BUG type 4 found.
				// Now add up all eliminations.
				var conclusions = new List<Conclusion>();
				foreach (var candGroupByCell in candsGroupByCell)
				{
					var cell = candGroupByCell.Key;
					var digitMask = (Mask)0;
					foreach (var cand in candGroupByCell)
					{
						digitMask |= (Mask)(1 << cand % 9);
					}

					// Bitwise not.
					foreach (var d in digitMask = (Mask)(Grid.MaxCandidatesMask & ~digitMask))
					{
						if (conjugatePairDigit == d || !CandidatesMap[d].Contains(cell))
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

				// BUG type 4.
				var step = new BivalueUniversalGraveType4Step(
					[.. conclusions],
					[
						[
							.. from candidate in trueCandidates select new CandidateViewNode(ColorIdentifier.Normal, candidate),
							new CandidateViewNode(ColorIdentifier.Auxiliary1, c1 * 9 + conjugatePairDigit),
							new CandidateViewNode(ColorIdentifier.Auxiliary1, c2 * 9 + conjugatePairDigit),
							new HouseViewNode(ColorIdentifier.Normal, house)
						]
					],
					context.Options,
					MaskOperations.Create(digits),
					in cells,
					new(c1, c2, conjugatePairDigit)
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

	/// <summary>
	/// Check for BUG + n.
	/// </summary>
	private BivalueUniversalGraveMultipleStep? CheckMultiple(
		List<Step> accumulator,
		ref AnalysisContext context,
		ref readonly CandidateMap trueCandidates,
		bool onlyFindOne
	)
	{
		if (trueCandidates.Count > 18)
		{
			return null;
		}

		if (trueCandidates.PeerIntersection is not { Count: var mapCount and not 0 } map)
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

		// BUG + n.
		var step = new BivalueUniversalGraveMultipleStep(
			[.. conclusions],
			[[.. from candidate in trueCandidates select new CandidateViewNode(ColorIdentifier.Normal, candidate)]],
			context.Options,
			in trueCandidates
		);
		if (onlyFindOne)
		{
			return step;
		}

		accumulator.Add(step);

		return null;
	}

	/// <summary>
	/// Check for BUG-XZ.
	/// </summary>
	private BivalueUniversalGraveXzStep? CheckXz(
		List<Step> accumulator,
		ref readonly Grid grid,
		ref AnalysisContext context,
		ref readonly CandidateMap trueCandidates,
		bool onlyFindOne
	)
	{
		if (trueCandidates is not [var cand1, var cand2])
		{
			return null;
		}

		var (c1, d1, c2, d2) = (cand1 / 9, cand1 % 9, cand2 / 9, cand2 % 9);
		var mask = (Mask)(1 << d1 | 1 << d2);
		foreach (var cell in (PeersMap[c1] ^ PeersMap[c2]) & BivalueCells)
		{
			if (grid.GetCandidates(cell) != mask)
			{
				continue;
			}

			// BUG-XZ found.
			var conclusions = new List<Conclusion>();
			var condition = (c1.AsCellMap() + cell).InOneHouse(out _);
			var anotherCell = condition ? c2 : c1;
			var anotherDigit = condition ? d2 : d1;
			foreach (var peer in (cell.AsCellMap() + anotherCell).PeerIntersection)
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

			var step = new BivalueUniversalGraveXzStep(
				[.. conclusions],
				[
					[
						new CellViewNode(ColorIdentifier.Normal, cell),
						.. from candidate in trueCandidates select new CandidateViewNode(ColorIdentifier.Normal, candidate)
					]
				],
				context.Options,
				mask,
				[c1, c2],
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
}
