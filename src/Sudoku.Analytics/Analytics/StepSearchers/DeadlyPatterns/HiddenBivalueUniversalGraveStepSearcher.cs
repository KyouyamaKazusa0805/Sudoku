namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Hidden Bi-value Universal Grave</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Hidden Bi-value Universal Grave Type 1</item>
/// <item>Hidden Bi-value Universal Grave Type 2</item>
/// <item>Hidden Bi-value Universal Grave Type 3</item>
/// <item>Hidden Bi-value Universal Grave Type 4</item>
/// </list>
/// </summary>
[StepSearcher(
	"StepSearcherName_HiddenBivalueUniversalGraveStepSearcher",
	Technique.HiddenBivalueUniversalGraveType1, Technique.HiddenBivalueUniversalGraveType2,
	Technique.HiddenBivalueUniversalGraveType3, Technique.HiddenBivalueUniversalGraveType4,
	SupportedSudokuTypes = SudokuType.Standard,
	SupportAnalyzingMultipleSolutionsPuzzle = false)]
public sealed partial class HiddenBivalueUniversalGraveStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(ref StepAnalysisContext context)
	{
		// Test examples:
		// +3+1+5+9+64+782+279+38+1+54+6.6.7+25.9...........312......8.43...7...6....5.4.8...7...3...9..:541 243 643 545 147 247 148 548 149 349 268 668 273 976 177 277 377 681 286 187 387 591 595 199

		ref readonly var grid = ref context.Grid;

		// Collect for number of givens. If the number of a digit is greater than 7, it cannot be used as target value.
		var mask = (Mask)0;
		for (var digit = 0; digit < 9; digit++)
		{
			if (ValuesMap[digit].Count <= 7)
			{
				mask |= (Mask)(1 << digit);
			}
		}
		if (Mask.PopCount(mask) < 2)
		{
			return null;
		}

		var availableDigits = mask.GetAllSets();

		// Iterate on 2-4 digits of values.
		var singlePositions = new Dictionary<Cell, Mask>(81);
		for (var size = 2; size <= 4; size++)
		{
			// Iterate on each combination of digits of number 'size'.
			foreach (var combination in availableDigits.GetSubsets(size))
			{
				// Merge maps of such digit candidate position maps into one.
				var mergedMap = CellMap.Empty;
				foreach (var digit in combination)
				{
					mergedMap |= CandidatesMap[digit];
				}

				// If one of the merged map contains 3 or more candidates in the current combination, the pattern will be invalid.
				var (digitsMask, patternIsInvalid) = (MaskOperations.Create(combination), false);
				singlePositions.Clear();
				foreach (var cell in mergedMap)
				{
					var currentDigitsMask = (Mask)(grid.GetCandidates(cell) & digitsMask);
					var numberOfDigits = Mask.PopCount(currentDigitsMask);
					if (numberOfDigits > 2)
					{
						patternIsInvalid = true;
						break;
					}
					if (numberOfDigits == 1)
					{
						singlePositions.Add(cell, currentDigitsMask);
					}
				}
				if (patternIsInvalid || singlePositions.Count == 0)
				{
					continue;
				}

				var cellOffsets = new List<CellViewNode>();
				var candidateOffsets = new List<CandidateViewNode>();
				foreach (var digit in combination)
				{
					foreach (var cell in ValuesMap[digit])
					{
						cellOffsets.Add(new(ColorIdentifier.Normal, cell));
					}

					candidateOffsets.AddRange(
						from cell in CandidatesMap[digit]
						select new CandidateViewNode(ColorIdentifier.Normal, cell * 9 + digit)
					);
				}

				var singlePositionDigitsMask = singlePositions.Values.Aggregate(static (interim, next) => (Mask)(interim | next));
				if (Mask.IsPow2(singlePositionDigitsMask))
				{
					// Type 1 or 2.
					var targetDigit = Mask.Log2(singlePositionDigitsMask);
					if (CheckType1Or2(
						ref context, in grid, targetDigit, [.. singlePositions.Keys],
						cellOffsets, candidateOffsets) is { } type1Or2Step)
					{
						return type1Or2Step;
					}
				}
				else
				{
					// Type 3 or 4.

				}
			}
		}

		return null;
	}

	/// <summary>
	/// Check type 1 and 2.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="grid">The grid to be used.</param>
	/// <param name="targetDigit">The target digit.</param>
	/// <param name="singlePositions">The single positions.</param>
	/// <param name="cellOffsets">The cell view nodes.</param>
	/// <param name="candidateOffsets">The candidate view nodes.</param>
	/// <returns>The found step.</returns>
	private static HiddenBivalueUniversalGraveStep? CheckType1Or2(
		ref StepAnalysisContext context,
		ref readonly Grid grid,
		Digit targetDigit,
		ref readonly CellMap singlePositions,
		List<CellViewNode> cellOffsets,
		List<CandidateViewNode> candidateOffsets
	)
	{
		// Congratulations, you have found a deadly pattern with 8 cells without any names!
		// Now check eliminations.
		var extraCells = CandidatesMap[targetDigit] & singlePositions;
		var eliminations = extraCells is [var extraCell]
			?
			from digit in (Mask)(grid.GetCandidates(extraCell) & ~(1 << targetDigit))
			select new Conclusion(Elimination, extraCell, digit)
			: from cell in extraCells % CandidatesMap[targetDigit] select new Conclusion(Elimination, cell, targetDigit);
		if (eliminations.Length == 0)
		{
			return null;
		}

		// Delete view nodes overlapping with conclusions.
		var nodesToRemove = new List<CandidateViewNode>();
		foreach (var node in candidateOffsets)
		{
			if (extraCells.Contains(node.Cell))
			{
				nodesToRemove.Add(node);
			}
		}
		foreach (var node in nodesToRemove)
		{
			candidateOffsets.Remove(node);
		}

		if (extraCells.Count == 1)
		{
			var step = new HiddenBivalueUniversalGraveType1Step(
				eliminations.ToArray(),
				[[.. cellOffsets, .. candidateOffsets]],
				context.Options
			);
			if (context.OnlyFindOne)
			{
				return step;
			}
			context.Accumulator.Add(step);
		}
		else
		{
			foreach (var cell in extraCells)
			{
				candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, cell * 9 + targetDigit));
			}

			var step = new HiddenBivalueUniversalGraveType2Step(
				eliminations.ToArray(),
				[[.. cellOffsets, .. candidateOffsets]],
				context.Options,
				targetDigit,
				in singlePositions
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
