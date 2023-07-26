namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Reverse Bivalue Universal Grave</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Reverse Bivalue Universal Grave Type 1</item>
/// <item>Reverse Bivalue Universal Grave Type 2</item>
/// <item>Reverse Bivalue Universal Grave Type 4</item>
/// </list>
/// </summary>
[StepSearcher(
	Technique.ReverseBivalueUniversalGraveType1, Technique.ReverseBivalueUniversalGraveType2,
	Technique.ReverseBivalueUniversalGraveType3, Technique.ReverseBivalueUniversalGraveType4,
	ConditionalCases = ConditionalCase.Standard | ConditionalCase.TimeComplexity)]
public sealed partial class ReverseBivalueUniversalGraveStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates the global maps. The length of this field is always 7.
	/// </summary>
	private static readonly CellMap[] GlobalMaps = [~CellMap.Empty, .. from chute in Chutes select chute.Cells];


	/// <summary>
	/// Indicates whether the searcher can search for partially-used types, meaning the formed reverse BUG pattern may <b>not</b> be required
	/// occupying <b>all</b> value cells of two digits. The default value is <see langword="true"/>.
	/// </summary>
	[RuntimeIdentifier(RuntimeIdentifier.SearchForReverseBugPartiallyUsedTypes)]
	public bool AllowPartiallyUsedTypes { get; set; } = true;

	/// <summary>
	/// Indicates the maximum number of cells the step searcher will be searched for. This value controls the complexity of the technique.
	/// The maximum value is 4, and recommend value is 2. This value cannot be negative. The default value is 4.
	/// </summary>
	[RuntimeIdentifier(RuntimeIdentifier.ReverseBugMaxSearchingEmptyCellsCount)]
	public int MaxSearchingEmptyCellsCount { get; set; } = 4;


	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		// Collect all possible digits can be used for the final construction of reverse BUGs.
		scoped ref readonly var grid = ref context.Grid;
		Mask digits;
		if (AllowPartiallyUsedTypes)
		{
			digits = Grid.MaxCandidatesMask;
		}
		else
		{
			digits = 0;
			for (var digit = 0; digit < 9; digit++)
			{
				// Check whether the digit can be used.
				// If the number of values for the current digit is 8 or 9, it may not be used for searching for reverse BUGs.
				if (ValuesMap[digit].Count <= 7)
				{
					digits |= (Mask)(1 << digit);
				}
			}
		}

		// Remove naked singles.
		var emptyCells = EmptyCells;
		foreach (var cell in EmptyCells)
		{
			if (IsPow2(context.Grid.GetCandidates(cell)))
			{
				emptyCells.Remove(cell);
			}
		}

		var globalMapUpperBound = AllowPartiallyUsedTypes ? GlobalMaps.Length : 1;

		// Iterates on all combinations of digits, with length of each combination 2.
		foreach (var digitPair in digits.GetAllSets().GetSubsets(2))
		{
			var d1 = digitPair[0];
			var d2 = digitPair[1];
			var comparer = (Mask)(1 << d1 | 1 << d2);

			for (var i = 0; i < globalMapUpperBound; i++)
			{
				scoped ref readonly var globalMap = ref GlobalMaps[i];
				var baseValuesMap = ValuesMap[d1] | ValuesMap[d2];
				if (baseValuesMap.Count >= 16)
				{
					continue;
				}

				var valuesMap = i == 0 ? baseValuesMap : globalMap & baseValuesMap;

				// Extra check: If the global map doesn't use all cells of a grid, we should check the other 2 floors/towers.
				// If those floors/towers are not filled one of two digits, the pattern won't be formed, neither.
				var d1Map = ValuesMap[d1] - globalMap;
				var d2Map = ValuesMap[d2] - globalMap;
				var d1Counter = d1Map.Count;
				var d2Counter = d2Map.Count;
				switch (i)
				{
					case 0 when (d1Counter, d2Counter) is ( >= 7, _) or (_, >= 7):
					{
						continue;
					}
					default:
					{
						var flag = true;
						foreach (var c in d1Map | d2Map)
						{
							if (grid.GetStatus(c) == CellStatus.Given)
							{
								flag = false;
								break;
							}
						}
						if (!flag)
						{
							// The last cells cannot contain givens of digit 1 or 2.
							continue;
						}

						break;
					}
				}

				// The following loop is used for appending new empty cells into the variable 'valuesMap'.
				// Reverse BUGs can be split into two parts: Reverse URs and Reverse ULs.
				// Both of them are used cells, of length being a even number.
				// If the variable 'valuesMap' holds an odd number of cells,
				// we should append 2 or 4 cells; otherwise, 1 or 3 cells, to achieve this.
				// The total number of empty cells chosen may not be greater than 4,
				// because eliminations in such constructed pattern may not exist. In addition, only type 2 will use at most 4 empty cells;
				// other types will only use 1 or 2 empty cells.
				for (
					var incrementStep = (valuesMap.Count & 1) == 0 ? 2 : 1;
					incrementStep < Min(18 - d1Counter - d2Counter, MaxSearchingEmptyCellsCount);
					incrementStep += 2
				)
				{
					foreach (var cellsChosen in emptyCells.GetSubsets(incrementStep))
					{
						var completePattern = valuesMap | cellsChosen;
						if (!UniqueLoopStepSearcherHelper.IsGeneralizedUniqueLoop(completePattern))
						{
							// This pattern is invalid.
							continue;
						}

						if (CheckType1(ref context, d1, d2, comparer, completePattern, cellsChosen) is { } type1Step)
						{
							return type1Step;
						}
						if (CheckType2(ref context, d1, d2, comparer, completePattern, cellsChosen) is { } type2Step)
						{
							return type2Step;
						}
						if (CheckType4(ref context, d1, d2, comparer, completePattern, cellsChosen) is { } type4Step)
						{
							return type4Step;
						}
					}
				}
			}
		}

		return null;
	}

	/// <summary>
	/// Check for type 1.
	/// </summary>
	/// <param name="context"><inheritdoc cref="Collect(ref AnalysisContext)" path="/param[@name='context']"/></param>
	/// <param name="d1">The first digit used.</param>
	/// <param name="d2">The second digit used.</param>
	/// <param name="comparer">A mask that contains the digits <paramref name="d1"/> and <paramref name="d2"/>.</param>
	/// <param name="completePattern">The complete pattern.</param>
	/// <param name="cellsChosen">The empty cells chosen.</param>
	/// <returns><inheritdoc cref="Collect(ref AnalysisContext)" path="/returns"/></returns>
	private ReverseBivalueUniversalGraveType1Step? CheckType1(
		scoped ref AnalysisContext context,
		Digit d1,
		Digit d2,
		Mask comparer,
		scoped in CellMap completePattern,
		scoped in CellMap cellsChosen
	)
	{
		if (cellsChosen is not [var extraCell])
		{
			return null;
		}

		var mask = context.Grid.GetCandidates(extraCell);
		if ((mask & comparer) is not (var elimDigitsMask and not 0))
		{
			return null;
		}

		var elimDigit = TrailingZeroCount(elimDigitsMask);
		var conclusion = new Conclusion(Elimination, extraCell, elimDigit);

		var cellOffsets = new List<CellViewNode>(completePattern.Count);
		foreach (var cell in completePattern)
		{
			cellOffsets.Add(new(cellsChosen.Contains(cell) ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal, cell));
		}

		var step = new ReverseBivalueUniversalGraveType1Step(
			[conclusion],
			[[.. cellOffsets]],
			d1,
			d2,
			completePattern,
			cellsChosen
		);
		if (context.OnlyFindOne)
		{
			return step;
		}

		context.Accumulator.Add(step);
		return null;
	}

	/// <summary>
	/// Check for type 2.
	/// </summary>
	/// <param name="context"><inheritdoc cref="Collect(ref AnalysisContext)" path="/param[@name='context']"/></param>
	/// <param name="d1">The first digit used.</param>
	/// <param name="d2">The second digit used.</param>
	/// <param name="comparer">A mask that contains the digits <paramref name="d1"/> and <paramref name="d2"/>.</param>
	/// <param name="completePattern">The complete pattern.</param>
	/// <param name="cellsChosen">The empty cells chosen.</param>
	/// <returns><inheritdoc cref="Collect(ref AnalysisContext)" path="/returns"/></returns>
	private ReverseBivalueUniversalGraveType2Step? CheckType2(
		scoped ref AnalysisContext context,
		Digit d1,
		Digit d2,
		Mask comparer,
		scoped in CellMap completePattern,
		scoped in CellMap cellsChosen
	)
	{
		var lastDigitsMask = (Mask)(context.Grid[cellsChosen] & ~comparer);
		if (!IsPow2(lastDigitsMask))
		{
			return null;
		}

		var extraDigit = TrailingZeroCount(lastDigitsMask);
		var elimMap = cellsChosen.PeerIntersection & EmptyCells & CandidatesMap[extraDigit];
		if (!elimMap)
		{
			return null;
		}

		var cellOffsets = new List<CellViewNode>(completePattern.Count);
		foreach (var cell in completePattern)
		{
			cellOffsets.Add(new(cellsChosen.Contains(cell) ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal, cell));
		}

		var candidateOffsets = new List<CandidateViewNode>(cellsChosen.Count);
		foreach (var cell in cellsChosen)
		{
			candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + extraDigit));
		}

		var step = new ReverseBivalueUniversalGraveType2Step(
			from cell in elimMap select new Conclusion(Elimination, cell, extraDigit),
			[[.. cellOffsets, .. candidateOffsets]],
			d1,
			d2,
			extraDigit,
			completePattern,
			cellsChosen
		);
		if (context.OnlyFindOne)
		{
			return step;
		}

		context.Accumulator.Add(step);

		return null;
	}

	/// <summary>
	/// Check for type 4.
	/// </summary>
	/// <param name="context"><inheritdoc cref="Collect(ref AnalysisContext)" path="/param[@name='context']"/></param>
	/// <param name="d1">The first digit used.</param>
	/// <param name="d2">The second digit used.</param>
	/// <param name="comparer">A mask that contains the digits <paramref name="d1"/> and <paramref name="d2"/>.</param>
	/// <param name="completePattern">The complete pattern.</param>
	/// <param name="cellsChosen">The empty cells chosen.</param>
	/// <returns><inheritdoc cref="Collect(ref AnalysisContext)" path="/returns"/></returns>
	private ReverseBivalueUniversalGraveType4Step? CheckType4(
		scoped ref AnalysisContext context,
		Digit d1,
		Digit d2,
		Mask comparer,
		scoped in CellMap completePattern,
		scoped in CellMap cellsChosen
	)
	{
		if (cellsChosen is not [var cell1, var cell2])
		{
			return null;
		}

		if (cellsChosen.InOneHouse)
		{
			return null;
		}

		scoped ref readonly var grid = ref context.Grid;
		var cell1Digit = grid.GetCandidates(cell1) & comparer;
		var cell2Digit = grid.GetCandidates(cell2) & comparer;
		var mergedDigitMask = cell1Digit | cell2Digit;
		if (!IsPow2(mergedDigitMask))
		{
			return null;
		}

		var selectedDigit = TrailingZeroCount(mergedDigitMask);
		if (!((grid.Exists(cell1, selectedDigit) ?? false) && (grid.Exists(cell2, selectedDigit) ?? false)))
		{
			// We should ensure all chosen cells (empty cells) contain the selected digit.
			return null;
		}

		foreach (var house in cellsChosen.Houses)
		{
			var possibleConjugatePairCells = CandidatesMap[selectedDigit] & HousesMap[house];
			if (possibleConjugatePairCells.Count != 2)
			{
				continue;
			}

			var conjugatePairCellOuterPattern = (possibleConjugatePairCells - cellsChosen)[0];
			var conjugatePairCellInnerPattern = (possibleConjugatePairCells - conjugatePairCellOuterPattern)[0];
			var anotherCell = (cellsChosen - conjugatePairCellInnerPattern)[0];
			if (!(CellsMap[anotherCell] + conjugatePairCellOuterPattern).InOneHouse)
			{
				continue;
			}

			var conclusion = new Conclusion(Elimination, anotherCell, selectedDigit);
			var cellOffsets = new List<CellViewNode>(completePattern.Count);
			foreach (var cell in completePattern)
			{
				cellOffsets.Add(
					new(cellsChosen.Contains(cell) ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal, cell)
				);
			}

			var lockedTargetInner = new LockedTarget(selectedDigit, conjugatePairCellInnerPattern);
			var lockedTargetOuter = new LockedTarget(selectedDigit, conjugatePairCellOuterPattern);
			var anotherLockedTarget = new LockedTarget(selectedDigit, anotherCell);
			var step = new ReverseBivalueUniversalGraveType4Step(
				[conclusion],
				[
					[
						.. cellOffsets,
						new CandidateViewNode(WellKnownColorIdentifier.Auxiliary1, conjugatePairCellInnerPattern * 9 + selectedDigit),
						new CandidateViewNode(WellKnownColorIdentifier.Auxiliary1, conjugatePairCellOuterPattern * 9 + selectedDigit),
						new HouseViewNode(WellKnownColorIdentifier.Normal, house),
						new LinkViewNode(WellKnownColorIdentifier.Normal, lockedTargetInner, anotherLockedTarget, Inference.Weak),
						new LinkViewNode(WellKnownColorIdentifier.Normal, lockedTargetOuter, anotherLockedTarget, Inference.Weak)
					]
				],
				d1,
				d2,
				completePattern,
				cellsChosen,
				new(possibleConjugatePairCells, selectedDigit)
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
