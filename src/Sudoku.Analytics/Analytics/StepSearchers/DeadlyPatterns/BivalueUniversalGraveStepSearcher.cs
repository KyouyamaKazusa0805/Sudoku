namespace Sudoku.Analytics.StepSearchers;

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
[StepSearcher]
public sealed partial class BivalueUniversalGraveStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates whether the searcher should call the extended BUG checker
	/// to search for all true candidates no matter how difficult searching.
	/// </summary>
	public bool SearchExtendedTypes { get; set; }


	/// <inheritdoc/>
	protected internal override Step? GetAll(scoped ref AnalysisContext context)
	{
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
	/// <param name="context"><inheritdoc cref="GetAll(ref AnalysisContext)" path="/param[@name='context']"/></param>
	/// <returns><inheritdoc cref="GetAll(ref AnalysisContext)" path="/returns"/></returns>
	private Step? CheckForTrueCandidateTypes(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		if (!Cached.FindTrueCandidates(grid, out var trueCandidates))
		{
			return null;
		}

		switch (trueCandidates)
		{
			case []:
			{
				throw new StepSearcherProcessException<BivalueUniversalGraveStepSearcher>();
			}
			case [var trueCandidate]:
			{
				// BUG + 1 found.
				var step = new BivalueUniversalGraveType1Step(
					new[] { new Conclusion(Assignment, trueCandidate) },
					new[] { View.Empty | new CandidateViewNode(DisplayColorKind.Normal, trueCandidate) }
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

	/// <summary>
	/// Check for types that uses false candidates.
	/// </summary>
	/// <param name="context"><inheritdoc cref="GetAll(ref AnalysisContext)" path="/param[@name='context']"/></param>
	/// <returns><inheritdoc cref="GetAll(ref AnalysisContext)" path="/returns"/></returns>
	private Step? CheckForFalseCandidateTypes(scoped ref AnalysisContext context)
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

				if (!Cached.FormsPattern(copied))
				{
					continue;
				}

				var cellOffsets = new List<CellViewNode>(multivalueCells.Count);
				foreach (var multiValueCell in multivalueCells)
				{
					cellOffsets.Add(new(DisplayColorKind.Normal, multiValueCell));
				}

				var step = new BivalueUniversalGraveFalseCandidateTypeStep(
					new[] { new Conclusion(Elimination, cell, digit) },
					new[] { View.Empty | cellOffsets },
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

	/// <summary>
	/// Check for type 2.
	/// </summary>
	private Step? CheckType2(List<Step> accumulator, IReadOnlyList<int> trueCandidates, bool onlyFindOne)
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
		var step = new BivalueUniversalGraveType2Step(conclusions.ToArray(), new[] { View.Empty | candidateOffsets }, digit, (CellMap)cells);
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
	private Step? CheckType3Naked(List<Step> accumulator, scoped in Grid grid, IReadOnlyList<int> trueCandidates, bool onlyFindOne)
	{
		// Check whether all true candidates lie in a same house.
		var map = CellMap.Empty + from c in trueCandidates group c by c / 9 into z select z.Key;
		if (!map.InOneHouse)
		{
			return null;
		}

		// Get the digit mask.
		var digitsMask = (short)0;
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
						conclusions.ToArray(),
						new[] { View.Empty | candidateOffsets | new HouseViewNode(DisplayColorKind.Normal, house) },
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

	/// <summary>
	/// Check for type 4.
	/// </summary>
	private Step? CheckType4(List<Step> accumulator, IReadOnlyList<int> trueCandidates, bool onlyFindOne)
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
				var c1 = HouseCells[house][first];
				var c2 = HouseCells[house][second];
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
					var digitMask = (short)0;
					foreach (var cand in candGroupByCell)
					{
						digitMask |= (short)(1 << cand % 9);
					}

					// Bitwise not.
					foreach (var d in digitMask = (short)(~digitMask & Grid.MaxCandidatesMask))
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

				var candidateOffsets = new CandidateViewNode[trueCandidates.Count + 2];
				var i = 0;
				foreach (var candidate in trueCandidates)
				{
					candidateOffsets[i++] = new(DisplayColorKind.Normal, candidate);
				}
				candidateOffsets[^2] = new(DisplayColorKind.Auxiliary1, c1 * 9 + conjugatePairDigit);
				candidateOffsets[^1] = new(DisplayColorKind.Auxiliary1, c2 * 9 + conjugatePairDigit);

				// BUG type 4.
				var digitsMask = (short)0;
				foreach (var digit in digits)
				{
					digitsMask |= (short)(1 << digit);
				}

				var step = new BivalueUniversalGraveType4Step(
					conclusions.ToArray(),
					new[] { View.Empty | candidateOffsets | new HouseViewNode(DisplayColorKind.Normal, house) },
					digitsMask,
					CellMap.Empty + cells,
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
	private Step? CheckMultiple(List<Step> accumulator, IReadOnlyList<int> trueCandidates, bool onlyFindOne)
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
		var step = new BivalueUniversalGraveMultipleStep(conclusions.ToArray(), new[] { View.Empty | candidateOffsets }, trueCandidates);
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
	private Step? CheckXz(List<Step> accumulator, scoped in Grid grid, IReadOnlyList<int> trueCandidates, bool onlyFindOne)
	{
		if (trueCandidates is not [var cand1, var cand2])
		{
			return null;
		}

		var c1 = cand1 / 9;
		var c2 = cand2 / 9;
		var d1 = cand1 % 9;
		var d2 = cand2 % 9;
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
				conclusions.ToArray(),
				new[] { View.Empty | new CellViewNode(DisplayColorKind.Normal, cell) | candidateOffsets },
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

/// <summary>
/// Represents a cached gathering operation set.
/// </summary>
file static class Cached
{
	/// <summary>
	/// Finds all possible true candidates in this current grid.
	/// </summary>
	/// <param name="grid">The grid to find all possible true candidates.</param>
	/// <param name="trueCandidates">All possible true candidates returned.</param>
	/// <param name="maximumCellsToCheck">Indicates the maximum number of possible cells to check.</param>
	/// <returns>
	/// A <see cref="bool"/> value indicating whether the grid contains any possible true candidates.
	/// If so, the argument <paramref name="trueCandidates"/> won't be <see langword="null"/>.
	/// </returns>
	/// <remarks>
	/// A <b>true candidate</b> is a candidate that makes the puzzle containing no valid solution
	/// if it is eliminated from the current puzzle. It is a strict concept, which means sometimes the puzzle
	/// doesn't contain any satisfied candidate to make the puzzle containing no solution if being eliminated.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	/// Throws when the puzzle contains multiple solutions or even no solution.
	/// </exception>
	public static bool FindTrueCandidates(
		scoped in Grid grid,
		[NotNullWhen(true)] out IReadOnlyList<int>? trueCandidates,
		int maximumCellsToCheck = 20
	)
	{
		Argument.ThrowIfInvalid(grid.IsValid(), "The puzzle must be valid (containing a unique solution).");

		// Get the number of multi-value cells.
		// If the number of that is greater than the specified number,
		// here will return the default list directly.
		var multivalueCellsCount = 0;
		foreach (var value in EmptyCells)
		{
			switch (PopCount((uint)grid.GetCandidates(value)))
			{
				case 1: // The grid contains a naked single. We don't check on this grid now.
				case > 2 when ++multivalueCellsCount > maximumCellsToCheck:
				{
					goto ReturnFalse;
				}
			}
		}

		// Store all bi-value cells and construct the relations.
		scoped var span = (stackalloc int[3]);
		var stack = new CellMap[multivalueCellsCount + 1, 9];
		foreach (var cell in BivalueCells)
		{
			foreach (var digit in grid.GetCandidates(cell))
			{
				scoped ref var map = ref stack[0, digit];
				map.Add(cell);

				cell.CopyHouseInfo(ref span[0]);
				foreach (var house in span)
				{
					if ((map & HousesMap[house]).Count > 2)
					{
						// The specified house contains at least three positions to fill with the digit,
						// which is invalid in any BUG + n patterns.
						goto ReturnFalse;
					}
				}
			}
		}

		// Store all multi-value cells.
		// Suppose the pattern is the simplest BUG + 1 pattern (i.e. Only one multi-value cell).
		// The comments will help you to understand the processing.
		SkipInit(out short mask);
		var pairs = new short[multivalueCellsCount, 37]; // 37 == (1 + 8) * 8 / 2 + 1
		var multivalueCells = (EmptyCells - BivalueCells).ToArray();
		for (var i = 0; i < multivalueCells.Length; i++)
		{
			// e.g. { 2, 4, 6 } (42)
			mask = grid.GetCandidates(multivalueCells[i]);

			// e.g. { 2, 4 }, { 4, 6 }, { 2, 6 } (10, 40, 34)
			var pairList = MaskOperations.GetMaskSubsets(mask, 2);

			// e.g. pairs[i, ..] = { 3, { 2, 4 }, { 4, 6 }, { 2, 6 } } ({ 3, 10, 40, 34 })
			pairs[i, 0] = (short)pairList.Length;
			for (var z = 1; z <= pairList.Length; z++)
			{
				pairs[i, z] = pairList[z - 1];
			}
		}

		// Now check the pattern.
		// If the pattern is a valid BUG + n, the processing here will give you one plan of all possible
		// combinations; otherwise, none will be found.
		scoped var playground = (stackalloc int[3]);
		var currentIndex = 1;
		var chosen = new int[multivalueCellsCount + 1];
		var resultMap = new CellMap[9];
		var result = new List<int>();
		do
		{
			int i;
			var currentCell = multivalueCells[currentIndex - 1];
			var @continue = false;
			for (i = chosen[currentIndex] + 1; i <= pairs[currentIndex - 1, 0]; i++)
			{
				@continue = true;
				mask = pairs[currentIndex - 1, i];
				foreach (var digit in pairs[currentIndex - 1, i])
				{
					var temp = stack[currentIndex - 1, digit];
					temp.Add(currentCell);

					currentCell.CopyHouseInfo(ref playground[0]);
					foreach (var house in playground)
					{
						if ((temp & HousesMap[house]).Count > 2)
						{
							@continue = false;
							break;
						}
					}

					if (!@continue) { break; }
				}

				if (@continue) { break; }
			}

			if (@continue)
			{
				for (var z = 0; z < stack.GetLength(1); z++)
				{
					stack[currentIndex, z] = stack[currentIndex - 1, z];
				}

				chosen[currentIndex] = i;
				var pos1 = TrailingZeroCount(mask);

				stack[currentIndex, pos1].Add(currentCell);
				stack[currentIndex, mask.GetNextSet(pos1)].Add(currentCell);
				if (currentIndex == multivalueCellsCount)
				{
					// Iterate on each digit.
					for (var digit = 0; digit < 9; digit++)
					{
						// Take the cell that doesn't contain in the map above.
						// Here, the cell is the "true candidate cell".
						scoped ref var map = ref resultMap[digit];
						map = CandidatesMap[digit] - stack[currentIndex, digit];
						foreach (var cell in map)
						{
							result.Add(cell * 9 + digit);
						}
					}

					goto ReturnTrue;
				}
				else
				{
					currentIndex++;
				}
			}
			else
			{
				chosen[currentIndex--] = 0;
			}
		} while (currentIndex > 0);

	ReturnTrue:
		trueCandidates = result;
		return true;

	ReturnFalse:
		trueCandidates = null;
		return false;
	}

	/// <summary>
	/// Checks whether the specified grid forms a BUG deadly pattern.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool FormsPattern(scoped in Grid grid)
	{
		_ = grid is { BivalueCells: var bivalueCells, EmptyCells: var emptyCells, CandidatesMap: var candidatesMap };
		if (bivalueCells != emptyCells)
		{
			return false;
		}

		scoped var housesCount = (stackalloc int[3]);
		foreach (var cell in emptyCells)
		{
			foreach (var digit in grid.GetCandidates(cell))
			{
				housesCount.Clear();

				for (var i = 0; i < 3; i++)
				{
					housesCount[i] = (candidatesMap[digit] & HousesMap[cell.ToHouseIndex((HouseType)i)]).Count;
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
