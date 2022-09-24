namespace Sudoku.Solving.Logics.Implementations.Searchers;

[StepSearcher]
internal sealed unsafe partial class ComplexFishStepSearcher : IComplexFishStepSearcher
{
	private static readonly PatternOverlayStepSearcher ElimsSearcher = new();


	/// <inheritdoc/>
	[StepSearcherProperty]
	public int MaxSize { get; set; }


	/// <inheritdoc/>
	public IStep? GetAll(scoped in LogicalAnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;

		// Gather the POM eliminations to get all possible fish eliminations.
		var pomElims = GetPomEliminationsFirstly(grid);

		// Other variables.
		var tempGrid = grid;

		// Sum up how many digits exist complex fish.
		var count = 0;
		for (var digit = 0; digit < 9; digit++)
		{
			if (pomElims[digit] is not null)
			{
				count++;
			}
		}

		using var cts = new CancellationTokenSource();
		var firstPossibleStep = (IStep?)null;
		var tempList = new List<ComplexFishStep>();
		var searchingTasks = new Task[count];
		var onlyFindOne = context.OnlyFindOne;
		count = 0;
		for (var digit = 0; digit < 9; digit++)
		{
			if (pomElims[digit] is null)
			{
				continue;
			}

			// Note the iteration variable can't be used directly.
			// We should add a copy in order to use it.
			var currentDigit = digit;

			// Create a background thread to work on searching for fishes of this digit.
			searchingTasks[count++] = Task.Run(
				() =>
				{
					if (GetAll(tempList, tempGrid, pomElims, currentDigit, onlyFindOne) is { } step)
					{
						firstPossibleStep = step;
						cts.Cancel();
					}
				},
				cts.Token
			);
		}

		try
		{
			// Synchronize the tasks.
			Task.WaitAll(searchingTasks);
		}
		catch (AggregateException ex) when (ex.InnerException is OperationCanceledException)
		{
			// User has canceled the operation.
			return firstPossibleStep!;
		}

		// Remove duplicate items.
		context.Accumulator!.AddRange(IDistinctableStep<ComplexFishStep>.Distinct(tempList));

		return null;
	}


	/// <summary>
	/// Get all possible fish steps.
	/// </summary>
	/// <param name="accumulator">The accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="pomElims">The possible eliminations to check, specified as a dictionary.</param>
	/// <param name="digit">The current digit used.</param>
	/// <param name="onlyFindOne">Indicates whether the method only find one possible step.</param>
	private IStep? GetAll(
		ICollection<ComplexFishStep> accumulator, scoped in Grid grid,
		IList<Conclusion>?[] pomElims, int digit, bool onlyFindOne)
	{
		const HouseType bothLines = (HouseType)3;

		var currentCoverSets = stackalloc int[MaxSize];
		var searchForMutantCases = stackalloc[] { false, true };

		// Iterate on each size.
		for (var size = 2; size <= MaxSize; size++)
		{
			// Iterate on different cases on whether searcher finds mutant fishes.
			// If false, search for franken fishes.
			for (var caseIndex = 0; caseIndex < 2; caseIndex++)
			{
				var searchForMutant = searchForMutantCases[caseIndex];

				// Try to check the POM eliminations.
				// If the digit as a key doesn't contain any list in that dictionary,
				// just skip this loop.
				var pomElimsOfThisDigit = pomElims[digit];
				if (pomElimsOfThisDigit is null)
				{
					continue;
				}

				// Get the eliminations of this digit.
				var elims = new int[pomElimsOfThisDigit.Count];
				var index = 0;
				foreach (var conclusion in pomElimsOfThisDigit)
				{
					elims[index++] = conclusion.Cell;
				}

				// Then iterate on each elimination.
				foreach (var cell in elims)
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
						if (!searchForMutant
							&& (baseSetsMask & AllRowsMask) != 0 && (baseSetsMask & AllColumnsMask) != 0)
						{
							continue;
						}

						// Get the primary map of endo-fins.
						CellMap tempMap = CellMap.Empty, endofins = CellMap.Empty;
						for (int i = 0, length = baseSets.Length; i < length; i++)
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
						Unsafe.SkipInit(out HouseType baseHouseTypes);
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
							int usedInCoverSets = 0, i = 0;
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
								if ((usedInBaseSets >> houseIndex & 1) != 0
									|| (usedInCoverSets >> houseIndex & 1) != 0)
								{
									continue;
								}

								// Add the house into the cover sets, and check the cover house types.
								usedInCoverSets |= 1 << houseIndex;
								Unsafe.SkipInit(out HouseType coverHouseTypes);
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
								if ((usedInBaseSets & AllRowsMask) == usedInBaseSets
									&& (usedInCoverSets & AllColumnsMask) == usedInCoverSets
									|| (usedInBaseSets & AllColumnsMask) == usedInBaseSets
									&& (usedInCoverSets & AllRowsMask) == usedInCoverSets)
								{
									// Normal fish.
									goto BacktrackValue;
								}

								if (!searchForMutant
									&& (usedInCoverSets & AllRowsMask) != 0
									&& (usedInCoverSets & AllColumnsMask) != 0)
								{
									// Mutant fish but checking Franken now.
									goto BacktrackValue;
								}

								if (searchForMutant
									&& baseHouseTypes != bothLines && coverHouseTypes != bothLines)
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
									candidateOffsets.Add(new(DisplayColorKind.Normal, body * 9 + digit));
								}
								foreach (var exofin in exofins)
								{
									candidateOffsets.Add(new(DisplayColorKind.Exofin, exofin * 9 + digit));
								}
								foreach (var endofin in endofins)
								{
									candidateOffsets.Add(new(DisplayColorKind.Endofin, endofin * 9 + digit));
								}

								// Don't forget the extra cover set.
								// Add it into the list now.
								var actualCoverSets = new int[size];
								for (int p = 0, iterationSize = size - 1; p < iterationSize; p++)
								{
									actualCoverSets[p] = coverSets[p];
								}
								actualCoverSets[^1] = houseIndex;

								// Collect highlighting houses.
								var coverSetsMask = 0;
								foreach (var baseSet in baseSets)
								{
									houseOffsets.Add(new(DisplayColorKind.Normal, baseSet));
								}
								foreach (var coverSet in actualCoverSets)
								{
									houseOffsets.Add(new(DisplayColorKind.Auxiliary2, coverSet));
									coverSetsMask |= 1 << coverSet;
								}

								// Add into the 'accumulator'.
								var step = new ComplexFishStep(
									ImmutableArray.CreateRange(conclusions),
									ImmutableArray.Create(View.Empty | candidateOffsets | houseOffsets),
									digit,
									baseSetsMask,
									coverSetsMask,
									exofins,
									endofins,
									!searchForMutant,
									IFishStepSearcher.IsSashimi(baseSets, fins, digit)
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
	private static IList<Conclusion>?[] GetPomEliminationsFirstly(scoped in Grid grid)
	{
		var tempList = new List<IStep>();
		ElimsSearcher.GetAll(new(tempList, grid, false));

		var result = new IList<Conclusion>?[9];
		foreach (PatternOverlayStep step in tempList)
		{
			var digit = step.Digit;
			scoped ref var currentList = ref result[digit];
			if (currentList is null)
			{
				currentList = new List<Conclusion>(step.Conclusions);
			}
			else
			{
				currentList.AddRange(step.Conclusions);
			}
		}

		return result;
	}
}
