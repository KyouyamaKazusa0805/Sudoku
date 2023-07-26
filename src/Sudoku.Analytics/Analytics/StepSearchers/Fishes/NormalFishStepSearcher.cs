namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Normal Fish</b> step searcher. The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Normal fishes:
/// <list type="bullet">
/// <item>X-Wing</item>
/// <item>Swordfish</item>
/// <item>Jellyfish</item>
/// </list>
/// </item>
/// <item>
/// Finned fishes:
/// <list type="bullet">
/// <item>
/// Finned normal fishes:
/// <list type="bullet">
/// <item>Finned X-Wing</item>
/// <item>Finned Swordfish</item>
/// <item>Finned Jellyfish</item>
/// </list>
/// </item>
/// <item>
/// Finned sashimi fishes:
/// <list type="bullet">
/// <item>Sashimi X-Wing</item>
/// <item>Sashimi Swordfish</item>
/// <item>Sashimi Jellyfish</item>
/// </list>
/// </item>
/// </list>
/// </item>
/// </list>
/// </summary>
[StepSearcher(
	Technique.XWing, Technique.Swordfish, Technique.Jellyfish,
	Technique.Squirmbag, Technique.Whale, Technique.Leviathan,
	Technique.FinnedXWing, Technique.FinnedSwordfish, Technique.FinnedJellyfish,
	Technique.FinnedSquirmbag, Technique.FinnedWhale, Technique.FinnedLeviathan,
	Technique.SashimiXWing, Technique.SashimiSwordfish, Technique.SashimiJellyfish,
	Technique.SashimiSquirmbag, Technique.SashimiWhale, Technique.SashimiLeviathan,
	Technique.SiameseFinnedXWing, Technique.SiameseFinnedSwordfish, Technique.SiameseFinnedJellyfish,
	Technique.SiameseFinnedSquirmbag, Technique.SiameseFinnedWhale, Technique.SiameseFinnedLeviathan,
	Technique.SiameseSashimiXWing, Technique.SiameseSashimiSwordfish, Technique.SiameseSashimiJellyfish,
	Technique.SiameseSashimiSquirmbag, Technique.SiameseSashimiWhale, Technique.SiameseSashimiLeviathan)]
public sealed partial class NormalFishStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override unsafe Step? Collect(scoped ref AnalysisContext context)
	{
		var r = stackalloc int*[9];
		var c = stackalloc int*[9];
		InitBlock(r, 0, (uint)sizeof(int*) * 9);
		InitBlock(c, 0, (uint)sizeof(int*) * 9);

		scoped ref readonly var grid = ref context.Grid;
		var accumulator = context.Accumulator!;
		var onlyFindOne = context.OnlyFindOne;
		for (var digit = 0; digit < 9; digit++)
		{
			if (ValuesMap[digit].Count > 5)
			{
				continue;
			}

			// Gather.
			for (var house = 9; house < 27; house++)
			{
				if (HousesMap[house] & CandidatesMap[digit])
				{
#pragma warning disable CA2014
					if (house < 18)
					{
						if (r[digit] == null)
						{
							var ptr = stackalloc int[10];
							InitBlock(ptr, 0, 10 * sizeof(int));

							r[digit] = ptr;
						}

						r[digit][++r[digit][0]] = house;
					}
					else
					{
						if (c[digit] == null)
						{
							var ptr = stackalloc int[10];
							InitBlock(ptr, 0, 10 * sizeof(int));

							c[digit] = ptr;
						}

						c[digit][++c[digit][0]] = house;
					}
#pragma warning restore CA2014
				}
			}
		}

		for (var size = 2; size <= 4; size++)
		{
			if (Collect(accumulator, grid, size, r, c, false, true, onlyFindOne) is { } finlessRowFish)
			{
				return finlessRowFish;
			}
			if (Collect(accumulator, grid, size, r, c, false, false, onlyFindOne) is { } finlessColumnFish)
			{
				return finlessColumnFish;
			}
			if (Collect(accumulator, grid, size, r, c, true, true, onlyFindOne) is { } finnedRowFish)
			{
				return finnedRowFish;
			}
			if (Collect(accumulator, grid, size, r, c, true, false, onlyFindOne) is { } finnedColumnFish)
			{
				return finnedColumnFish;
			}
		}

		return null;
	}

	/// <summary>
	/// Get all possible normal fishes.
	/// </summary>
	/// <param name="accumulator">The accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="size">The size.</param>
	/// <param name="r">The possible row table to iterate.</param>
	/// <param name="c">The possible column table to iterate.</param>
	/// <param name="withFin">Indicates whether the searcher will check for the existence of fins.</param>
	/// <param name="searchRow">
	/// Indicates whether the searcher searches for fishes in the direction of rows.
	/// </param>
	/// <param name="onlyFindOne">Indicates whether the method only searches for one step.</param>
	/// <returns>The first found step.</returns>
	private unsafe NormalFishStep? Collect(
		List<Step> accumulator,
		scoped in Grid grid,
		int size,
		int** r,
		int** c,
		bool withFin,
		bool searchRow,
		bool onlyFindOne
	)
	{
		// Iterate on each digit.
		for (var digit = 0; digit < 9; digit++)
		{
			// Check the validity of the distribution for the current digit.
			var pBase = searchRow ? r[digit] : c[digit];
			var pCover = searchRow ? c[digit] : r[digit];
			if (pBase == null || pBase[0] <= size)
			{
				continue;
			}

			// Iterate on the base set combination.
			foreach (var bs in PointerOperations.GetArrayFromStart(pBase, 10, 1, true).GetSubsets(size))
			{
				// 'baseLine' is the map that contains all base set cells.
				var baseLine = size switch
				{
					2 => CandidatesMap[digit] & (HousesMap[bs[0]] | HousesMap[bs[1]]),
					3 => CandidatesMap[digit] & (HousesMap[bs[0]] | HousesMap[bs[1]] | HousesMap[bs[2]]),
					4 => CandidatesMap[digit] & (HousesMap[bs[0]] | HousesMap[bs[1]] | HousesMap[bs[2]] | HousesMap[bs[3]])
				};

				// Iterate on the cover set combination.
				foreach (var cs in PointerOperations.GetArrayFromStart(pCover, 10, 1, true).GetSubsets(size))
				{
					// 'coverLine' is the map that contains all cover set cells.
					var coverLine = size switch
					{
						2 => CandidatesMap[digit] & (HousesMap[cs[0]] | HousesMap[cs[1]]),
						3 => CandidatesMap[digit] & (HousesMap[cs[0]] | HousesMap[cs[1]] | HousesMap[cs[2]]),
						4 => CandidatesMap[digit] & (HousesMap[cs[0]] | HousesMap[cs[1]] | HousesMap[cs[2]] | HousesMap[cs[3]])
					};

					// Now check the fins and the elimination cells.
					CellMap elimMap, fins = CellMap.Empty;
					if (!withFin)
					{
						// If the current searcher doesn't check fins, we'll just get the pure check:
						// 1. Base set contain more cells than cover sets.
						// 2. Elimination cells set isn't empty.
						if (baseLine - coverLine)
						{
							continue;
						}

						elimMap = coverLine - baseLine;
						if (!elimMap)
						{
							continue;
						}
					}
					else // Should check fins.
					{
						// All fins should be in the same block.
						var blockMask = (fins = baseLine - coverLine).BlockMask;
						if (!fins || !IsPow2(blockMask))
						{
							continue;
						}

						// Cover set shouldn't overlap with the block of all fins lying in.
						var finBlock = TrailingZeroCount(blockMask);
						if (!(coverLine & HousesMap[finBlock]))
						{
							continue;
						}

						// Don't intersect.
						if (!(HousesMap[finBlock] & coverLine - baseLine))
						{
							continue;
						}

						// Finally, get the elimination cells.
						elimMap = coverLine - baseLine & HousesMap[finBlock];
					}

					// Gather the conclusions and candidates or houses to be highlighted.
					var candidateOffsets = new List<CandidateViewNode>();
					var houseOffsets = new List<HouseViewNode>();
					foreach (var cell in withFin ? baseLine - fins : baseLine)
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
					}
					if (withFin)
					{
						foreach (var cell in fins)
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, cell * 9 + digit));
						}
					}
					foreach (var baseSet in bs)
					{
						houseOffsets.Add(new(WellKnownColorIdentifier.Normal, baseSet));
					}
					foreach (var coverSet in cs)
					{
						houseOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, coverSet));
					}

					var (baseSetsMask, coverSetsMask) = (0, 0);
					foreach (var baseSet in bs)
					{
						baseSetsMask |= 1 << baseSet;
					}
					foreach (var coverSet in cs)
					{
						coverSetsMask |= 1 << coverSet;
					}

					// Gather the result.
					var step = new NormalFishStep(
						from cell in elimMap select new Conclusion(Elimination, cell, digit),
						[[.. candidateOffsets, .. houseOffsets], GetDirectView(digit, bs, cs, fins, searchRow)],
						digit,
						baseSetsMask,
						coverSetsMask,
						fins,
						FishStepSearcherHelper.IsSashimi(bs, fins, digit)
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
	/// Get the direct fish view with the specified grid and the base sets.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <param name="baseSets">The base sets.</param>
	/// <param name="coverSets">The cover sets.</param>
	/// <param name="fins">The cells of the fin in the current fish.</param>
	/// <param name="searchRow">Indicates whether the current searcher searches row.</param>
	/// <returns>The view.</returns>
	private static View GetDirectView(Digit digit, House[] baseSets, House[] coverSets, scoped in CellMap fins, bool searchRow)
	{
		// Get the highlighted cells (necessary).
		var cellOffsets = new List<CellViewNode>();
		var candidateOffsets = fins ? new List<CandidateViewNode>() : null;
		foreach (var baseSet in baseSets)
		{
			foreach (var cell in HousesMap[baseSet])
			{
				switch (CandidatesMap[digit].Contains(cell))
				{
					case true when fins.Contains(cell):
					{
						cellOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, cell));
						break;
					}
					default:
					{
						var flag = false;
						foreach (var c in ValuesMap[digit])
						{
							if (HousesMap[c.ToHouseIndex(searchRow ? HouseType.Column : HouseType.Row)].Contains(cell))
							{
								flag = true;
								break;
							}
						}
						if (flag)
						{
							continue;
						}

						var (baseMap, coverMap) = (CellMap.Empty, CellMap.Empty);
						foreach (var b in baseSets)
						{
							baseMap |= HousesMap[b];
						}
						foreach (var c in coverSets)
						{
							coverMap |= HousesMap[c];
						}
						baseMap &= coverMap;
						if (baseMap.Contains(cell))
						{
							continue;
						}

						cellOffsets.Add(new(WellKnownColorIdentifier.Normal, cell) { RenderingMode = RenderingMode.BothDirectAndPencilmark });
						break;
					}
				}
			}
		}

		foreach (var cell in ValuesMap[digit])
		{
			cellOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, cell) { RenderingMode = RenderingMode.BothDirectAndPencilmark });
		}
		foreach (var cell in fins)
		{
			candidateOffsets!.Add(new(WellKnownColorIdentifier.Auxiliary1, cell * 9 + digit));
		}

		return [.. cellOffsets, .. candidateOffsets ?? []];
	}
}
