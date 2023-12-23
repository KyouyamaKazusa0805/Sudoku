namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Uniqueness Clue Cover</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Uniqueness Clue Cover</item>
/// </list>
/// </summary>
[StepSearcher(Technique.UniquenessClueCover)]
[StepSearcherRuntimeName("StepSearcherName_UniquenessClueCoverStepSearcher")]
public sealed partial class UniquenessClueCoverStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="/g/developer-notes" />
	/// The technique is very complex that I cannot enumerate all possible cases.
	/// See <see href="http://sudopedia.enjoysudoku.com/Uniqueness_Clue_Cover.html">this link</see> to learn about this technique.
	/// </remarks>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		foreach (var (chuteIndex, chute, isRow, _) in Chutes)
		{
			if (chute - EmptyCells is not [var c1, var c2] valueCells)
			{
				// The number of the value cells must be 2 for this type.
				continue;
			}

			if (PeersMap[c1].Contains(c2) || PeersMap[c2].Contains(c1))
			{
				// The two value cells cannot lie in a same house.
				continue;
			}

			var (c1Digit, c2Digit) = (grid.GetDigit(c1), grid.GetDigit(c2));
			if (c1Digit == c2Digit)
			{
				// The two value cells cannot be filled with a same digit.
				continue;
			}

			var digitsMask = (Mask)(1 << c1Digit | 1 << c2Digit);
			var elimHouseType = isRow ? HouseType.Column : HouseType.Row;
			var excludedHouseType = isRow ? HouseType.Row : HouseType.Column;
			var excludedLines = HousesMap[c1.ToHouseIndex(excludedHouseType)] | HousesMap[c2.ToHouseIndex(excludedHouseType)];
			var conclusions = new List<Conclusion>(2);
			foreach (var elimCell in chute - excludedLines & (HousesMap[c1.ToHouseIndex(elimHouseType)] | HousesMap[c2.ToHouseIndex(elimHouseType)]))
			{
				var correspondingValueCell = (HousesMap[elimCell.ToHouseIndex(elimHouseType)] & chute & valueCells)[0];
				var elimDigit = TrailingZeroCount((Mask)(digitsMask & ~(1 << grid.GetDigit(correspondingValueCell))));
				if (CandidatesMap[elimDigit].Contains(elimCell))
				{
					conclusions.Add(new(Assignment, elimCell, elimDigit));
				}
			}
			if (conclusions.Count == 0)
			{
				// No eliminations can be found.
				continue;
			}

			var step = new UniquenessClueCoverStep(
				[.. conclusions],
				[
					[
						new ChuteViewNode(WellKnownColorIdentifier.Normal, chuteIndex),
						new CellViewNode(WellKnownColorIdentifier.Normal, c1),
						new CellViewNode(WellKnownColorIdentifier.Normal, c2)
					]
				],
				context.PredefinedOptions,
				CellsMap[c1] + c2,
				(Mask)(1 << c1Digit | 1 << c2Digit),
				chuteIndex
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
