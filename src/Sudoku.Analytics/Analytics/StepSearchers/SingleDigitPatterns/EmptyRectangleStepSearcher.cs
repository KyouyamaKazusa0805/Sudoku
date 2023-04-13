namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with an <b>Empty Rectangle</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Empty Rectangle</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed partial class EmptyRectangleStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? GetAll(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		for (var digit = 0; digit < 9; digit++)
		{
			for (var block = 0; block < 9; block++)
			{
				// Check the empty rectangle occupies more than 2 cells.
				// and the structure forms an empty rectangle.
				var erMap = CandidatesMap[digit] & HousesMap[block];
				if (erMap.Count < 2 || !EmptyRectangleHelper.IsEmptyRectangle(erMap, block, out var row, out var column))
				{
					continue;
				}

				// Search for conjugate pair.
				for (var i = 0; i < 12; i++)
				{
					var linkMap = CandidatesMap[digit] & HousesMap[EmptyRectangleLinkIds[block, i]];
					if (linkMap.Count != 2)
					{
						continue;
					}

					var blockMask = linkMap.BlockMask;
					if (IsPow2(blockMask) || i < 6 && !(linkMap & HousesMap[column]) || i >= 6 && !(linkMap & HousesMap[row]))
					{
						continue;
					}

					var t = (linkMap - HousesMap[i < 6 ? column : row])[0];
					var elimHouse = i < 6 ? t % 9 + 18 : t / 9 + 9;
					if ((CandidatesMap[digit] & HousesMap[elimHouse] & HousesMap[i < 6 ? row : column]) is not [var elimCell, ..])
					{
						continue;
					}

					if (!CandidatesMap[digit].Contains(elimCell))
					{
						continue;
					}

					// Gather all highlight candidates.
					var candidateOffsets = new List<CandidateViewNode>();
					using scoped var cpCells = new ValueList<int>(2);
					foreach (var cell in HousesMap[block] & CandidatesMap[digit])
					{
						candidateOffsets.Add(new(WellKnownColorIdentifierKind.Auxiliary1, cell * 9 + digit));
					}
					foreach (var cell in linkMap)
					{
						candidateOffsets.Add(new(WellKnownColorIdentifierKind.Normal, cell * 9 + digit));
						cpCells.Add(cell);
					}

					var step = new EmptyRectangleStep(
						new[] { new Conclusion(Elimination, elimCell, digit) },
						new[] { View.Empty | candidateOffsets | new HouseViewNode(WellKnownColorIdentifierKind.Normal, block) },
						digit,
						block,
						new(cpCells[0], cpCells[1], digit)
					);

					if (context.OnlyFindOne)
					{
						return step;
					}

					context.Accumulator.Add(step);
				}
			}
		}

		return null;
	}
}
