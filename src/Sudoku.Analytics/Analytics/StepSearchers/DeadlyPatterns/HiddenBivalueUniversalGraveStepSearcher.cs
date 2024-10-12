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
		ref readonly var grid = ref context.Grid;
		var candidatesResetGrid = grid.ResetCandidatesGrid;
		var resetCandidatesMap = candidatesResetGrid.CandidatesMap;

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
		for (var size = 2; size <= Math.Min(4, (int)Mask.PopCount(mask)); size++)
		{
			// Iterate on each combination of digits of number 'size'.
			foreach (var combination in availableDigits.GetSubsets(size))
			{
				// Merge maps of such digit candidate position maps into one.
				var mergedMap = CellMap.Empty;
				foreach (var digit in combination)
				{
					mergedMap |= resetCandidatesMap[digit];
				}

				// If one of the merged map contains 3 or more candidates in the current combination, the pattern will be invalid.
				var (digitsMask, patternIsInvalid) = (MaskOperations.Create(combination), false);
				singlePositions.Clear();
				foreach (var cell in mergedMap)
				{
					var currentDigitsMask = (Mask)(candidatesResetGrid.GetCandidates(cell) & digitsMask);
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
						ref context, in candidatesResetGrid, targetDigit, [.. singlePositions.Keys],
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
		// Test examples:
		// Type 1
		// 1+73+5849+62+6...3+2+7...+2..+6+75.+386+42.+1+3+7..+3+7+4.+6+1+2+82+1.8734.+6+39+67182..7+82+3+4+5+6....+1+62+983+7
		// +2+4+8+5.+1..+3.+5.+3.+8+412+1.3.+24.+8+5..+2+1.5+8+34+31..+8+2+567+58..3.+1+2+9+4+3.81.+2+5+6+8+25.+4.3.+1.6+12+5+3.+4+8
		// .+1+7+5+2+3..+6+6+5..+71.+32.+23.4+6+75+1+3+6.+4+1+9+2+7...+1+25+736.27.+6+38.+1.+7.63.+5+1+2.1.+2+7+64+5.+35+3.+1.+2+6.7
		// .+1+2.+4+5.+6+3+5..+3.1.+42.34+2+6.+15....+13.+5+2+4+3+4+15+2...62+5.+6.+4+3.1+6+25..+34+1+8+47+98+1+2+63+51+8+3+4+56+2..
		// .+8+7.+625...+215+8...+7+56+91.7.8.+832+7+94...1+9+4+6+2+57+3+8+7+5+6+3+189+24+2+1+8.7....6+752..+8..94+3+8+5..7.
		//
		// Type 2
		// +3+1+5+9+64+782+279+38+1+54+6.6.7+25.9...........312......8.43...7...6....5.4.8...7...3...9..
		// ..+9..+2.+4.+4.+8..+9.12.+23..4..5..+4+2+8367+9+38..+9+1+2+5+429.+5+4.+1+8+3.+3+1+42...8+8+4+291..+3.+9.5..+84+2+1
		// ....+2+1.+4+3+4......12+1+23.45.+8....+5+1+83+64+31+82+6+4..+5+56+47+93+1+2+8.+3+1+48.+2+5+6..5...4.+16+4.1+5.+8+3.
		// ..+6.+2+1.+5+3.+5.+3.+6.12+1+23.45....+6+1..3+2+4+5+2..1+5.+3+76+375+4+6+28+9+1.+3.+6+175+2.+51+72+3....6.+2+5..+13+7
		// .+6+7..+1.+5+4+8+5+4.+6+9.12+1.3.45....+4.+5.3+1.+7+51.6.+7.+4+37+3..+1+45..+3..17+6+48+5+6+7+14+5+8+2+3+9+485..+26+7+1

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
		foreach (var cell in extraCells)
		{
			candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, cell * 9 + targetDigit));
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
