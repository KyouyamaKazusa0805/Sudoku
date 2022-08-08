namespace Sudoku.Solving.Manual.Searchers;

[StepSearcher]
internal sealed unsafe partial class EmptyRectangleStepSearcher : IEmptyRectangleStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		for (int digit = 0; digit < 9; digit++)
		{
			for (int block = 0; block < 9; block++)
			{
				// Check the empty rectangle occupies more than 2 cells.
				// and the structure forms an empty rectangle.
				var erMap = CandidatesMap[digit] & HouseMaps[block];
				if (erMap.Count < 2
					|| !IEmptyRectangleStepSearcher.IsEmptyRectangle(erMap, block, out int row, out int column))
				{
					continue;
				}

				// Search for conjugate pair.
				for (int i = 0; i < 12; i++)
				{
					var linkMap = CandidatesMap[digit] & HouseMaps[EmptyRectangleLinkIds[block, i]];
					if (linkMap.Count != 2)
					{
						continue;
					}

					short blockMask = linkMap.BlockMask;
					if (IsPow2(blockMask)
						|| i < 6 && (linkMap & HouseMaps[column]) is []
						|| i >= 6 && (linkMap & HouseMaps[row]) is [])
					{
						continue;
					}

					int t = (linkMap - HouseMaps[i < 6 ? column : row])[0];
					int elimHouse = i < 6 ? t % 9 + 18 : t / 9 + 9;
					var elimCellMap = CandidatesMap[digit] & HouseMaps[elimHouse] & HouseMaps[i < 6 ? row : column];
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
					foreach (int cell in HouseMaps[block] & CandidatesMap[digit])
					{
						candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
					}
					foreach (int cell in linkMap)
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
						cpCells.Add(cell);
					}

					var step = new EmptyRectangleStep(
						ImmutableArray.Create(new Conclusion(ConclusionType.Elimination, elimCell, digit)),
						ImmutableArray.Create(
							View.Empty
								| candidateOffsets
								| new HouseViewNode(DisplayColorKind.Normal, block)
						),
						digit,
						block,
						new(cpCells[0], cpCells[1], digit)
					);

					if (onlyFindOne)
					{
						return step;
					}

					accumulator.Add(step);
				}
			}
		}

		return null;
	}
}
