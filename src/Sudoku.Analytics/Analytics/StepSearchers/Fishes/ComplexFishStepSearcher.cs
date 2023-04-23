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
[StepSearcher, ConditionalCases(ConditionalCase.UnlimitedTimeComplexity)]
public sealed partial class ComplexFishStepSearcher : StepSearcher
{
	/// <summary>
	/// The internal <see cref="StepSearcher"/> instance that is used for pre-checking the possible eliminations of the fishes.
	/// </summary>
	private static readonly PatternOverlayStepSearcher ElimsSearcher = new();


	/// <summary>
	/// Indicates the maximum size of the fish the step searcher instance can search for. The maximum possible value of this property is 7.
	/// The default value is 3.
	/// </summary>
	public int MaxSize { get; set; } = 3;


	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;

		// Gather the POM eliminations to get all possible fish eliminations.
		var pomElims = GetPomEliminationsFirstly(grid);
		if (!Array.Exists(pomElims, static map => map is not []))
		{
			return null;
		}

		// Other variables.
		var tempGrid = grid;

		// Sum up how many digits exist complex fish.
		var count = 0;
		for (var digit = 0; digit < 9; digit++)
		{
			if (pomElims[digit] is not [])
			{
				count++;
			}
		}

		using var cts = new CancellationTokenSource();
		var firstPossibleStep = default(Step?);
		var tempList = new List<ComplexFishStep>();
		var searchingTasks = new Task[count];
		var onlyFindOne = context.OnlyFindOne;

		count = 0;
		for (var digit = 0; digit < 9; digit++)
		{
			if (pomElims[digit] is var pomElimsOfThisDigit and not [])
			{
				// Create a background thread to work on searching for fishes of this digit.
				var digitCopied = digit;
				searchingTasks[count++] = Task.Run(searchingAction, cts.Token);


				void searchingAction()
				{
					if (GetAll(tempList, tempGrid, pomElimsOfThisDigit, digitCopied, onlyFindOne) is { } step)
					{
						firstPossibleStep = step;
						cts.Cancel();
					}
				}
			}
		}

		try
		{
			// Synchronize tasks.
			Task.WaitAll(searchingTasks);
		}
		catch (AggregateException ex) when (ex.InnerException is OperationCanceledException)
		{
			// User has canceled the operation.
			return firstPossibleStep!;
		}

		// Remove duplicate items.
		context.Accumulator!.AddRange(tempList.Distinct());

		return null;
	}

	/// <summary>
	/// Get all possible fish steps.
	/// </summary>
	/// <param name="accumulator">The accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="pomElimsOfThisDigit">The possible eliminations to check.</param>
	/// <param name="digit">The current digit used.</param>
	/// <param name="onlyFindOne">Indicates whether the method only find one possible step.</param>
	private unsafe Step? GetAll(
		ICollection<ComplexFishStep> accumulator,
		scoped in Grid grid,
		scoped in CellMap pomElimsOfThisDigit,
		Digit digit,
		bool onlyFindOne
	)
	{
		const HouseType bothLines = (HouseType)3;

		var currentCoverSets = stackalloc House[MaxSize];
		var searchForMutantCases = stackalloc[] { false, true };

		// Iterate on each size.
		for (var size = 2; size <= MaxSize; size++)
		{
			// Iterate on different cases on whether searcher finds mutant fishes.
			// If false, search for franken fishes.
			for (var caseIndex = 0; caseIndex < 2; caseIndex++)
			{
				// Then iterate on each elimination.
				var searchForMutant = searchForMutantCases[caseIndex];
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
						if (!searchForMutant && (baseSetsMask & AllRowsMask) != 0 && (baseSetsMask & AllColumnsMask) != 0)
						{
							continue;
						}

						// Get the primary map of endo-fins.
						var tempMap = CellMap.Empty;
						var endofins = CellMap.Empty;
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
						// the whole technique structure can't contain endo-fins now.
						// We just assume the possible elimination is true, then the last incomplete
						// structure can't contain any fins; otherwise, kraken fishes.
						// Do you know why we only check endo-fins instead of checking both exo-fins
						// and endo-fins? Because here the incomplete structure don't contain
						// any exo-fins at all.
						if (endofins)
						{
							continue;
						}

						// Get the mask for checking for mutant fish.
						SkipInit(out HouseType baseHouseTypes);
						if (searchForMutant)
						{
							baseHouseTypes = HouseType.Block;
							if ((baseSetsMask & AllRowsMask) != 0)
							{
								baseHouseTypes |= HouseType.Row;
							}
							if ((baseSetsMask & AllColumnsMask) != 0)
							{
								baseHouseTypes |= HouseType.Column;
							}
						}

						// Get all used base set list.
						var usedInBaseSets = 0;
						var baseMap = CellMap.Empty;
						foreach (var baseSet in baseSets)
						{
							baseMap |= HousesMap[baseSet];
							usedInBaseSets |= 1 << baseSet;
						}

						// Remove the cells in both peers of 'cell' and the fish body.
						var actualBaseMap = baseMap;
						baseMap &= possibleMap;

						// Now check the possible cover sets to iterate.
						var z = baseMap.Houses & ~usedInBaseSets & AllHousesMask;
						if (z == 0)
						{
							continue;
						}

						// If the count is lower than size, we can't find any possible fish.
						var popCount = PopCount((uint)z);
						if (popCount < size)
						{
							continue;
						}

						// Now record the cover sets into the cover table.
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
							var usedInCoverSets = 0;
							var i = 0;
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
								SkipInit(out HouseType coverHouseTypes);
								if (searchForMutant)
								{
									coverHouseTypes = HouseType.Block;
									if ((usedInCoverSets & AllRowsMask) != 0)
									{
										coverHouseTypes |= HouseType.Row;
									}
									if ((usedInCoverSets & AllColumnsMask) != 0)
									{
										coverHouseTypes |= HouseType.Column;
									}
								}

								// Now check whether the current fish is normal ones,
								// or neither base sets nor cover sets of the mutant fish
								// contain both row and column houses.
								if ((usedInBaseSets & AllRowsMask) == usedInBaseSets && (usedInCoverSets & AllColumnsMask) == usedInCoverSets
									|| (usedInBaseSets & AllColumnsMask) == usedInBaseSets && (usedInCoverSets & AllRowsMask) == usedInCoverSets)
								{
									// Normal fish.
									goto BacktrackValue;
								}

								if (!searchForMutant && (usedInCoverSets & AllRowsMask) != 0 && (usedInCoverSets & AllColumnsMask) != 0)
								{
									// Mutant fish but checking Franken now.
									goto BacktrackValue;
								}

								if (searchForMutant && baseHouseTypes != bothLines && coverHouseTypes != bothLines)
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

								// Check whether the elimination exists.
								if (!elimMap)
								{
									goto BacktrackValue;
								}

								// Gather eliminations, views and step information.
								var conclusions = new List<Conclusion>();
								foreach (var elimCell in elimMap)
								{
									conclusions.Add(new(Elimination, elimCell, digit));
								}

								// Collect highlighting candidates.
								var candidateOffsets = new List<CandidateViewNode>();
								var houseOffsets = new List<HouseViewNode>();
								foreach (var body in actualBaseMap)
								{
									candidateOffsets.Add(new(WellKnownColorIdentifierKind.Normal, body * 9 + digit));
								}
								foreach (var exofin in exofins)
								{
									candidateOffsets.Add(new(WellKnownColorIdentifierKind.Exofin, exofin * 9 + digit));
								}
								foreach (var endofin in endofins)
								{
									candidateOffsets.Add(new(WellKnownColorIdentifierKind.Endofin, endofin * 9 + digit));
								}

								// Don't forget the extra cover set.
								// Add it into the list now.
								var actualCoverSets = new House[size];
								for (var p = 0; p < size - 1; p++)
								{
									actualCoverSets[p] = coverSets[p];
								}
								actualCoverSets[^1] = houseIndex;

								// Collect highlighting houses.
								var coverSetsMask = 0;
								foreach (var baseSet in baseSets)
								{
									houseOffsets.Add(new(WellKnownColorIdentifierKind.Normal, baseSet));
								}
								foreach (var coverSet in actualCoverSets)
								{
									houseOffsets.Add(new(WellKnownColorIdentifierKind.Auxiliary2, coverSet));
									coverSetsMask |= 1 << coverSet;
								}

								// Add into the 'accumulator'.
								var step = new ComplexFishStep(
									conclusions.ToArray(),
									new[] { View.Empty | candidateOffsets | houseOffsets },
									digit,
									baseSetsMask,
									coverSetsMask,
									exofins,
									endofins,
									!searchForMutant,
									FishStepSearcherHelper.IsSashimi(baseSets, fins, digit)
								);
								if (onlyFindOne)
								{
									return step;
								}

								accumulator.Add(step);

							// Backtracking.
							BacktrackValue:
								usedInCoverSets &= ~(1 << houseIndex);
							}
						}
					}
				}
			}
		}

		return null;
	}


	/// <summary>
	/// Get POM technique eliminations at first.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The dictionary that contains all eliminations grouped by digit used.</returns>
	private static CellMap[] GetPomEliminationsFirstly(scoped in Grid grid)
	{
		var tempList = new List<Step>();
		scoped var context = new AnalysisContext(tempList, grid, false);
		ElimsSearcher.Collect(ref context);

		var result = new CellMap[9];
		foreach (PatternOverlayStep step in tempList)
		{
			result[step.Digit].AddRange(from conclusion in step.Conclusions select conclusion.Cell);
		}

		return result;
	}
}
