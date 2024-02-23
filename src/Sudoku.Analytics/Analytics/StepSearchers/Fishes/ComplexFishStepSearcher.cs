namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Complex Fish</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Franken Fishes
/// <list type="bullet">
/// <item>Finned Franken Fish</item>
/// <item>Sashimi Franken Fish</item>
/// </list>
/// </item>
/// <item>
/// Mutant Fishes
/// <list type="bullet">
/// <item>Finned Mutant Fish</item>
/// <item>Sashimi Mutant Fish</item>
/// </list>
/// </item>
/// </list>
/// </summary>
[StepSearcher(
	Technique.FrankenXWing, Technique.FrankenSwordfish, Technique.FrankenJellyfish,
	Technique.FrankenSquirmbag, Technique.FrankenWhale, Technique.FrankenLeviathan,
	Technique.MutantXWing, Technique.MutantSwordfish, Technique.MutantJellyfish,
	Technique.MutantSquirmbag, Technique.MutantWhale, Technique.MutantLeviathan,
	Technique.FinnedFrankenXWing, Technique.FinnedFrankenSwordfish, Technique.FinnedFrankenJellyfish,
	Technique.FinnedFrankenSquirmbag, Technique.FinnedFrankenWhale, Technique.FinnedFrankenLeviathan,
	Technique.SashimiFrankenXWing, Technique.SashimiFrankenSwordfish, Technique.SashimiFrankenJellyfish,
	Technique.SashimiFrankenSquirmbag, Technique.SashimiFrankenWhale, Technique.SashimiFrankenLeviathan,
	Technique.SiameseFinnedFrankenXWing, Technique.SiameseFinnedFrankenSwordfish, Technique.SiameseFinnedFrankenJellyfish,
	Technique.SiameseFinnedFrankenSquirmbag, Technique.SiameseFinnedFrankenWhale, Technique.SiameseFinnedFrankenLeviathan,
	Technique.SiameseSashimiFrankenXWing, Technique.SiameseSashimiFrankenSwordfish, Technique.SiameseSashimiFrankenJellyfish,
	Technique.SiameseSashimiFrankenSquirmbag, Technique.SiameseSashimiFrankenWhale, Technique.SiameseSashimiFrankenLeviathan,
	Technique.FinnedMutantXWing, Technique.FinnedMutantSwordfish, Technique.FinnedMutantJellyfish,
	Technique.FinnedMutantSquirmbag, Technique.FinnedMutantWhale, Technique.FinnedMutantLeviathan,
	Technique.SashimiMutantXWing, Technique.SashimiMutantSwordfish, Technique.SashimiMutantJellyfish,
	Technique.SashimiMutantSquirmbag, Technique.SashimiMutantWhale, Technique.SashimiMutantLeviathan,
	Technique.SiameseFinnedMutantXWing, Technique.SiameseFinnedMutantSwordfish, Technique.SiameseFinnedMutantJellyfish,
	Technique.SiameseFinnedMutantSquirmbag, Technique.SiameseFinnedMutantWhale, Technique.SiameseFinnedMutantLeviathan,
	Technique.SiameseSashimiMutantXWing, Technique.SiameseSashimiMutantSwordfish, Technique.SiameseSashimiMutantJellyfish,
	Technique.SiameseSashimiMutantSquirmbag, Technique.SiameseSashimiMutantWhale, Technique.SiameseSashimiMutantLeviathan)]
[StepSearcherFlags(StepSearcherFlags.TimeComplexity)]
[StepSearcherRuntimeName("StepSearcherName_ComplexFishStepSearcher")]
public sealed partial class ComplexFishStepSearcher : StepSearcher
{
	/// <summary>
	/// The internal <see cref="StepSearcher"/> instance that is used for pre-checking the possible eliminations of the fishes.
	/// </summary>
	private static readonly PatternOverlayStepSearcher ElimsSearcher = new();


	/// <summary>
	/// Indicates whether the step searcher allows searching for Siamese fishes.
	/// </summary>
	[RuntimeIdentifier(RuntimeIdentifier.AllowSiameseComplexFish)]
	public bool AllowSiamese { get; set; }

	/// <summary>
	/// Indicates the maximum size of the fish the step searcher instance can search for. The maximum possible value of this property is 7.
	/// The default value is 3.
	/// </summary>
	[RuntimeIdentifier(RuntimeIdentifier.MaxSizeOfComplexFish)]
	public int MaxSize { get; set; } = 3;


	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;

		// Gather the POM eliminations to get all possible fish eliminations.
		var pomElims = GetPomEliminationsFirstly(in grid, ref context);
		if (pomElims.All(CommonMethods.False))
		{
			return null;
		}

		// Other variables.
		var tempGrid = grid;

		// Sum up how many digits exist complex fish.
		var tempList = new List<ComplexFishStep>();
		for (var digit = 0; digit < 9; digit++)
		{
			scoped ref readonly var pomElimsOfThisDigit = ref pomElims[digit];

			// Create a background thread to work on searching for fishes of this digit.
			if (pomElimsOfThisDigit)
			{
				Collect(tempList, in tempGrid, ref context, in pomElimsOfThisDigit, digit, context.OnlyFindOne);
			}
		}

		var accumulator = EquatableStep.Distinct(tempList).ToList();
		scoped var siameses = AllowSiamese ? FishModule.GetSiamese(accumulator.ConvertAll(static p => (FishStep)p), in grid) : [];
		if (context.OnlyFindOne)
		{
			return siameses is [var siamese, ..] ? siamese : accumulator.FirstOrDefault() is { } normal ? normal : null;
		}

		if (siameses.Length != 0)
		{
			foreach (var step in siameses)
			{
				context.Accumulator.Add(step);
			}
		}
		if (accumulator.Count != 0)
		{
			context.Accumulator.AddRange(accumulator);
		}

		return null;
	}

	/// <summary>
	/// Get all possible fish steps.
	/// </summary>
	/// <param name="accumulator">The accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="pomElimsOfThisDigit">The possible eliminations to check.</param>
	/// <param name="digit">The current digit used.</param>
	/// <param name="onlyFindOne">Indicates whether the method only find one possible step.</param>
	private void Collect(
		List<ComplexFishStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		scoped ref readonly CellMap pomElimsOfThisDigit,
		Digit digit,
		bool onlyFindOne
	)
	{
		const HouseType bothLines = (HouseType)3;

		// Iterate on each size.
		scoped var currentCoverSets = (stackalloc House[MaxSize]);
		for (var size = 2; size <= MaxSize; size++)
		{
			// Iterate on different cases on whether searcher finds mutant fishes.
			// If false, search for franken fishes.
			foreach (var checkMutant in (false, true))
			{
				// Then iterate on each elimination.
				foreach (var cell in pomElimsOfThisDigit)
				{
					// Try to assume the digit is true in the current cell,
					// and we can get a map of all possible cells that can be filled with the digit.
					var possibleMap = CandidatesMap[digit] - PeersMap[cell] - cell;

					// Get the table of all possible houses that contains that digit.
					scoped var baseTable = possibleMap.Houses.GetAllSets();

					// If the 'table.Length' property is lower than '2 * size',
					// we can't find any possible complex fish now. Just skip it.
					if (baseTable.Length < size << 1)
					{
						continue;
					}

					// Iterate on each base set combinations.
					foreach (var baseSets in baseTable.GetSubsets(size))
					{
						// Get the mask representing the base sets used.
						var baseSetsMask = 0;
						foreach (var baseSet in baseSets)
						{
							baseSetsMask |= 1 << baseSet;
						}

						// Franken fishes doesn't contain both row and column two house types.
						if (!checkMutant && (baseSetsMask & HouseMaskOperations.AllRowsMask) != 0 && (baseSetsMask & HouseMaskOperations.AllColumnsMask) != 0)
						{
							continue;
						}

						// Get the primary map of endo-fins.
						var (tempMap, endofins) = (CellMap.Empty, CellMap.Empty);
						for (var i = 0; i < baseSets.Length; i++)
						{
							var baseSet = baseSets[i];
							if (i != 0)
							{
								endofins |= HousesMap[baseSet] & tempMap;
							}

							tempMap |= HousesMap[baseSet];
						}
						endofins &= possibleMap;

						// We can't hold any endo-fins at present. The algorithm limits
						// the whole technique pattern can't contain endo-fins now.
						// We just assume the possible elimination is true, then the last incomplete
						// pattern can't contain any fins; otherwise, kraken fishes.
						// Do you know why we only check endo-fins instead of checking both exo-fins
						// and endo-fins? Because here the incomplete pattern don't contain
						// any exo-fins at all.
						if (endofins)
						{
							continue;
						}

						// Get the mask for checking for mutant fish.
						Unsafe.SkipInit(out HouseType baseHouseTypes);
						if (checkMutant)
						{
							baseHouseTypes = HouseType.Block;
							if ((baseSetsMask & HouseMaskOperations.AllRowsMask) != 0)
							{
								baseHouseTypes |= HouseType.Row;
							}
							if ((baseSetsMask & HouseMaskOperations.AllColumnsMask) != 0)
							{
								baseHouseTypes |= HouseType.Column;
							}
						}

						// Get all used base set list.
						var (usedInBaseSets, baseMap) = (0, CellMap.Empty);
						foreach (var baseSet in baseSets)
						{
							baseMap |= HousesMap[baseSet];
							usedInBaseSets |= 1 << baseSet;
						}

						// Remove the cells in both peers of 'cell' and the fish body.
						var actualBaseMap = baseMap;
						baseMap &= possibleMap;

						// Now check the possible cover sets to iterate.
						if ((baseMap.Houses & ~usedInBaseSets & HouseMaskOperations.AllHousesMask) is not (var z and not 0))
						{
							continue;
						}

						// If the count is lower than size, we can't find any possible fish.
						var popCount = PopCount((uint)z);
						if (popCount < size)
						{
							continue;
						}

						// Now collect the cover sets into the cover table.
						scoped var coverTable = z.GetAllSets();

						// Iterate on each cover sets combination.
						foreach (var coverSets in coverTable.GetSubsets(size - 1))
						{
							// Now get the cover sets map.
							var coverMap = CellMap.Empty;
							foreach (var coverSet in coverSets)
							{
								coverMap |= HousesMap[coverSet];
							}

							// All cells in base sets should lie in cover sets.
							if (baseMap - coverMap)
							{
								continue;
							}

							// Now check the current cover sets.
							var (usedInCoverSets, i) = (0, 0);
							foreach (var coverSet in coverSets)
							{
								currentCoverSets[i++] = coverSet;
								usedInCoverSets |= 1 << coverSet;
							}
							actualBaseMap &= CandidatesMap[digit];

							// Now iterate on three different house types, to check the final house
							// that is the elimination lies in.
							foreach (var houseType in HouseTypes)
							{
								var houseIndex = cell.ToHouseIndex(houseType);

								// Check whether the house is both used in base sets and cover sets.
								if ((usedInBaseSets >> houseIndex & 1) != 0 || (usedInCoverSets >> houseIndex & 1) != 0)
								{
									continue;
								}

								// Add the house into the cover sets, and check the cover house types.
								usedInCoverSets |= 1 << houseIndex;
								Unsafe.SkipInit(out HouseType coverHouseTypes);
								if (checkMutant)
								{
									coverHouseTypes = HouseType.Block;
									if ((usedInCoverSets & HouseMaskOperations.AllRowsMask) != 0)
									{
										coverHouseTypes |= HouseType.Row;
									}
									if ((usedInCoverSets & HouseMaskOperations.AllColumnsMask) != 0)
									{
										coverHouseTypes |= HouseType.Column;
									}
								}

								// Now check whether the current fish is normal ones,
								// or neither base sets nor cover sets of the mutant fish
								// contain both row and column houses.
								if ((usedInBaseSets & HouseMaskOperations.AllRowsMask) == usedInBaseSets
									&& (usedInCoverSets & HouseMaskOperations.AllColumnsMask) == usedInCoverSets
									|| (usedInBaseSets & HouseMaskOperations.AllColumnsMask) == usedInBaseSets
									&& (usedInCoverSets & HouseMaskOperations.AllRowsMask) == usedInCoverSets)
								{
									// Normal fish.
									goto BacktrackValue;
								}

								if (!checkMutant && (usedInCoverSets & HouseMaskOperations.AllRowsMask) != 0
									&& (usedInCoverSets & HouseMaskOperations.AllColumnsMask) != 0)
								{
									// Mutant fish but checking Franken now.
									goto BacktrackValue;
								}

								if (checkMutant && baseHouseTypes != bothLines && coverHouseTypes != bothLines)
								{
									// Not Mutant fish.
									goto BacktrackValue;
								}

								// Now actual base sets must overlap the current house.
								if (!(actualBaseMap & HousesMap[houseIndex]))
								{
									goto BacktrackValue;
								}

								// Verify passed.
								// Re-initializes endo-fins.
								endofins = CellMap.Empty;

								// Insert into the current cover set list, in order to keep
								// all cover sets are in order (i.e. Sort the cover sets).
								var j = size - 2;
								for (; j >= 0; j--)
								{
									if (currentCoverSets[j] >= houseIndex)
									{
										currentCoverSets[j + 1] = currentCoverSets[j];
									}
									else
									{
										currentCoverSets[j + 1] = houseIndex;
										break;
									}
								}
								if (j < 0)
								{
									currentCoverSets[0] = houseIndex;
								}

								// Collect all endo-fins firstly.
								for (j = 0; j < size; j++)
								{
									if (j > 0)
									{
										endofins |= HousesMap[baseSets[j]] & tempMap;
									}

									if (j == 0)
									{
										tempMap = HousesMap[baseSets[j]];
									}
									else
									{
										tempMap |= HousesMap[baseSets[j]];
									}
								}
								endofins &= CandidatesMap[digit];

								// Add the new house into the cover sets map.
								var nowCoverMap = coverMap | HousesMap[houseIndex];

								// Collect all exo-fins, in order to get all eliminations.
								var exofins = actualBaseMap - nowCoverMap - endofins;
								var elimMap = nowCoverMap - actualBaseMap & CandidatesMap[digit];
								var fins = exofins | endofins;
								if (fins)
								{
									elimMap &= fins.PeerIntersection & CandidatesMap[digit];
								}

								// Cannibalism rule: If a cell is on the intersection of 2 cover sets and a base set, it'll be a cannibalism.
								// A cannibalism will remove the cell that is in a base set.
								// Gather step information.
								// Don't forget the extra cover set. Add it into the list now.
								var actualCoverSets = new House[size];
								for (var p = 0; p < size - 1; p++)
								{
									actualCoverSets[p] = coverSets[p];
								}
								actualCoverSets[^1] = houseIndex;

								var cannibal = false;
								if (!exofins && !endofins)
								{
									foreach (var cannibalism in actualBaseMap)
									{
										var coverSetCounter = 0;
										foreach (var coverSet in actualCoverSets)
										{
											if (HousesMap[coverSet].Contains(cannibalism))
											{
												coverSetCounter++;
											}
										}
										if (coverSetCounter == 2)
										{
											elimMap.Add(cannibalism);
											cannibal = true;
										}
									}
								}

								// Check whether the elimination exists.
								if (!elimMap)
								{
									goto BacktrackValue;
								}

								// Collect highlighting houses.
								var coverSetsMask = 0;
								var houseOffsets = new List<HouseViewNode>();
								foreach (var baseSet in baseSets)
								{
									houseOffsets.Add(new(ColorIdentifier.Normal, baseSet));
								}
								foreach (var coverSet in actualCoverSets)
								{
									houseOffsets.Add(new(ColorIdentifier.Auxiliary2, coverSet));
									coverSetsMask |= 1 << coverSet;
								}

								// Add into the 'accumulator'.
								accumulator.Add(
									new ComplexFishStep(
										[.. from elimCell in elimMap select new Conclusion(Elimination, elimCell, digit)],
										[
											[
												..
												from body in actualBaseMap - exofins - endofins
												select new CandidateViewNode(ColorIdentifier.Normal, body * 9 + digit),
												..
												from exofin in exofins
												select new CandidateViewNode(ColorIdentifier.Exofin, exofin * 9 + digit),
												..
												from endofin in endofins
												select new CandidateViewNode(ColorIdentifier.Endofin, endofin * 9 + digit),
												.. houseOffsets
											]
										],
										context.PredefinedOptions,
										digit,
										baseSetsMask,
										coverSetsMask,
										in exofins,
										in endofins,
										!checkMutant,
										FishModule.IsSashimi(baseSets, in fins, digit),
										cannibal
									)
								);

							BacktrackValue:
								// If failed to be checked, the code will jump to here. We should reset the value in order to backtrack.
								usedInCoverSets &= ~(1 << houseIndex);
							}
						}
					}
				}
			}
		}
	}


	/// <summary>
	/// Get POM technique eliminations at first.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <returns>The dictionary that contains all eliminations grouped by digit used.</returns>
	private static ReadOnlySpan<CellMap> GetPomEliminationsFirstly(scoped ref readonly Grid grid, scoped ref AnalysisContext context)
	{
		var tempList = new List<Step>();
		var playground = grid;
		scoped var context2 = new AnalysisContext(tempList, ref playground, false, context.PredefinedOptions);
		ElimsSearcher.Collect(ref context2);

		var result = new CellMap[9];
		foreach (PatternOverlayStep step in tempList)
		{
			scoped ref var current = ref result[step.Digit];
			current |= from conclusion in step.Conclusions.AsReadOnlySpan() select conclusion.Cell;
		}
		return result;
	}
}
