namespace Sudoku.Analytics.StepSearchers;

using unsafe DirectSubsetHandlerFuncPtr = delegate*<DirectSubsetStepSearcher, List<DirectSubsetStep>, ref AnalysisContext, ref readonly Grid, int, bool, ref readonly CellMap, ReadOnlySpan<CellMap>, DirectSubsetStep?>;

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
	"StepSearcherName_DirectSubsetStepSearcher",

	// Complex Full House
	Technique.NakedPairFullHouse, Technique.NakedPairPlusFullHouse, Technique.HiddenPairFullHouse,
	Technique.LockedPairFullHouse, Technique.LockedHiddenPairFullHouse,
	Technique.NakedTripleFullHouse, Technique.NakedTriplePlusFullHouse, Technique.HiddenTripleFullHouse,
	Technique.LockedTripleFullHouse, Technique.LockedHiddenTripleFullHouse,
	Technique.NakedQuadrupleFullHouse, Technique.NakedQuadruplePlusFullHouse, Technique.HiddenQuadrupleFullHouse,

	// Complex Crosshatching Block
	Technique.NakedPairCrosshatchingBlock, Technique.NakedPairPlusCrosshatchingBlock, Technique.HiddenPairCrosshatchingBlock,
	Technique.LockedPairCrosshatchingBlock, Technique.LockedHiddenPairCrosshatchingBlock,
	Technique.NakedTripleCrosshatchingBlock, Technique.NakedTriplePlusCrosshatchingBlock, Technique.HiddenTripleCrosshatchingBlock,
	Technique.LockedTripleCrosshatchingBlock, Technique.LockedHiddenTripleCrosshatchingBlock,
	Technique.NakedQuadrupleCrosshatchingBlock, Technique.NakedQuadruplePlusCrosshatchingBlock, Technique.HiddenQuadrupleCrosshatchingBlock,

	// Complex Crosshatching Row
	Technique.NakedPairCrosshatchingRow, Technique.NakedPairPlusCrosshatchingRow, Technique.HiddenPairCrosshatchingRow,
	Technique.LockedPairCrosshatchingRow, Technique.LockedHiddenPairCrosshatchingRow,
	Technique.NakedTripleCrosshatchingRow, Technique.NakedTriplePlusCrosshatchingRow, Technique.HiddenTripleCrosshatchingRow,
	Technique.LockedTripleCrosshatchingRow, Technique.LockedHiddenTripleCrosshatchingRow,
	Technique.NakedQuadrupleCrosshatchingRow, Technique.NakedQuadruplePlusCrosshatchingRow, Technique.HiddenQuadrupleCrosshatchingRow,

	// Complex Crosshatching Column
	Technique.NakedPairCrosshatchingColumn, Technique.NakedPairPlusCrosshatchingColumn, Technique.HiddenPairCrosshatchingColumn,
	Technique.LockedPairCrosshatchingColumn, Technique.LockedHiddenPairCrosshatchingColumn,
	Technique.NakedTripleCrosshatchingColumn, Technique.NakedTriplePlusCrosshatchingColumn, Technique.HiddenTripleCrosshatchingColumn,
	Technique.LockedTripleCrosshatchingColumn, Technique.LockedHiddenTripleCrosshatchingColumn,
	Technique.NakedQuadrupleCrosshatchingColumn, Technique.NakedQuadruplePlusCrosshatchingColumn, Technique.HiddenQuadrupleCrosshatchingColumn,

	// Complex Naked Single
	Technique.NakedPairNakedSingle, Technique.NakedPairPlusNakedSingle, Technique.HiddenPairNakedSingle,
	Technique.LockedPairNakedSingle, Technique.LockedHiddenPairNakedSingle,
	Technique.NakedTripleNakedSingle, Technique.NakedTriplePlusNakedSingle, Technique.HiddenTripleNakedSingle,
	Technique.LockedTripleNakedSingle, Technique.LockedHiddenTripleNakedSingle,
	Technique.NakedQuadrupleNakedSingle, Technique.NakedQuadruplePlusNakedSingle, Technique.HiddenQuadrupleNakedSingle,

	IsCachingSafe = true,
	IsAvailabilityReadOnly = true,
	IsOrderingFixed = true,
	RuntimeFlags = StepSearcherRuntimeFlags.DirectTechniquesOnly)]
public sealed partial class DirectSubsetStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates whether the step searcher allows searching for direct hidden subset.
	/// </summary>
	[SettingItemName(SettingItemNames.AllowDirectLockedSubset)]
	public bool AllowDirectHiddenSubset { get; set; }

	/// <summary>
	/// Indicates whether the step searcher allows searching for direct locked hidden subset.
	/// </summary>
	[SettingItemName(SettingItemNames.AllowDirectLockedHiddenSubset)]
	public bool AllowDirectLockedHiddenSubset { get; set; }

	/// <summary>
	/// Indicates whether the step searcher allows searching for direct naked subset.
	/// </summary>
	[SettingItemName(SettingItemNames.AllowDirectNakedSubset)]
	public bool AllowDirectNakedSubset { get; set; }

	/// <summary>
	/// Indicates whether the step searcher allows searching for direct locked subset.
	/// </summary>
	[SettingItemName(SettingItemNames.AllowDirectLockedSubset)]
	public bool AllowDirectLockedSubset { get; set; }

	/// <summary>
	/// Indicates the size of the naked subsets you want to search for (including locked subsets and naked subsets (+)).
	/// The maximum value is 4.
	/// </summary>
	[SettingItemName(SettingItemNames.DirectNakedSubsetMaxSize)]
	public int DirectNakedSubsetMaxSize { get; set; } = 2;

	/// <summary>
	/// Indicates the size of the hidden subsets you want to search for (including locked hidden subsets). The maximum value is 4.
	/// </summary>
	[SettingItemName(SettingItemNames.DirectHiddenSubsetMaxSize)]
	public int DirectHiddenSubsetMaxSize { get; set; } = 2;


	/// <inheritdoc/>
	protected internal override unsafe Step? Collect(ref AnalysisContext context)
	{
		var first = stackalloc DirectSubsetHandlerFuncPtr[] { &HiddenSubset, &NakedSubset };
		var second = stackalloc DirectSubsetHandlerFuncPtr[] { &NakedSubset, &HiddenSubset };
		var searchers = context.Options switch { { DistinctDirectMode: true, IsDirectMode: true } => first, _ => second };

		ref readonly var grid = ref context.Grid;
		var emptyCells = grid.EmptyCells;
		var candidatesMap = grid.CandidatesMap;
		var nakedSingleCells = CellMap.Empty;
		foreach (var cell in emptyCells)
		{
			if (IsPow2((uint)grid.GetCandidates(cell)))
			{
				nakedSingleCells.Add(cell);
			}
		}
		emptyCells &= ~nakedSingleCells;

		var accumulator = new List<DirectSubsetStep>();
		foreach (var searchingForLocked in (true, false))
		{
			for (var size = 2; size <= (searchingForLocked ? 3 : 4); size++)
			{
				if (searchers[0](this, accumulator, ref context, in grid, size, searchingForLocked, in emptyCells, candidatesMap) is { } step1)
				{
					return step1;
				}
				if (searchers[1](this, accumulator, ref context, in grid, size, searchingForLocked, in emptyCells, candidatesMap) is { } step2)
				{
					return step2;
				}
			}
		}

		if (!context.OnlyFindOne && accumulator.Count != 0)
		{
			var final = StepMarshal.RemoveDuplicateItems(accumulator);
			context.Accumulator.AddRange(final);
		}

		return null;
	}


	/// <summary>
	/// Search for hidden subsets.
	/// </summary>
	private static DirectSubsetStep? HiddenSubset(
		DirectSubsetStepSearcher @this,
		List<DirectSubsetStep> accumulator,
		ref AnalysisContext context,
		ref readonly Grid grid,
		int size,
		bool searchingForLocked,
		ref readonly CellMap emptyCells,
		ReadOnlySpan<CellMap> candidatesMap
	)
	{
		if (size > @this.DirectHiddenSubsetMaxSize)
		{
			return null;
		}

		for (var house = 0; house < 27; house++)
		{
			ref readonly var currentHouseCells = ref HousesMap[house];
			var traversingMap = currentHouseCells & emptyCells;
			var mask = grid[in traversingMap];
			foreach (var digits in mask.GetAllSets().GetSubsets(size))
			{
				var (tempMask, digitsMask, cells) = (mask, (Mask)0, CellMap.Empty);
				foreach (var digit in digits)
				{
					tempMask &= (Mask)~(1 << digit);
					digitsMask |= (Mask)(1 << digit);
					cells |= currentHouseCells & candidatesMap[digit];
				}
				if (cells.Count != size)
				{
					continue;
				}

				// Gather eliminations.
				var conclusions = CandidateMap.Empty;
				foreach (var digit in tempMask)
				{
					foreach (var cell in cells & candidatesMap[digit])
					{
						conclusions.Add(cell * 9 + digit);
					}
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				// Gather highlight candidates.
				var (cellOffsets, candidateOffsets) = (new List<IconViewNode>(), new List<CandidateViewNode>());
				foreach (var digit in digits)
				{
					foreach (var cell in cells & candidatesMap[digit])
					{
						candidateOffsets.Add(new(ColorIdentifier.Normal, cell * 9 + digit));
					}

					cellOffsets.AddRange(Excluder.GetSubsetExcluders(in grid, digit, house, in cells));
				}

				var isLocked = cells.IsInIntersection;
				if (!searchingForLocked || isLocked && searchingForLocked)
				{
					var containsExtraEliminations = false;
					if (isLocked)
					{
						// A potential locked hidden subset found. Extra eliminations should be checked.
						// Please note that here a hidden subset may not be a locked one because eliminations aren't validated.
						var eliminatingHouse = TrailingZeroCount(cells.SharedHouses & ~(1 << house));
						foreach (var cell in HousesMap[eliminatingHouse] & emptyCells & ~cells)
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
						@this, accumulator, ref context, in grid, in conclusions, in cells, digitsMask, house, searchingForLocked,
						containsExtraEliminations, cellOffsets, candidateOffsets, in emptyCells) is { } fullHouse)
					{
						return fullHouse;
					}
					if (CheckHiddenSubsetHiddenSingle(
						@this, accumulator, ref context, in grid, in conclusions, in cells, digitsMask, house, searchingForLocked,
						containsExtraEliminations, cellOffsets, candidateOffsets, candidatesMap) is { } hiddenSingle)
					{
						return hiddenSingle;
					}
					if (CheckHiddenSubsetNakedSingle(
						@this, accumulator, ref context, in grid, in conclusions, in cells, digitsMask, house, searchingForLocked,
						containsExtraEliminations, cellOffsets, candidateOffsets, in emptyCells) is { } nakedSingle)
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
		DirectSubsetStepSearcher @this,
		List<DirectSubsetStep> accumulator,
		ref AnalysisContext context,
		ref readonly Grid grid,
		int size,
		bool searchingForLocked,
		ref readonly CellMap emptyCells,
		ReadOnlySpan<CellMap> candidatesMap
	)
	{
		if (size > @this.DirectNakedSubsetMaxSize)
		{
			return null;
		}

		for (var house = 0; house < 27; house++)
		{
			if ((HousesMap[house] & emptyCells) is not { Count: >= 2 } currentEmptyMap)
			{
				continue;
			}

			// Remove cells that only contain 1 candidate (Naked Singles).
			foreach (var cell in HousesMap[house] & emptyCells)
			{
				if (IsPow2(grid.GetCandidates(cell)))
				{
					currentEmptyMap.Remove(cell);
				}
			}

			// Iterate on each combination.
			foreach (ref readonly var cells in currentEmptyMap & size)
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
					var map = cells % candidatesMap[digit];
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
					@this, accumulator, ref context, in grid, in conclusions, in cells, digitsMask, house,
					searchingForLocked, isLocked, candidateOffsets, in emptyCells) is { } fullHouse)
				{
					return fullHouse;
				}
				if (CheckNakedSubsetHiddenSingle(
					@this, accumulator, ref context, in grid, in conclusions, in cells, digitsMask, house,
					searchingForLocked, isLocked, candidateOffsets, candidatesMap) is { } hiddenSingle)
				{
					return hiddenSingle;
				}
				if (CheckNakedSubsetNakedSingle(
					@this, accumulator, ref context, in grid, in conclusions, in cells, digitsMask, house,
					searchingForLocked, isLocked, candidateOffsets, in emptyCells) is { } nakedSingle)
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
	/// <param name="this">Indicates the current type instance.</param>
	/// <param name="accumulator">Indicates the accumulator.</param>
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
	/// <param name="emptyCells">Indicates the empty cells.</param>
	/// <returns>The found step.</returns>
	private static DirectSubsetStep? CheckHiddenSubsetFullHouse(
		DirectSubsetStepSearcher @this,
		List<DirectSubsetStep> accumulator,
		ref AnalysisContext context,
		ref readonly Grid grid,
		ref readonly CandidateMap conclusions,
		ref readonly CellMap subsetCells,
		Mask subsetDigitsMask,
		House subsetHouse,
		bool searchingForLocked,
		bool containsExtraEliminations,
		List<IconViewNode> cellOffsets,
		List<CandidateViewNode> candidateOffsets,
		ref readonly CellMap emptyCells
	)
	{
		foreach (var cell in conclusions.Cells)
		{
			foreach (var houseType in HouseTypes)
			{
				var house = cell.ToHouse(houseType);
				var emptyCellsInHouse = HousesMap[house] & emptyCells;
				if (emptyCellsInHouse.Count <= 1)
				{
					continue;
				}

				// Check for candidates for the cell.
				var eliminatedDigitsMask = MaskOperations.Create(from c in conclusions where c / 9 == cell select c % 9);
				var valueDigitsMask = (Mask)(Grid.MaxCandidatesMask & ~grid[HousesMap[house] & ~emptyCellsInHouse, true]);
				var lastDigitsMask = (Mask)(valueDigitsMask & ~eliminatedDigitsMask);
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

				var subsetTechnique = GetSubsetTechnique_Hidden(in subsetCells);
				switch (subsetTechnique)
				{
					case Technique.LockedHiddenPair or Technique.LockedHiddenTriple
					when !@this.AllowDirectLockedHiddenSubset:
					case Technique.HiddenPair or Technique.HiddenTriple or Technique.HiddenQuadruple
					when !@this.AllowDirectHiddenSubset:
					{
						continue;
					}
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
					context.Options,
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
					subsetTechnique
				);
				if (context.OnlyFindOne)
				{
					return step;
				}

				accumulator.Add(step);
			}
		}

		return null;
	}

	/// <summary>
	/// Check for hidden single produced on hidden subsets.
	/// </summary>
	/// <inheritdoc cref="CheckHiddenSubsetFullHouse(DirectSubsetStepSearcher, List{DirectSubsetStep}, ref AnalysisContext, ref readonly Grid, ref readonly CandidateMap, ref readonly CellMap, Mask, House, bool, bool, List{IconViewNode}, List{CandidateViewNode}, ref readonly CellMap)"/>
	private static DirectSubsetStep? CheckHiddenSubsetHiddenSingle(
		DirectSubsetStepSearcher @this,
		List<DirectSubsetStep> accumulator,
		ref AnalysisContext context,
		ref readonly Grid grid,
		ref readonly CandidateMap conclusions,
		ref readonly CellMap subsetCells,
		Mask subsetDigitsMask,
		House subsetHouse,
		bool searchingForLocked,
		bool containsExtraEliminations,
		List<IconViewNode> cellOffsets,
		List<CandidateViewNode> candidateOffsets,
		ReadOnlySpan<CellMap> candidatesMap
	)
	{
		foreach (var (_, cell, digit) in conclusions.EnumerateCellDigit())
		{
			foreach (var houseType in HouseTypes)
			{
				var house = cell.ToHouse(houseType);
				var eliminatedCells = (from c in conclusions where c % 9 == digit select c / 9).AsCellMap();
				var availableCells = HousesMap[house] & candidatesMap[digit] & ~eliminatedCells;
				if (availableCells is not [var lastCell])
				{
					continue;
				}

				var subsetTechnique = GetSubsetTechnique_Hidden(in subsetCells);
				switch (subsetTechnique)
				{
					case Technique.LockedHiddenPair or Technique.LockedHiddenTriple
					when !@this.AllowDirectLockedHiddenSubset:
					case Technique.HiddenPair or Technique.HiddenTriple or Technique.HiddenQuadruple
					when !@this.AllowDirectHiddenSubset:
					{
						continue;
					}
				}

				var step = new DirectSubsetStep(
					[new(Assignment, lastCell, digit)],
					[
						[
							.. candidateOffsets,
							.. cellOffsets,
							.. Excluder.GetHiddenSingleExcluders(in grid, digit, house, lastCell, out var chosenCells, out _),
							..
							from c in HousesMap[house] & eliminatedCells
							select new CandidateViewNode(ColorIdentifier.Elimination, c * 9 + digit),
							new DiamondViewNode(ColorIdentifier.Auxiliary3, lastCell),
							new CandidateViewNode(ColorIdentifier.Elimination, lastCell * 9 + digit),
							new HouseViewNode(ColorIdentifier.Normal, subsetHouse),
							new HouseViewNode(ColorIdentifier.Auxiliary3, house)
						]
					],
					context.Options,
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
					subsetTechnique
				);
				if (context.OnlyFindOne)
				{
					return step;
				}

				accumulator.Add(step);
			}
		}

		return null;
	}

	/// <summary>
	/// Check for naked single produced on hidden subsets.
	/// </summary>
	/// <inheritdoc cref="CheckHiddenSubsetFullHouse(DirectSubsetStepSearcher, List{DirectSubsetStep}, ref AnalysisContext, ref readonly Grid, ref readonly CandidateMap, ref readonly CellMap, Mask, House, bool, bool, List{IconViewNode}, List{CandidateViewNode}, ref readonly CellMap)"/>
	private static DirectSubsetStep? CheckHiddenSubsetNakedSingle(
		DirectSubsetStepSearcher @this,
		List<DirectSubsetStep> accumulator,
		ref AnalysisContext context,
		ref readonly Grid grid,
		ref readonly CandidateMap conclusions,
		ref readonly CellMap subsetCells,
		Mask subsetDigitsMask,
		House subsetHouse,
		bool searchingForLocked,
		bool containsExtraEliminations,
		List<IconViewNode> cellOffsets,
		List<CandidateViewNode> candidateOffsets,
		ref readonly CellMap emptyCells
	)
	{
		foreach (var cell in conclusions.Cells)
		{
			var eliminatedDigitsMask = MaskOperations.Create(from c in conclusions where c / 9 == cell select c % 9);
			var availableDigitsMask = (Mask)(grid.GetCandidates(cell) & ~eliminatedDigitsMask);
			if (!IsPow2(availableDigitsMask))
			{
				return null;
			}

			var subsetTechnique = GetSubsetTechnique_Hidden(in subsetCells);
			switch (subsetTechnique)
			{
				case Technique.LockedHiddenPair or Technique.LockedHiddenTriple
				when !@this.AllowDirectLockedHiddenSubset:
				case Technique.HiddenPair or Technique.HiddenTriple or Technique.HiddenQuadruple
				when !@this.AllowDirectHiddenSubset:
				{
					return null;
				}
			}

			var lastDigit = Log2((uint)availableDigitsMask);
			var step = new DirectSubsetStep(
				[new(Assignment, cell, lastDigit)],
				[
					[
						.. candidateOffsets,
						.. cellOffsets,
						.. Excluder.GetNakedSingleExcluders(in grid, cell, lastDigit, out _),
						new HouseViewNode(ColorIdentifier.Normal, subsetHouse),
						new CellViewNode(ColorIdentifier.Auxiliary3, cell)
					]
				],
				context.Options,
				cell,
				lastDigit,
				in subsetCells,
				subsetDigitsMask,
				subsetHouse,
				[cell],
				eliminatedDigitsMask,
				SingleModule.GetNakedSingleSubtype(in grid, cell),
				Technique.NakedSingle,
				subsetTechnique
			);
			if (context.OnlyFindOne)
			{
				return step;
			}

			accumulator.Add(step);
		}

		return null;
	}

	/// <summary>
	/// Check for full house produced on naked subsets.
	/// </summary>
	/// <param name="this">Indicates the current type instance.</param>
	/// <param name="accumulator">Indicates the accumulator.</param>
	/// <param name="context">The context.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="conclusions">The conclusions produced by hidden subsets.</param>
	/// <param name="subsetCells">The subset cells used.</param>
	/// <param name="subsetDigitsMask">The digits that the subset pattern used.</param>
	/// <param name="subsetHouse">The house producing the subset.</param>
	/// <param name="searchingForLocked">Indicates whether the current mode is for searching locked hidden subsets.</param>
	/// <param name="isLocked">Indicates whether the subset is locked.</param>
	/// <param name="candidateOffsets">Indicates the candidate offsets.</param>
	/// <param name="emptyCells">Indicates the empty cells.</param>
	/// <returns>The found step.</returns>
	private static DirectSubsetStep? CheckNakedSubsetFullHouse(
		DirectSubsetStepSearcher @this,
		List<DirectSubsetStep> accumulator,
		ref AnalysisContext context,
		ref readonly Grid grid,
		ref readonly CandidateMap conclusions,
		ref readonly CellMap subsetCells,
		Mask subsetDigitsMask,
		House subsetHouse,
		bool searchingForLocked,
		bool? isLocked,
		List<CandidateViewNode> candidateOffsets,
		ref readonly CellMap emptyCells
	)
	{
		foreach (var (_, cell, digit) in conclusions.EnumerateCellDigit())
		{
			foreach (var houseType in HouseTypes)
			{
				var house = cell.ToHouse(houseType);
				var emptyCellsInHouse = HousesMap[house] & emptyCells;
				if (emptyCellsInHouse.Count <= 1)
				{
					continue;
				}

				// Check for candidates for the cell.
				var eliminatedDigitsMask = MaskOperations.Create(from c in conclusions where c / 9 == cell select c % 9);
				var valueDigitsMask = (Mask)(Grid.MaxCandidatesMask & ~grid[HousesMap[house] & ~emptyCellsInHouse, true]);
				var lastDigitsMask = (Mask)(valueDigitsMask & ~eliminatedDigitsMask);
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

				var subsetTechnique = GetSubsetTechnique_Naked(in subsetCells, isLocked);
				switch (subsetTechnique)
				{
					case Technique.LockedPair or Technique.LockedTriple
					when !@this.AllowDirectLockedSubset:
					case Technique.NakedPairPlus or Technique.NakedTriplePlus or Technique.NakedQuadruplePlus
					when !@this.AllowDirectNakedSubset:
					case Technique.NakedPair or Technique.NakedTriple or Technique.NakedQuadruple
					when !@this.AllowDirectNakedSubset:
					{
						continue;
					}
				}

				var step = new DirectSubsetStep(
					[new(Assignment, cell, lastDigit)],
					[
						[
							.. candidateOffsets,
							new CandidateViewNode(ColorIdentifier.Elimination, cell * 9 + digit),
							new CellViewNode(ColorIdentifier.Auxiliary3, cell),
							new HouseViewNode(ColorIdentifier.Normal, subsetHouse),
							new HouseViewNode(ColorIdentifier.Auxiliary3, house)
						]
					],
					context.Options,
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
					subsetTechnique
				);
				if (context.OnlyFindOne)
				{
					return step;
				}

				accumulator.Add(step);
			}
		}

		return null;
	}

	/// <summary>
	/// Check for hidden single produced on naked subsets.
	/// </summary>
	/// <inheritdoc cref="CheckNakedSubsetFullHouse(DirectSubsetStepSearcher, List{DirectSubsetStep}, ref AnalysisContext, ref readonly Grid, ref readonly CandidateMap, ref readonly CellMap, short, int, bool, bool?, List{CandidateViewNode}, ref readonly CellMap)"/>
	private static DirectSubsetStep? CheckNakedSubsetHiddenSingle(
		DirectSubsetStepSearcher @this,
		List<DirectSubsetStep> accumulator,
		ref AnalysisContext context,
		ref readonly Grid grid,
		ref readonly CandidateMap conclusions,
		ref readonly CellMap subsetCells,
		Mask subsetDigitsMask,
		House subsetHouse,
		bool searchingForLocked,
		bool? isLocked,
		List<CandidateViewNode> candidateOffsets,
		ReadOnlySpan<CellMap> candidatesMap
	)
	{
		foreach (var (_, cell, digit) in conclusions.EnumerateCellDigit())
		{
			foreach (var houseType in HouseTypes)
			{
				var house = cell.ToHouse(houseType);
				var eliminatedCells = (from c in conclusions where c % 9 == digit select c / 9).AsCellMap();
				var availableCells = HousesMap[house] & candidatesMap[digit] & ~eliminatedCells;
				if (availableCells is not [var lastCell])
				{
					continue;
				}

				var subsetTechnique = GetSubsetTechnique_Naked(in subsetCells, isLocked);
				switch (subsetTechnique)
				{
					case Technique.LockedPair or Technique.LockedTriple
					when !@this.AllowDirectLockedSubset:
					case Technique.NakedPairPlus or Technique.NakedTriplePlus or Technique.NakedQuadruplePlus
					when !@this.AllowDirectNakedSubset:
					case Technique.NakedPair or Technique.NakedTriple or Technique.NakedQuadruple
					when !@this.AllowDirectNakedSubset:
					{
						continue;
					}
				}

				var step = new DirectSubsetStep(
					[new(Assignment, lastCell, digit)],
					[
						[
							.. candidateOffsets,
							.. Excluder.GetHiddenSingleExcluders(in grid, digit, house, lastCell, out var chosenCells, out _),
							..
							from c in HousesMap[house] & eliminatedCells
							select new CandidateViewNode(ColorIdentifier.Elimination, c * 9 + digit),
							new DiamondViewNode(ColorIdentifier.Auxiliary3, lastCell),
							new CandidateViewNode(ColorIdentifier.Elimination, lastCell * 9 + digit),
							new HouseViewNode(ColorIdentifier.Normal, subsetHouse),
							new HouseViewNode(ColorIdentifier.Auxiliary3, house)
						]
					],
					context.Options,
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
					subsetTechnique
				);
				if (context.OnlyFindOne)
				{
					return step;
				}

				accumulator.Add(step);
			}
		}

		return null;
	}

	/// <summary>
	/// Check for naked single produced on naked subsets.
	/// </summary>
	/// <inheritdoc cref="CheckNakedSubsetFullHouse(DirectSubsetStepSearcher, List{DirectSubsetStep}, ref AnalysisContext, ref readonly Grid, ref readonly CandidateMap, ref readonly CellMap, short, int, bool, bool?, List{CandidateViewNode}, ref readonly CellMap)"/>
	private static DirectSubsetStep? CheckNakedSubsetNakedSingle(
		DirectSubsetStepSearcher @this,
		List<DirectSubsetStep> accumulator,
		ref AnalysisContext context,
		ref readonly Grid grid,
		ref readonly CandidateMap conclusions,
		ref readonly CellMap subsetCells,
		Mask subsetDigitsMask,
		House subsetHouse,
		bool searchingForLocked,
		bool? isLocked,
		List<CandidateViewNode> candidateOffsets,
		ref readonly CellMap emptyCells
	)
	{
		foreach (var (_, cell, digit) in conclusions.EnumerateCellDigit())
		{
			var eliminatedDigitsMask = MaskOperations.Create(from c in conclusions where c / 9 == cell select c % 9);
			var availableDigitsMask = (Mask)(grid.GetCandidates(cell) & ~eliminatedDigitsMask);
			if (!IsPow2(availableDigitsMask))
			{
				continue;
			}

			var subsetTechnique = GetSubsetTechnique_Naked(in subsetCells, isLocked);
			switch (subsetTechnique)
			{
				case Technique.LockedPair or Technique.LockedTriple
				when !@this.AllowDirectLockedSubset:
				case Technique.NakedPairPlus or Technique.NakedTriplePlus or Technique.NakedQuadruplePlus
				when !@this.AllowDirectNakedSubset:
				case Technique.NakedPair or Technique.NakedTriple or Technique.NakedQuadruple
				when !@this.AllowDirectNakedSubset:
				{
					continue;
				}
			}

			var lastDigit = Log2((uint)availableDigitsMask);
			var step = new DirectSubsetStep(
				[new(Assignment, cell, lastDigit)],
				[
					[
						.. candidateOffsets,
						.. Excluder.GetNakedSingleExcluders(in grid, cell, lastDigit, out _),
						new HouseViewNode(ColorIdentifier.Normal, subsetHouse),
						new DiamondViewNode(ColorIdentifier.Auxiliary3, cell),
						new CandidateViewNode(ColorIdentifier.Elimination, cell * 9 + digit)
					]
				],
				context.Options,
				cell,
				lastDigit,
				in subsetCells,
				subsetDigitsMask,
				subsetHouse,
				[cell],
				eliminatedDigitsMask,
				SingleModule.GetNakedSingleSubtype(in grid, cell),
				Technique.NakedSingle,
				subsetTechnique
			);
			if (context.OnlyFindOne)
			{
				return step;
			}

			accumulator.Add(step);
		}

		return null;
	}


	/// <summary>
	/// Gets the <see cref="Technique"/> field describing the subset usage on hidden subsets.
	/// </summary>
	/// <param name="subsetCells">The subset cells used.</param>
	/// <returns>The final result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Technique GetSubsetTechnique_Hidden(ref readonly CellMap subsetCells)
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
	private static Technique GetSubsetTechnique_Naked(ref readonly CellMap subsetCells, bool? isLocked)
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
}
