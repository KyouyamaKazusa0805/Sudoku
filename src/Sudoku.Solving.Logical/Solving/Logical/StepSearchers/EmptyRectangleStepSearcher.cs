namespace Sudoku.Solving.Logical.StepSearchers;

[StepSearcher]
internal sealed unsafe partial class EmptyRectangleStepSearcher : IEmptyRectangleStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(scoped in LogicalAnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		for (var digit = 0; digit < 9; digit++)
		{
			for (var block = 0; block < 9; block++)
			{
				// Check the empty rectangle occupies more than 2 cells.
				// and the structure forms an empty rectangle.
				var erMap = CandidatesMap[digit] & HousesMap[block];
				if (erMap.Count < 2
					|| !IEmptyRectangleStepSearcher.IsEmptyRectangle(erMap, block, out var row, out var column))
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
					if (IsPow2(blockMask)
						|| i < 6 && (linkMap & HousesMap[column]) is []
						|| i >= 6 && (linkMap & HousesMap[row]) is [])
					{
						continue;
					}

					var t = (linkMap - HousesMap[i < 6 ? column : row])[0];
					var elimHouse = i < 6 ? t % 9 + 18 : t / 9 + 9;
					var elimCellMap = CandidatesMap[digit] & HousesMap[elimHouse] & HousesMap[i < 6 ? row : column];
					if (elimCellMap is not [var elimCell, ..])
					{
						continue;
					}

					if (!CandidatesMap[digit].Contains(elimCell))
					{
						continue;
					}

					// Gather all highlight candidates.
					var candidateOffsets = new List<CandidateViewNode>();
					var cpCells = new List<int>(2);
					foreach (var cell in HousesMap[block] & CandidatesMap[digit])
					{
						candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
					}
					foreach (var cell in linkMap)
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
						cpCells.Add(cell);
					}

					var step = new EmptyRectangleStep(
						ImmutableArray.Create(new Conclusion(Elimination, elimCell, digit)),
						ImmutableArray.Create(
							View.Empty
								| candidateOffsets
								| new HouseViewNode(DisplayColorKind.Normal, block)
						),
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
