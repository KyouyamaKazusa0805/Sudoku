namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with an <b>Law of Leftover</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Law of leftover</item>
/// </list>
/// </summary>
[StepSearcher(Technique.LawOfLeftover)]
[StepSearcherFlags(ConditionalFlags.DirectTechniquesOnly)]
[StepSearcherRuntimeName("StepSearcherName_LawOfLeftoverStepSearcher")]
public sealed partial class LawOfLeftoverStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		foreach (var ((line, block), (a, b, c, _)) in IntersectionMaps)
		{
			// Try to check for value cells from two different sets.
			var (lineSetDigitsMask, blockSetDigitsMask) = ((Mask)0, (Mask)0);
			var mergedCells = CellMap.Empty;
			foreach (var cell in a - EmptyCells)
			{
				lineSetDigitsMask |= (Mask)(1 << grid.GetDigit(cell));
				mergedCells.Add(cell);
			}
			foreach (var cell in b - EmptyCells)
			{
				blockSetDigitsMask |= (Mask)(1 << grid.GetDigit(cell));
				mergedCells.Add(cell);
			}

			// The LoL technique requires value digits from two different sets should be merged into a big set
			// that contain 6 digits.
			// Check whether merged mask contain 6 on bits.
			var mergedDigitsMask = (Mask)(lineSetDigitsMask | blockSetDigitsMask);
			if (PopCount((uint)mergedDigitsMask) != 6 || mergedCells.Count != 6)
			{
				continue;
			}

			// Check whether both two sides don't hold all 6 digits.
			if ((a - EmptyCells).Count == 6 || (b - EmptyCells).Count == 6)
			{
				continue;
			}

			// A LoL is found. Now check for eliminations.
			var conclusions = new List<Conclusion>();
			foreach (var (houseCells, digitsMaskTheOtherSide) in ((a & EmptyCells, blockSetDigitsMask), (b & EmptyCells, lineSetDigitsMask)))
			{
				switch (houseCells)
				{
					case [var targetCell]:
					{
						// The cell can be filled with the digit from the other side.
						var disapperedDigit = TrailingZeroCount((Mask)(mergedDigitsMask & digitsMaskTheOtherSide));
						conclusions.Add(new(Assignment, targetCell, disapperedDigit));

						break;
					}
					default:
					{
						var disappearedDigitsMask = (Mask)(mergedDigitsMask & digitsMaskTheOtherSide);
						foreach (var cell in houseCells)
						{
							foreach (var digit in (Mask)(grid.GetCandidates(cell) & ~disappearedDigitsMask))
							{
								conclusions.Add(new(Elimination, cell, digit));
							}
						}

						break;
					}
				}
			}
			if (conclusions.Count == 0)
			{
				continue;
			}

			var step = new LawOfLeftoverStep(
				[.. conclusions],
				[
					[
						..
						from cell in a
						select new CellViewNode(WellKnownColorIdentifier.Normal, cell) { RenderingMode = BothDirectAndPencilmark },
						..
						from cell in b
						select new CellViewNode(WellKnownColorIdentifier.Auxiliary2, cell) { RenderingMode = BothDirectAndPencilmark },
						..
						from cell in c
						select new CellViewNode(WellKnownColorIdentifier.Auxiliary3, cell) { RenderingMode = BothDirectAndPencilmark }
					]
				],
				context.PredefinedOptions,
				line,
				block,
				in a,
				in b
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
