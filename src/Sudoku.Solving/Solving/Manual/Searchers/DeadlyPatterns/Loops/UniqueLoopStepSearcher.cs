namespace Sudoku.Solving.Manual.Searchers.DeadlyPatterns.Loops;

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
[StepSearcher]
public sealed unsafe class UniqueLoopStepSearcher : IUniqueLoopStepSearcher, IUniqueLoopOrBivalueOddagonStepSearcher
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

			IUniqueLoopOrBivalueOddagonStepSearcher.SearchForPossibleLoopPatterns(
				grid, d1, d2, cell, (RegionLabel)255, 0, 2, ref loopMap,
				tempLoop, () => IUniqueLoopStepSearcher.IsValidLoop(tempLoop), loops
			);

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
					case 0:
					{
						throw new NoSolutionException(grid);
					}
					case 1:
					{
						if (CheckType1(resultAccumulator, grid, d1, d2, currentLoop, links, extraCellsMap, onlyFindOne) is { } step1)
						{
							return step1;
						}

						break;
					}
					default:
					{
						// Type 2, 3, 4.
						// Here use default label to ensure the order of
						// the handling will be 1->2->3->4.
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
							if (CheckType4(resultAccumulator, grid, d1, d2, currentLoop, links, extraCellsMap, comparer, onlyFindOne) is { } step4)
							{
								return step4;
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
				from step in IDistinctableStep<UniqueLoopStep>.Distinct(resultAccumulator)
				orderby step.Loop.Count
				select step;

			if (onlyFindOne)
			{
				goto ReturnFirstElement;
			}

			accumulator.AddRange(resultList);
		}

	ReturnNull:
		// Invalid cases ('onlyFindOne' is false, the grid doesn't exist any possible ULs, etc.).
		// Just return null is okay.
		return null;

	ReturnFirstElement:
		return resultList.First();
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
	/// <param name="onlyFindOne">Indicates whether the searcher only searching for one step is okay.</param>
	/// <returns>The step is worth.</returns>
	private Step? CheckType1(
		ICollection<UniqueLoopStep> accumulator, in Grid grid, int d1, int d2, in Cells loop,
		IList<(Link, ColorIdentifier)> links, in Cells extraCellsMap, bool onlyFindOne)
	{
		int extraCell = extraCellsMap[0];
		var conclusions = new List<Conclusion>(2);
		if (grid.Exists(extraCell, d1) is true) conclusions.Add(new(ConclusionType.Elimination, extraCell, d1));
		if (grid.Exists(extraCell, d2) is true) conclusions.Add(new(ConclusionType.Elimination, extraCell, d2));
		if (conclusions.Count == 0)
		{
			goto ReturnNull;
		}

		var candidateOffsets = new List<(int, ColorIdentifier)>();
		foreach (int cell in new Cells(loop) { ~extraCell })
		{
			candidateOffsets.Add((cell * 9 + d1, (ColorIdentifier)0));
			candidateOffsets.Add((cell * 9 + d2, (ColorIdentifier)0));
		}

		var step = new UniqueLoopType1Step(
			ImmutableArray.CreateRange(conclusions),
			ImmutableArray.Create(new PresentationData
			{
				Candidates = candidateOffsets,
				Links = links
			}),
			d1,
			d2,
			loop
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
	/// <param name="d1">The digit 1.</param>
	/// <param name="d2">The digit 2.</param>
	/// <param name="loop">The loop.</param>
	/// <param name="links">The links.</param>
	/// <param name="extraCellsMap">The extra cells map.</param>
	/// <param name="comparer">The comparer mask (equals to <c><![CDATA[1 << d1 | 1 << d2]]></c>).</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searching for one step is okay.</param>
	/// <returns>The step is worth.</returns>
	private Step? CheckType2(
		ICollection<UniqueLoopStep> accumulator, in Grid grid, int d1, int d2, in Cells loop,
		IList<(Link, ColorIdentifier)> links, in Cells extraCellsMap, short comparer, bool onlyFindOne)
	{
		short mask = 0;
		foreach (int cell in extraCellsMap)
		{
			mask |= grid.GetCandidates(cell);
		}
		mask &= (short)~comparer;

		if (mask == 0 || (mask & mask - 1) != 0)
		{
			return null;
		}

		int extraDigit = TrailingZeroCount(mask);
		var elimMap = extraCellsMap % CandMaps[extraDigit];
		if (elimMap.IsEmpty)
		{
			goto ReturnNull;
		}

		var candidateOffsets = new List<(int, ColorIdentifier)>();
		foreach (int cell in loop)
		{
			foreach (int digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)(digit == extraDigit ? 1 : 0)));
			}
		}

		var step = new UniqueLoopType2Step(
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
	/// <param name="d1">The digit 1.</param>
	/// <param name="d2">The digit 2.</param>
	/// <param name="loop">The loop.</param>
	/// <param name="links">The links.</param>
	/// <param name="extraCellsMap">The extra cells map.</param>
	/// <param name="comparer">The comparer mask (equals to <c><![CDATA[1 << d1 | 1 << d2]]></c>).</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searching for one step is okay.</param>
	/// <returns>The step is worth.</returns>
	private Step? CheckType3(
		ICollection<UniqueLoopStep> accumulator, in Grid grid, int d1, int d2, in Cells loop,
		IList<(Link, ColorIdentifier)> links, in Cells extraCellsMap, short comparer, bool onlyFindOne)
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
			goto ReturnNull;
		}

		short m = 0;
		foreach (int cell in extraCellsMap)
		{
			m |= grid.GetCandidates(cell);
		}
		if ((m & comparer) != comparer)
		{
			goto ReturnNull;
		}

		short otherDigitsMask = (short)(m & ~comparer);
		foreach (int region in extraCellsMap.CoveredRegions)
		{
			if (!((ValueMaps[d1] | ValueMaps[d2]) & RegionMaps[region]).IsEmpty)
			{
				continue;
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

					var step = new UniqueLoopType3Step(
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
	/// <param name="onlyFindOne">Indicates whether the searcher only searching for one step is okay.</param>
	/// <returns>The step is worth.</returns>
	private Step? CheckType4(
		ICollection<UniqueLoopStep> accumulator, in Grid grid, int d1, int d2, in Cells loop,
		IList<(Link, ColorIdentifier)> links, in Cells extraCellsMap, short comparer, bool onlyFindOne)
	{
		if (!extraCellsMap.InOneRegion)
		{
			goto ReturnNull;
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

				var step = new UniqueLoopType4Step(
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
				);

				if (onlyFindOne)
				{
					return step;
				}

				accumulator.Add(step);
			}
		}

	ReturnNull:
		return null;
	}
}
