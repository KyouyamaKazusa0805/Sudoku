namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Direct Subset</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Direct Locked Subset</item>
/// <item>Direct Hidden Locked Subset</item>
/// <item>Direct Naked Subset</item>
/// <item>Direct Hidden Subset</item>
/// </list>
/// </summary>
[StepSearcher(
	Technique.NakedPairFullHouse, Technique.NakedPairPlusFullHouse, Technique.HiddenPairFullHouse,
	Technique.LockedPairFullHouse, Technique.LockedHiddenPairFullHouse,
	Technique.NakedTripleFullHouse, Technique.NakedTriplePlusFullHouse, Technique.HiddenTripleFullHouse,
	Technique.LockedTripleFullHouse, Technique.LockedHiddenTripleFullHouse,
	Technique.NakedQuadrupleFullHouse, Technique.NakedQuadruplePlusFullHouse, Technique.HiddenQuadrupleFullHouse,
	Technique.NakedPairCrosshatchingBlock, Technique.NakedPairPlusCrosshatchingBlock, Technique.HiddenPairCrosshatchingBlock,
	Technique.LockedPairCrosshatchingBlock, Technique.LockedHiddenPairCrosshatchingBlock,
	Technique.NakedTripleCrosshatchingBlock, Technique.NakedTriplePlusCrosshatchingBlock, Technique.HiddenTripleCrosshatchingBlock,
	Technique.LockedTripleCrosshatchingBlock, Technique.LockedHiddenTripleCrosshatchingBlock,
	Technique.NakedQuadrupleCrosshatchingBlock, Technique.NakedQuadruplePlusCrosshatchingBlock, Technique.HiddenQuadrupleCrosshatchingBlock,
	Technique.NakedPairCrosshatchingRow, Technique.NakedPairPlusCrosshatchingRow, Technique.HiddenPairCrosshatchingRow,
	Technique.LockedPairCrosshatchingRow, Technique.LockedHiddenPairCrosshatchingRow,
	Technique.NakedTripleCrosshatchingRow, Technique.NakedTriplePlusCrosshatchingRow, Technique.HiddenTripleCrosshatchingRow,
	Technique.LockedTripleCrosshatchingRow, Technique.LockedHiddenTripleCrosshatchingRow,
	Technique.NakedQuadrupleCrosshatchingRow, Technique.NakedQuadruplePlusCrosshatchingRow, Technique.HiddenQuadrupleCrosshatchingRow,
	Technique.NakedPairCrosshatchingColumn, Technique.NakedPairPlusCrosshatchingColumn, Technique.HiddenPairCrosshatchingColumn,
	Technique.LockedPairCrosshatchingColumn, Technique.LockedHiddenPairCrosshatchingColumn,
	Technique.NakedTripleCrosshatchingColumn, Technique.NakedTriplePlusCrosshatchingColumn, Technique.HiddenTripleCrosshatchingColumn,
	Technique.LockedTripleCrosshatchingColumn, Technique.LockedHiddenTripleCrosshatchingColumn,
	Technique.NakedQuadrupleCrosshatchingColumn, Technique.NakedQuadruplePlusCrosshatchingColumn, Technique.HiddenQuadrupleCrosshatchingColumn,
	Technique.NakedPairNakedSingle, Technique.NakedPairPlusNakedSingle, Technique.HiddenPairNakedSingle,
	Technique.LockedPairNakedSingle, Technique.LockedHiddenPairNakedSingle,
	Technique.NakedTripleNakedSingle, Technique.NakedTriplePlusNakedSingle, Technique.HiddenTripleNakedSingle,
	Technique.LockedTripleNakedSingle, Technique.LockedHiddenTripleNakedSingle,
	Technique.NakedQuadrupleNakedSingle, Technique.NakedQuadruplePlusNakedSingle, Technique.HiddenQuadrupleNakedSingle,
	IsFixed = true, IsReadOnly = true)]
[StepSearcherFlags(StepSearcherFlags.DirectTechniquesOnly)]
[StepSearcherRuntimeName("StepSearcherName_DirectSubsetStepSearcher")]
public sealed partial class DirectSubsetStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override unsafe Step? Collect(scoped ref AnalysisContext context)
	{
		var p = stackalloc SubsetModuleSearcherFunc[] { &HiddenSubset, &NakedSubset };
		var q = stackalloc SubsetModuleSearcherFunc[] { &NakedSubset, &HiddenSubset };
		var searchers = context.PredefinedOptions is { DistinctDirectMode: true, IsDirectMode: true } ? p : q;
		scoped ref readonly var grid = ref context.Grid;
		foreach (var searchingForLocked in (true, false))
		{
			for (var size = 2; size <= (searchingForLocked ? 3 : 4); size++)
			{
				for (var i = 0; i < 2; i++)
				{
					if (searchers[i](ref context, in grid, size, searchingForLocked) is { } step)
					{
						return step;
					}
				}
			}
		}

		return null;
	}


	/// <summary>
	/// Gets the <see cref="Technique"/> field describing the subset usage on hidden subsets.
	/// </summary>
	/// <param name="subsetCells">The subset cells used.</param>
	/// <returns>The final result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Technique GetSubsetTechnique_Hidden(scoped ref readonly CellMap subsetCells)
		=> (subsetCells.IsInIntersection, subsetCells.Count) switch
		{
			(true, 2) => Technique.LockedHiddenPair,
			(_, 2) => Technique.HiddenPair,
			(true, 3) => Technique.LockedHiddenTriple,
			(_, 3) => Technique.HiddenTriple,
			(_, 4) => Technique.HiddenQuadruple
		};

	/// <summary>
	/// Gets the <see cref="Technique"/> field describing the subset usage on naked subsets.
	/// </summary>
	/// <param name="subsetCells">The subset cells used.</param>
	/// <param name="isLocked">Indicates whether the subset is locked in logic.</param>
	/// <returns>The final result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Technique GetSubsetTechnique_Naked(scoped ref readonly CellMap subsetCells, bool? isLocked)
		=> (isLocked, subsetCells.Count) switch
		{
			(true, 2) => Technique.LockedPair,
			(false, 2) => Technique.NakedPairPlus,
			(_, 2) => Technique.NakedPair,
			(true, 3) => Technique.LockedTriple,
			(false, 3) => Technique.NakedTriplePlus,
			(_, 3) => Technique.NakedTriple,
			(false, 4) => Technique.NakedQuadruplePlus,
			_ => Technique.NakedQuadruple
		};

	/// <summary>
	/// Search for hidden subsets.
	/// </summary>
	private static DirectSubsetStep? HiddenSubset(
		scoped ref AnalysisContext context,
		scoped ref readonly Grid grid,
		int size,
		bool searchingForLocked
	)
	{
		for (var house = 0; house < 27; house++)
		{
			scoped ref readonly var currentHouseCells = ref HousesMap[house];
			var traversingMap = currentHouseCells & EmptyCells;
			var mask = grid[in traversingMap];
			foreach (var digits in mask.GetAllSets().GetSubsets(size))
			{
				var (tempMask, digitsMask, cells) = (mask, (Mask)0, CellMap.Empty);
				foreach (var digit in digits)
				{
					tempMask &= (Mask)~(1 << digit);
					digitsMask |= (Mask)(1 << digit);
					cells |= currentHouseCells & CandidatesMap[digit];
				}
				if (cells.Count != size)
				{
					continue;
				}

				// Gather eliminations.
				var conclusions = CandidateMap.Empty;
				foreach (var digit in tempMask)
				{
					foreach (var cell in cells & CandidatesMap[digit])
					{
						conclusions.Add(cell * 9 + digit);
					}
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				// Gather highlight candidates.
				var (cellOffsets, candidateOffsets) = (new List<CellViewNode>(), new List<CandidateViewNode>());
				foreach (var digit in digits)
				{
					foreach (var cell in cells & CandidatesMap[digit])
					{
						candidateOffsets.Add(new(ColorIdentifier.Normal, cell * 9 + digit));
					}

					cellOffsets.AddRange(SubsetModule.GetCrosshatchBaseCells(in grid, digit, house, in cells));
				}

				var isLocked = cells.IsInIntersection;
				if (!searchingForLocked || isLocked && searchingForLocked)
				{
					var containsExtraEliminations = false;
					if (isLocked)
					{
						// A potential locked hidden subset found. Extra eliminations should be checked.
						// Please note that here a hidden subset may not be a locked one because eliminations aren't validated.
						var eliminatingHouse = TrailingZeroCount(cells.CoveredHouses & ~(1 << house));
						foreach (var cell in (HousesMap[eliminatingHouse] & EmptyCells) - cells)
						{
							foreach (var digit in digitsMask)
							{
								if ((grid.GetCandidates(cell) >> digit & 1) != 0)
								{
									conclusions.Add(cell * 9 + digit);
									containsExtraEliminations = true;
								}
							}
						}
					}

					if (searchingForLocked && isLocked && !containsExtraEliminations
						|| !searchingForLocked && isLocked && containsExtraEliminations)
					{
						// This is a locked hidden subset. We cannot handle this as a normal hidden subset.
						continue;
					}

					// Check whether such conclusions will raise a single.
					if (CheckHiddenSubsetFullHouse(
						ref context, in grid, in conclusions, in cells, digitsMask, house,
						searchingForLocked, containsExtraEliminations, cellOffsets, candidateOffsets) is { } fullHouse)
					{
						return fullHouse;
					}
					if (CheckHiddenSubsetHiddenSingle(
						ref context, in grid, in conclusions, in cells, digitsMask, house,
						searchingForLocked, containsExtraEliminations, cellOffsets, candidateOffsets) is { } hiddenSingle)
					{
						return hiddenSingle;
					}
					if (CheckHiddenSubsetNakedSingle(
						ref context, in grid, in conclusions, in cells, digitsMask, house,
						searchingForLocked, containsExtraEliminations, cellOffsets, candidateOffsets) is { } nakedSingle)
					{
						return nakedSingle;
					}
				}
			}
		}

		return null;
	}

	/// <summary>
	/// Search for naked subsets.
	/// </summary>
	private static DirectSubsetStep? NakedSubset(
		scoped ref AnalysisContext context,
		scoped ref readonly Grid grid,
		int size,
		bool searchingForLocked
	)
	{
		for (var house = 0; house < 27; house++)
		{
			if ((HousesMap[house] & EmptyCells) is not { Count: >= 2 } currentEmptyMap)
			{
				continue;
			}

			// Remove cells that only contain 1 candidate (Naked Singles).
			foreach (var cell in HousesMap[house] & EmptyCells)
			{
				if (IsPow2(grid.GetCandidates(cell)))
				{
					currentEmptyMap.Remove(cell);
				}
			}

			// Iterate on each combination.
			foreach (ref readonly var cells in currentEmptyMap.GetSubsets(size))
			{
				var digitsMask = grid[in cells];
				if (PopCount((uint)digitsMask) != size)
				{
					continue;
				}

				// Naked subset found. Now check eliminations.
				var (lockedDigitsMask, conclusions) = ((Mask)0, CandidateMap.Empty);
				foreach (var digit in digitsMask)
				{
					var map = cells % CandidatesMap[digit];
					lockedDigitsMask |= (Mask)(map.InOneHouse(out _) ? 0 : 1 << digit);
					conclusions |= map * digit;
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>(16);
				foreach (var cell in cells)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(ColorIdentifier.Normal, cell * 9 + digit));
					}
				}

				var isLocked = cells.IsInIntersection
					? true
					: lockedDigitsMask == digitsMask && size != 4
						? true
						: lockedDigitsMask != 0 ? false : default(bool?);
				if ((isLocked, searchingForLocked) is not ((true, true) or (not true, false)))
				{
					continue;
				}

				// Check whether such conclusions will raise a single.
				if (CheckNakedSubsetFullHouse(
					ref context, in grid, in conclusions, in cells, digitsMask, house,
					searchingForLocked, isLocked, candidateOffsets) is { } fullHouse)
				{
					return fullHouse;
				}
				if (CheckNakedSubsetHiddenSingle(
					ref context, in grid, in conclusions, in cells, digitsMask, house,
					searchingForLocked, isLocked, candidateOffsets) is { } hiddenSingle)
				{
					return hiddenSingle;
				}
				if (CheckNakedSubsetNakedSingle(
					ref context, in grid, in conclusions, in cells, digitsMask, house,
					searchingForLocked, isLocked, candidateOffsets) is { } nakedSingle)
				{
					return nakedSingle;
				}
			}
		}

		return null;
	}

	/// <summary>
	/// Check for full houses produced on hidden subsets.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="conclusions">The conclusions produced by hidden subsets.</param>
	/// <param name="subsetCells">The subset cells used.</param>
	/// <param name="subsetDigitsMask">The digits that the subset pattern used.</param>
	/// <param name="subsetHouse">The house producing the subset.</param>
	/// <param name="searchingForLocked">Indicates whether the current mode is for searching locked hidden subsets.</param>
	/// <param name="containsExtraEliminations">Indicates whether the extra eliminations are inferred.</param>
	/// <param name="cellOffsets">Indicates the cell nodes.</param>
	/// <param name="candidateOffsets">Indicates the candidate offsets.</param>
	/// <returns>The found step.</returns>
	private static DirectSubsetStep? CheckHiddenSubsetFullHouse(
		scoped ref AnalysisContext context,
		scoped ref readonly Grid grid,
		scoped ref readonly CandidateMap conclusions,
		scoped ref readonly CellMap subsetCells,
		Mask subsetDigitsMask,
		House subsetHouse,
		bool searchingForLocked,
		bool containsExtraEliminations,
		List<CellViewNode> cellOffsets,
		List<CandidateViewNode> candidateOffsets
	)
	{
		foreach (var candidate in conclusions)
		{
			var cell = candidate / 9;
			var digit = candidate % 9;
			foreach (var houseType in HouseTypes)
			{
				var house = cell.ToHouseIndex(houseType);
				var emptyCells = HousesMap[house] & EmptyCells;
				if (emptyCells.Count <= 1)
				{
					continue;
				}

				// Check for candidates for the cell.
				var eliminatedDigitsMask = MaskOperations.Create(from c in conclusions where c / 9 == cell select c % 9);
				var valueDigitsMask = (Mask)(Grid.MaxCandidatesMask & (Mask)~grid[HousesMap[house] - emptyCells, true]);
				var lastDigitsMask = (Mask)(valueDigitsMask & (Mask)~eliminatedDigitsMask);
				if (!IsPow2(lastDigitsMask))
				{
					continue;
				}

				var lastDigit = Log2((uint)lastDigitsMask);
				if ((grid.GetCandidates(cell) >> lastDigit & 1) == 0)
				{
					// This cell doesn't contain such digit.
					continue;
				}

				var step = new DirectSubsetStep(
					[new(Assignment, cell, lastDigit)],
					[
						[
							.. candidateOffsets,
							.. cellOffsets,
							new CellViewNode(ColorIdentifier.Auxiliary3, cell),
							new HouseViewNode(ColorIdentifier.Normal, subsetHouse),
							new HouseViewNode(ColorIdentifier.Auxiliary3, house)
						]
					],
					context.PredefinedOptions,
					cell,
					lastDigit,
					in subsetCells,
					subsetDigitsMask,
					house,
					[cell],
					eliminatedDigitsMask,
					houseType switch
					{
						HouseType.Block => SingleSubtype.FullHouseBlock,
						HouseType.Row => SingleSubtype.FullHouseRow,
						_ => SingleSubtype.FullHouseColumn
					},
					Technique.FullHouse,
					GetSubsetTechnique_Hidden(in subsetCells)
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
	/// Check for hidden single produced on hidden subsets.
	/// </summary>
	/// <inheritdoc cref="CheckHiddenSubsetFullHouse(ref AnalysisContext, ref readonly Grid, ref readonly CandidateMap, ref readonly CellMap, Mask, House, bool, bool, List{CellViewNode}, List{CandidateViewNode})"/>
	private static DirectSubsetStep? CheckHiddenSubsetHiddenSingle(
		scoped ref AnalysisContext context,
		scoped ref readonly Grid grid,
		scoped ref readonly CandidateMap conclusions,
		scoped ref readonly CellMap subsetCells,
		Mask subsetDigitsMask,
		House subsetHouse,
		bool searchingForLocked,
		bool containsExtraEliminations,
		List<CellViewNode> cellOffsets,
		List<CandidateViewNode> candidateOffsets
	)
	{
		foreach (var candidate in conclusions)
		{
			var cell = candidate / 9;
			var digit = candidate % 9;
			foreach (var houseType in HouseTypes)
			{
				var house = cell.ToHouseIndex(houseType);
				var eliminatedCells = (CellMap)from c in conclusions where c % 9 == digit select c / 9;
				var availableCells = (HousesMap[house] & CandidatesMap[digit]) - eliminatedCells;
				if (availableCells is not [var lastCell])
				{
					continue;
				}

				var step = new DirectSubsetStep(
					[new(Assignment, lastCell, digit)],
					[
						[
							.. candidateOffsets,
							.. cellOffsets,
							.. SingleModule.GetHiddenSingleExcluders(in grid, digit, house, lastCell, out var chosenCells),
							..
							from c in HousesMap[house] & eliminatedCells
							select new CandidateViewNode(ColorIdentifier.Elimination, c * 9 + digit),
							new CellViewNode(ColorIdentifier.Auxiliary3, lastCell) { RenderingMode = DirectModeOnly },
							new CandidateViewNode(ColorIdentifier.Elimination, lastCell * 9 + digit),
							new HouseViewNode(ColorIdentifier.Normal, subsetHouse),
							new HouseViewNode(ColorIdentifier.Auxiliary3, house)
						]
					],
					context.PredefinedOptions,
					lastCell,
					digit,
					in subsetCells,
					subsetDigitsMask,
					house,
					in eliminatedCells,
					(Mask)(1 << digit),
					SingleModule.GetHiddenSingleSubtype(in grid, lastCell, house, in chosenCells),
					houseType switch
					{
						HouseType.Block => Technique.CrosshatchingBlock,
						HouseType.Row => Technique.CrosshatchingRow,
						_ => Technique.CrosshatchingColumn
					},
					GetSubsetTechnique_Hidden(in subsetCells)
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
	/// Check for naked single produced on hidden subsets.
	/// </summary>
	/// <inheritdoc cref="CheckHiddenSubsetFullHouse(ref AnalysisContext, ref readonly Grid, ref readonly CandidateMap, ref readonly CellMap, Mask, House, bool, bool, List{CellViewNode}, List{CandidateViewNode})"/>
	private static DirectSubsetStep? CheckHiddenSubsetNakedSingle(
		scoped ref AnalysisContext context,
		scoped ref readonly Grid grid,
		scoped ref readonly CandidateMap conclusions,
		scoped ref readonly CellMap subsetCells,
		Mask subsetDigitsMask,
		House subsetHouse,
		bool searchingForLocked,
		bool containsExtraEliminations,
		List<CellViewNode> cellOffsets,
		List<CandidateViewNode> candidateOffsets
	)
	{
		foreach (var candidate in conclusions)
		{
			var cell = candidate / 9;
			var digit = candidate % 9;
			var eliminatedDigitsMask = MaskOperations.Create(from c in conclusions where c / 9 == cell select c % 9);
			var availableDigitsMask = (Mask)(grid.GetCandidates(cell) & (Mask)~eliminatedDigitsMask);
			if (!IsPow2(availableDigitsMask))
			{
				continue;
			}

			var lastDigit = Log2((uint)availableDigitsMask);
			var step = new DirectSubsetStep(
				[new(Assignment, cell, lastDigit)],
				[
					[
						.. candidateOffsets,
						.. cellOffsets,
						.. SingleModule.GetNakedSingleExcluders(in grid, cell, lastDigit, out _),
						new HouseViewNode(ColorIdentifier.Normal, subsetHouse),
						new CellViewNode(ColorIdentifier.Auxiliary3, cell)
					]
				],
				context.PredefinedOptions,
				cell,
				lastDigit,
				in subsetCells,
				subsetDigitsMask,
				subsetHouse,
				[cell],
				eliminatedDigitsMask,
				SingleSubtype.NakedSingle0 + (HousesMap[cell.ToHouseIndex(HouseType.Block)] - EmptyCells).Count,
				Technique.NakedSingle,
				GetSubsetTechnique_Hidden(in subsetCells)
			);
			if (context.OnlyFindOne)
			{
				return step;
			}

			context.Accumulator.Add(step);
		}

		return null;
	}

	/// <summary>
	/// Check for full house produced on naked subsets.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="conclusions">The conclusions produced by hidden subsets.</param>
	/// <param name="subsetCells">The subset cells used.</param>
	/// <param name="subsetDigitsMask">The digits that the subset pattern used.</param>
	/// <param name="subsetHouse">The house producing the subset.</param>
	/// <param name="searchingForLocked">Indicates whether the current mode is for searching locked hidden subsets.</param>
	/// <param name="isLocked">Indicates whether the subset is locked.</param>
	/// <param name="candidateOffsets">Indicates the candidate offsets.</param>
	/// <returns>The found step.</returns>
	private static DirectSubsetStep? CheckNakedSubsetFullHouse(
		scoped ref AnalysisContext context,
		scoped ref readonly Grid grid,
		scoped ref readonly CandidateMap conclusions,
		scoped ref readonly CellMap subsetCells,
		Mask subsetDigitsMask,
		House subsetHouse,
		bool searchingForLocked,
		bool? isLocked,
		List<CandidateViewNode> candidateOffsets
	)
	{
		foreach (var candidate in conclusions)
		{
			var cell = candidate / 9;
			var digit = candidate % 9;
			foreach (var houseType in HouseTypes)
			{
				var house = cell.ToHouseIndex(houseType);
				var emptyCells = HousesMap[house] & EmptyCells;
				if (emptyCells.Count <= 1)
				{
					continue;
				}

				// Check for candidates for the cell.
				var eliminatedDigitsMask = MaskOperations.Create(from c in conclusions where c / 9 == cell select c % 9);
				var valueDigitsMask = (Mask)(Grid.MaxCandidatesMask & (Mask)~grid[HousesMap[house] - emptyCells, true]);
				var lastDigitsMask = (Mask)(valueDigitsMask & (Mask)~eliminatedDigitsMask);
				if (!IsPow2(lastDigitsMask))
				{
					continue;
				}

				var lastDigit = Log2((uint)lastDigitsMask);
				if ((grid.GetCandidates(cell) >> lastDigit & 1) == 0)
				{
					// This cell doesn't contain such digit.
					continue;
				}

				var step = new DirectSubsetStep(
					[new(Assignment, cell, lastDigit)],
					[
						[
							.. candidateOffsets,
							new CellViewNode(ColorIdentifier.Auxiliary3, cell),
							new HouseViewNode(ColorIdentifier.Normal, subsetHouse),
							new HouseViewNode(ColorIdentifier.Auxiliary3, house)
						]
					],
					context.PredefinedOptions,
					cell,
					lastDigit,
					in subsetCells,
					subsetDigitsMask,
					house,
					[cell],
					eliminatedDigitsMask,
					houseType switch
					{
						HouseType.Block => SingleSubtype.FullHouseBlock,
						HouseType.Row => SingleSubtype.FullHouseRow,
						_ => SingleSubtype.FullHouseColumn
					},
					Technique.FullHouse,
					GetSubsetTechnique_Naked(in subsetCells, isLocked)
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
	/// Check for hidden single produced on naked subsets.
	/// </summary>
	/// <inheritdoc cref="CheckNakedSubsetFullHouse(ref AnalysisContext, ref readonly Grid, ref readonly CandidateMap, ref readonly CellMap, short, int, bool, bool?, List{CandidateViewNode})"/>
	private static DirectSubsetStep? CheckNakedSubsetHiddenSingle(
		scoped ref AnalysisContext context,
		scoped ref readonly Grid grid,
		scoped ref readonly CandidateMap conclusions,
		scoped ref readonly CellMap subsetCells,
		Mask subsetDigitsMask,
		House subsetHouse,
		bool searchingForLocked,
		bool? isLocked,
		List<CandidateViewNode> candidateOffsets
	)
	{
		foreach (var candidate in conclusions)
		{
			var cell = candidate / 9;
			var digit = candidate % 9;
			foreach (var houseType in HouseTypes)
			{
				var house = cell.ToHouseIndex(houseType);
				var eliminatedCells = (CellMap)from c in conclusions where c % 9 == digit select c / 9;
				var availableCells = (HousesMap[house] & CandidatesMap[digit]) - eliminatedCells;
				if (availableCells is not [var lastCell])
				{
					continue;
				}

				var step = new DirectSubsetStep(
					[new(Assignment, lastCell, digit)],
					[
						[
							.. candidateOffsets,
							.. SingleModule.GetHiddenSingleExcluders(in grid, digit, house, lastCell, out var chosenCells),
							..
							from c in HousesMap[house] & eliminatedCells
							select new CandidateViewNode(ColorIdentifier.Elimination, c * 9 + digit),
							new CellViewNode(ColorIdentifier.Auxiliary3, lastCell) { RenderingMode = DirectModeOnly },
							new CandidateViewNode(ColorIdentifier.Elimination, lastCell * 9 + digit),
							new HouseViewNode(ColorIdentifier.Normal, subsetHouse),
							new HouseViewNode(ColorIdentifier.Auxiliary3, house)
						]
					],
					context.PredefinedOptions,
					lastCell,
					digit,
					in subsetCells,
					subsetDigitsMask,
					house,
					in eliminatedCells,
					(Mask)(1 << digit),
					SingleModule.GetHiddenSingleSubtype(in grid, lastCell, house, in chosenCells),
					houseType switch
					{
						HouseType.Block => Technique.CrosshatchingBlock,
						HouseType.Row => Technique.CrosshatchingRow,
						_ => Technique.CrosshatchingColumn
					},
					GetSubsetTechnique_Naked(in subsetCells, isLocked)
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
	/// Check for naked single produced on naked subsets.
	/// </summary>
	/// <inheritdoc cref="CheckNakedSubsetFullHouse(ref AnalysisContext, ref readonly Grid, ref readonly CandidateMap, ref readonly CellMap, short, int, bool, bool?, List{CandidateViewNode})"/>
	private static DirectSubsetStep? CheckNakedSubsetNakedSingle(
		scoped ref AnalysisContext context,
		scoped ref readonly Grid grid,
		scoped ref readonly CandidateMap conclusions,
		scoped ref readonly CellMap subsetCells,
		Mask subsetDigitsMask,
		House subsetHouse,
		bool searchingForLocked,
		bool? isLocked,
		List<CandidateViewNode> candidateOffsets
	)
	{
		foreach (var candidate in conclusions)
		{
			var cell = candidate / 9;
			var digit = candidate % 9;
			var eliminatedDigitsMask = MaskOperations.Create(from c in conclusions where c / 9 == cell select c % 9);
			var availableDigitsMask = (Mask)(grid.GetCandidates(cell) & (Mask)~eliminatedDigitsMask);
			if (!IsPow2(availableDigitsMask))
			{
				continue;
			}

			var lastDigit = Log2((uint)availableDigitsMask);
			var step = new DirectSubsetStep(
				[new(Assignment, cell, lastDigit)],
				[
					[
						.. candidateOffsets,
						.. SingleModule.GetNakedSingleExcluders(in grid, cell, lastDigit, out _),
						new HouseViewNode(ColorIdentifier.Normal, subsetHouse),
						new CellViewNode(ColorIdentifier.Auxiliary3, cell) { RenderingMode = DirectModeOnly },
						new CandidateViewNode(ColorIdentifier.Elimination, cell * 9 + digit)
					]
				],
				context.PredefinedOptions,
				cell,
				lastDigit,
				in subsetCells,
				subsetDigitsMask,
				subsetHouse,
				[cell],
				eliminatedDigitsMask,
				SingleSubtype.NakedSingle0 + (HousesMap[cell.ToHouseIndex(HouseType.Block)] - EmptyCells).Count,
				Technique.NakedSingle,
				GetSubsetTechnique_Naked(in subsetCells, isLocked)
			);
			if (context.OnlyFindOne)
			{
				return step;
			}

			context.Accumulator.Add(step);
		}

		return null;
	}
}
