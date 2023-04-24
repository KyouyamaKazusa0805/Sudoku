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
[StepSearcher, ConditionalCases(ConditionalCase.Standard)]
public sealed partial class ReverseBivalueUniversalGraveStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates an array to be used as steps for a loop, for the cases when the number of values of the digits chosen are an even.
	/// </summary>
	private static readonly int[] IncrementStepsEven = { 2, 4 };

	/// <summary>
	/// Indicates an array to be used as steps for a loop, for the cases when the number of values of the digits chosen are an odd.
	/// </summary>
	private static readonly int[] IncrementStepsOdd = { 1, 3 };


	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		// Collect all possible digits can be used for the final construction of reverse BUGs.
		var digits = (Mask)0;
		for (var digit = 0; digit < 9; digit++)
		{
			// Check whether the digit can be used.
			// If the number of values for the current digit is 8 or 9, it may not be used for searching for reverse BUGs.
			if (ValuesMap[digit].Count <= 7)
			{
				digits |= (Mask)(1 << digit);
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

		// Iterates on all combinations of digits, with length of each combination 2.
		foreach (var digitPair in digits.GetAllSets().GetSubsets(2))
		{
			var d1 = digitPair[0];
			var d2 = digitPair[1];
			var comparer = (Mask)(1 << d1 | 1 << d2);
			var valuesMap = ValuesMap[d1] | ValuesMap[d2];

			// This loop is used for appending new empty cells into the varible 'valuesMap'.
			// Reverse BUGs can be split into two parts: Reverse URs and Reverse ULs.
			// Both of them are used cells, of length being a even number.
			// If the variable 'valuesMap' holds an odd number of cells,
			// we should append 2 or 4 cells; otherwise, 1 or 3 cells, to achieve this.
			// The total number of empty cells chosen may not be greater than 4,
			// because eliminations in such constructed pattern may not exist. In addition, only type 2 will use at most 4 empty cells;
			// other types will only use 1 or 2 empty cells.

			//                                                               [2, 4]               [1, 3]
			foreach (var incrementStep in (valuesMap.Count & 1) == 0 ? IncrementStepsEven : IncrementStepsOdd)
			{
				foreach (var cellsChosen in emptyCells & incrementStep)
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
	private Step? CheckType1(
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
			cellOffsets.Add(new(cellsChosen.Contains(cell) ? WellKnownColorIdentifierKind.Auxiliary1 : WellKnownColorIdentifierKind.Normal, cell));
		}

		var step = new ReverseBivalueUniversalGraveType1Step(
			new[] { conclusion },
			new[] { View.Empty | cellOffsets },
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
	private Step? CheckType2(
		scoped ref AnalysisContext context,
		Digit d1,
		Digit d2,
		Mask comparer,
		scoped in CellMap completePattern,
		scoped in CellMap cellsChosen
	)
	{
		var lastDigitsMask = (Mask)(context.Grid.GetDigitsUnion(cellsChosen) & ~comparer);
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

		var conclusions = from cell in elimMap select new Conclusion(Elimination, cell, extraDigit);
		var cellOffsets = new List<CellViewNode>(completePattern.Count);
		foreach (var cell in completePattern)
		{
			cellOffsets.Add(new(cellsChosen.Contains(cell) ? WellKnownColorIdentifierKind.Auxiliary1 : WellKnownColorIdentifierKind.Normal, cell));
		}

		var candidateOffsets = new List<CandidateViewNode>(cellsChosen.Count);
		foreach (var cell in cellsChosen)
		{
			candidateOffsets.Add(new(WellKnownColorIdentifierKind.Normal, cell * 9 + extraDigit));
		}

		var step = new ReverseBivalueUniversalGraveType2Step(
			conclusions,
			new[] { View.Empty | cellOffsets | candidateOffsets },
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
	private Step? CheckType4(
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
					new(cellsChosen.Contains(cell) ? WellKnownColorIdentifierKind.Auxiliary1 : WellKnownColorIdentifierKind.Normal, cell)
				);
			}

			var step = new ReverseBivalueUniversalGraveType4Step(
				new[] { conclusion },
				new[]
				{
					View.Empty
						| cellOffsets
						| new CandidateViewNode(WellKnownColorIdentifierKind.Auxiliary1, conjugatePairCellInnerPattern * 9 + selectedDigit)
						| new CandidateViewNode(WellKnownColorIdentifierKind.Auxiliary1, conjugatePairCellOuterPattern * 9 + selectedDigit)
						| new HouseViewNode(WellKnownColorIdentifierKind.Auxiliary1, house)
						| new LinkViewNode(
							WellKnownColorIdentifierKind.Normal,
							new(selectedDigit, conjugatePairCellInnerPattern),
							new(selectedDigit, anotherCell),
							Inference.Weak
						)
						| new LinkViewNode(
							WellKnownColorIdentifierKind.Normal,
							new(selectedDigit, conjugatePairCellOuterPattern),
							new(selectedDigit, anotherCell),
							Inference.Weak
						)
				},
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
