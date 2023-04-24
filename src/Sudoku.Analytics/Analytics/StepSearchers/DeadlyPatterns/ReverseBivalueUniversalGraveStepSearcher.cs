namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Reverse Bi-value Universal Grave</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Reverse Unique Rectangle</item>
/// <item>Reverse Unique Loop</item>
/// <item>Reverse Bivalue Universal Grave (Separated Type)</item>
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
					if (CheckType3(ref context) is { } type3Step)
					{
						return type3Step;
					}
					if (CheckType4(ref context) is { } type4Step)
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
	/// Check for type 3.
	/// </summary>
	/// <param name="context"><inheritdoc cref="Collect(ref AnalysisContext)" path="/param[@name='context']"/></param>
	/// <returns><inheritdoc cref="Collect(ref AnalysisContext)" path="/returns"/></returns>
	private Step? CheckType3(scoped ref AnalysisContext context)
	{
		return null;
	}

	/// <summary>
	/// Check for type 4.
	/// </summary>
	/// <param name="context"><inheritdoc cref="Collect(ref AnalysisContext)" path="/param[@name='context']"/></param>
	/// <returns><inheritdoc cref="Collect(ref AnalysisContext)" path="/returns"/></returns>
	private Step? CheckType4(scoped ref AnalysisContext context)
	{
		return null;
	}
}
