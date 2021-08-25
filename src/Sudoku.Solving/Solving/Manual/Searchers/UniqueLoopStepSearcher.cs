using Sudoku.Solving.Manual.Steps.DeadlyPatterns.Loops;

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
internal sealed unsafe class UniqueLoopStepSearcher : IUniqueLoopStepSearcher
{
	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(10, DisplayingLevel.B);


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		// A valid unique loop must contain at least 6 bivalue cells.
		if (BivalueMap.Count < 6)
		{
			goto ReturnNull;
		}

		var resultAccumulator = new List<UniqueLoopStep>();
		var loops = new List<(Cells, IList<(Link, ColorIdentifier)>)>();
		var tempLoop = new List<int>(14);
		var loopMap = Cells.Empty;

		// Now iterate on each bi-value cells as the start cell to get all possible unique loops,
		// making it the start point to execute the recursion.
		IOrderedEnumerable<UniqueLoopStep> resultList = default!;
		foreach (int cell in BivalueMap)
		{
			short mask = grid.GetCandidates(cell);
			int d1 = TrailingZeroCount(mask), d2 = mask.GetNextSet(d1);

			loopMap.Clear();
			loops.Clear();
			tempLoop.Clear();

			SearchForPossibleLoopPatterns(grid, d1, d2, cell, ref loopMap, tempLoop, loops);

			if (loops.Count == 0)
			{
				continue;
			}

			short comparer = (short)(1 << d1 | 1 << d2);
			foreach (var (currentLoop, links) in loops)
			{
				var extraCellsMap = currentLoop - BivalueMap;
				switch (extraCellsMap.Count)
				{
					case 1:
					{
						// Type 1.
						CheckType1(resultAccumulator, grid, d1, d2, currentLoop, links, extraCellsMap);

						break;
					}
					case not 0:
					{
						// Type 2, 3, 4.
						// Here use default label to ensure the order of
						// the handling will be 1->2->3->4.
						CheckType2(resultAccumulator, grid, d1, d2, currentLoop, links, extraCellsMap, comparer);

						if (extraCellsMap.Count == 2)
						{
							CheckType3(resultAccumulator, grid, d1, d2, currentLoop, links, extraCellsMap, comparer);
							CheckType4(resultAccumulator, grid, d1, d2, currentLoop, links, extraCellsMap, comparer);
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
				from step in StepAccumulating.Distinct(resultAccumulator)
				orderby step.Loop.Count
				select step;

			if (onlyFindOne)
			{
				goto ReturnFirstElement;
			}

			accumulator.AddRange(resultList);
		}

	ReturnFirstElement:
		return resultList.First();

	ReturnNull:
		// Invalid cases ('onlyFindOne' is false, the grid doesn't exist any possible ULs, etc.).
		// Just return null is okay.
		return null;
	}

	/// <summary>
	/// Searches for possible unique loop patterns.
	/// </summary>
	/// <param name="grid">The sudoku grid.</param>
	/// <param name="d1">The first digit used.</param>
	/// <param name="d2">The second digit used.</param>
	/// <param name="cell">The current cell calculated.</param>
	/// <param name="loopMap">Indicates the map of the loop.</param>
	/// <param name="tempLoop">Indicates the cells used of the loop, queued.</param>
	/// <param name="loops">The possible loops found.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void SearchForPossibleLoopPatterns(
		in Grid grid, int d1, int d2, int cell, ref Cells loopMap,
		List<int> tempLoop, List<(Cells, IList<(Link, ColorIdentifier)>)> loops) =>
		SearchForPossibleLoopPatterns(grid, d1, d2, cell, (RegionLabel)255, 0, 2, ref loopMap, tempLoop, loops);

	/// <summary>
	/// Searches for possible unique loop patterns.
	/// </summary>
	/// <param name="grid">The sudoku grid.</param>
	/// <param name="d1">The first digit used.</param>
	/// <param name="d2">The second digit used.</param>
	/// <param name="cell">The current cell calculated.</param>
	/// <param name="lastLabel">Indicates the last label used.</param>
	/// <param name="exDigitsMask">The extra digits mask.</param>
	/// <param name="allowedExtraCellsCount">Indicates the number of cells can be with extra digits.</param>
	/// <param name="loopMap">Indicates the map of the loop.</param>
	/// <param name="tempLoop">Indicates the cells used of the loop, queued.</param>
	/// <param name="loops">The possible loops found.</param>
	private void SearchForPossibleLoopPatterns(
		in Grid grid, int d1, int d2, int cell, RegionLabel lastLabel, short exDigitsMask,
		int allowedExtraCellsCount, ref Cells loopMap, List<int> tempLoop,
		List<(Cells, IList<(Link, ColorIdentifier)>)> loops)
	{
		loopMap.AddAnyway(cell);
		tempLoop.Add(cell);

		for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
		{
			if (label == lastLabel)
			{
				continue;
			}

			int region = cell.ToRegion(label);
			var cellsMap = RegionMaps[region] & new Cells(EmptyMap) { ~cell };
			if (cellsMap.IsEmpty)
			{
				continue;
			}

			foreach (int nextCell in cellsMap)
			{
				if (tempLoop[0] == nextCell && tempLoop.Count >= 6 && Looping.IsValidLoop(tempLoop))
				{
					// The loop is closed. Now construct the result pair.
					loops.Add((loopMap, Looping.GetLinks(tempLoop)));
				}
				else if (!loopMap.Contains(nextCell) && grid[nextCell, d1] && grid[nextCell, d2])
				{
					// Here, unique loop can be found if and only if
					// two cells both contain 'd1' and 'd2'.
					// Incomplete ULs can't be found at present.
					short nextCellMask = grid.GetCandidates(nextCell);
					exDigitsMask |= nextCellMask;
					exDigitsMask &= (short)~((1 << d1) | (1 << d2));
					int digitsCount = PopCount((uint)nextCellMask);

					// We can continue if:
					// - The cell has exactly 2 digits of the loop.
					// - The cell has 1 extra digit, the same as all previous cells
					// with an extra digit (for type 2 only).
					// - The cell has extra digits and the maximum number of cells
					// with extra digits, 2, is not reached.
					if (digitsCount != 2
						&& (exDigitsMask == 0 || (exDigitsMask & exDigitsMask - 1) != 0)
						&& allowedExtraCellsCount <= 0)
					{
						continue;
					}

					SearchForPossibleLoopPatterns(
						grid, d1, d2, nextCell, label, exDigitsMask,
						digitsCount > 2 ? allowedExtraCellsCount - 1 : allowedExtraCellsCount,
						ref loopMap, tempLoop, loops
					);
				}
			}
		}

		// Backtrack.
		loopMap.Remove(cell);
		tempLoop.RemoveAt(tempLoop.Count - 1);
	}

	/// <summary>
	/// Check type 1.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="d1">The digit 1.</param>
	/// <param name="d2">The digit 2.</param>
	/// <param name="loop">The loop.</param>
	/// <param name="links">The links.</param>
	/// <param name="extraCellsMap">The extra cells map.</param>
	private void CheckType1(
		ICollection<UniqueLoopStep> accumulator, in Grid grid, int d1, int d2,
		in Cells loop, IList<(Link, ColorIdentifier)> links, in Cells extraCellsMap)
	{
		int extraCell = extraCellsMap[0];
		var conclusions = new List<Conclusion>(2);
		if (grid.Exists(extraCell, d1) is true) conclusions.Add(new(ConclusionType.Elimination, extraCell, d1));
		if (grid.Exists(extraCell, d2) is true) conclusions.Add(new(ConclusionType.Elimination, extraCell, d2));
		if (conclusions.Count == 0)
		{
			return;
		}

		var candidateOffsets = new List<(int, ColorIdentifier)>();
		foreach (int cell in new Cells(loop) { ~extraCell })
		{
			candidateOffsets.Add((cell * 9 + d1, (ColorIdentifier)0));
			candidateOffsets.Add((cell * 9 + d2, (ColorIdentifier)0));
		}

		accumulator.Add(
			new UniqueLoopType1Step(
				ImmutableArray.CreateRange(conclusions),
				ImmutableArray.Create(new PresentationData
				{
					Candidates = candidateOffsets,
					Links = links
				}),
				d1,
				d2,
				loop
			)
		);
	}

	/// <summary>
	/// Check type 2.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="d1">The digit 1.</param>
	/// <param name="d2">The digit 2.</param>
	/// <param name="loop">The loop.</param>
	/// <param name="links">The links.</param>
	/// <param name="extraCellsMap">The extra cells map.</param>
	/// <param name="comparer">The comparer mask (equals to <c><![CDATA[1 << d1 | 1 << d2]]></c>).</param>
	private void CheckType2(
		ICollection<UniqueLoopStep> accumulator, in Grid grid, int d1, int d2,
		in Cells loop, IList<(Link, ColorIdentifier)> links, in Cells extraCellsMap, short comparer)
	{
		short mask = 0;
		foreach (int cell in extraCellsMap)
		{
			mask |= grid.GetCandidates(cell);
		}
		mask &= (short)~comparer;

		if (mask == 0 || (mask & mask - 1) != 0)
		{
			return;
		}

		int extraDigit = TrailingZeroCount(mask);
		var elimMap = extraCellsMap % CandMaps[extraDigit];
		if (elimMap.IsEmpty)
		{
			return;
		}

		var candidateOffsets = new List<(int, ColorIdentifier)>();
		foreach (int cell in loop)
		{
			foreach (int digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)(digit == extraDigit ? 1 : 0)));
			}
		}

		accumulator.Add(
			new UniqueLoopType2Step(
				elimMap.ToImmutableConclusions(extraDigit),
				ImmutableArray.Create(new PresentationData
				{
					Candidates = candidateOffsets,
					Links = links
				}),
				d1,
				d2,
				loop,
				extraDigit
			)
		);
	}

	/// <summary>
	/// Check type 3.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="d1">The digit 1.</param>
	/// <param name="d2">The digit 2.</param>
	/// <param name="loop">The loop.</param>
	/// <param name="links">The links.</param>
	/// <param name="extraCellsMap">The extra cells map.</param>
	/// <param name="comparer">The comparer mask (equals to <c><![CDATA[1 << d1 | 1 << d2]]></c>).</param>
	private void CheckType3(
		ICollection<UniqueLoopStep> accumulator, in Grid grid, int d1, int d2, in Cells loop,
		IList<(Link, ColorIdentifier)> links, in Cells extraCellsMap, short comparer)
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
		if (!extraCellsMap.InOneRegion || notSatisfiedType3)
		{
			return;
		}

		short m = 0;
		foreach (int cell in extraCellsMap)
		{
			m |= grid.GetCandidates(cell);
		}
		if ((m & comparer) != comparer)
		{
			return;
		}

		short otherDigitsMask = (short)(m & ~comparer);
		foreach (int region in extraCellsMap.CoveredRegions)
		{
			if (!((ValueMaps[d1] | ValueMaps[d2]) & RegionMaps[region]).IsEmpty)
			{
				return;
			}

			int[] otherCells = ((RegionMaps[region] & EmptyMap) - loop).ToArray();
			for (int size = PopCount((uint)otherDigitsMask) - 1, count = otherCells.Length; size < count; size++)
			{
				foreach (int[] cells in otherCells.GetSubsets(size))
				{
					short mask = 0;
					foreach (int cell in cells)
					{
						mask |= grid.GetCandidates(cell);
					}

					if (PopCount((uint)mask) != size + 1 || (mask & otherDigitsMask) != otherDigitsMask)
					{
						continue;
					}

					var elimMap = (RegionMaps[region] & EmptyMap) - cells - loop;
					if (elimMap.IsEmpty)
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					foreach (int digit in mask)
					{
						foreach (int cell in elimMap & CandMaps[digit])
						{
							conclusions.Add(new(ConclusionType.Elimination, cell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<(int, ColorIdentifier)>();
					foreach (int cell in loop)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(
								(
									cell * 9 + digit,
									(ColorIdentifier)((otherDigitsMask >> digit & 1) != 0 ? 1 : 0)
								)
							);
						}
					}
					foreach (int cell in cells)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)1));
						}
					}

					accumulator.Add(
						new UniqueLoopType3Step(
							ImmutableArray.CreateRange(conclusions),
							ImmutableArray.Create(new PresentationData
							{
								Candidates = candidateOffsets,
								Regions = new[] { (region, (ColorIdentifier)0) },
								Links = links
							}),
							d1,
							d2,
							loop,
							mask,
							cells
						)
					);
				}
			}
		}
	}

	/// <summary>
	/// Check type 4.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="d1">The digit 1.</param>
	/// <param name="d2">The digit 2.</param>
	/// <param name="loop">The loop.</param>
	/// <param name="links">The links.</param>
	/// <param name="extraCellsMap">The extra cells map.</param>
	/// <param name="comparer">The comparer mask (equals to <c><![CDATA[1 << d1 | 1 << d2]]></c>).</param>
	private void CheckType4(
		ICollection<UniqueLoopStep> accumulator, in Grid grid, int d1, int d2,
		in Cells loop, IList<(Link, ColorIdentifier)> links, in Cells extraCellsMap, short comparer)
	{
		if (!extraCellsMap.InOneRegion)
		{
			return;
		}

		var digitPairs = stackalloc[] { (d1, d2), (d2, d1) };
		foreach (int region in extraCellsMap.CoveredRegions)
		{
			for (int digitPairIndex = 0; digitPairIndex < 2; digitPairIndex++)
			{
				var (digit, otherDigit) = digitPairs[digitPairIndex];
				var map = RegionMaps[region] & CandMaps[digit];
				if (map != (RegionMaps[region] & loop))
				{
					continue;
				}

				int first = extraCellsMap[0], second = extraCellsMap[1];
				var conclusions = new List<Conclusion>(2);
				if (grid.Exists(first, otherDigit) is true)
				{
					conclusions.Add(new(ConclusionType.Elimination, first, otherDigit));
				}
				if (grid.Exists(second, otherDigit) is true)
				{
					conclusions.Add(new(ConclusionType.Elimination, second, otherDigit));
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<(int, ColorIdentifier)>();
				foreach (int cell in loop - extraCellsMap)
				{
					foreach (int d in grid.GetCandidates(cell))
					{
						candidateOffsets.Add((cell * 9 + d, (ColorIdentifier)0));
					}
				}
				foreach (int cell in extraCellsMap)
				{
					candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)1));
				}

				accumulator.Add(
					new UniqueLoopType4Step(
						ImmutableArray.CreateRange(conclusions),
						ImmutableArray.Create(new PresentationData
						{
							Candidates = candidateOffsets,
							Regions = new[] { (region, (ColorIdentifier)0) },
							Links = links
						}),
						d1,
						d2,
						loop,
						new(first, second, digit)
					)
				);
			}
		}
	}
}
