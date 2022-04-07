namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with an <b>Empty Rectangle</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Empty Rectangle</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe partial class EmptyRectangleStepSearcher : IEmptyRectangleStepSearcher
{
	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		for (int digit = 0; digit < 9; digit++)
		{
			for (int block = 0; block < 9; block++)
			{
				// Check the empty rectangle occupies more than 2 cells.
				// and the structure forms an empty rectangle.
				var erMap = CandMaps[digit] & RegionMaps[block];
				if (erMap.Count < 2
					|| !IEmptyRectangleStepSearcher.IsEmptyRectangle(erMap, block, out int row, out int column))
				{
					continue;
				}

				// Search for conjugate pair.
				for (int i = 0; i < 12; i++)
				{
					var linkMap = CandMaps[digit] & RegionMaps[LinkIds[block, i]];
					if (linkMap.Count != 2)
					{
						continue;
					}

					short blockMask = linkMap.BlockMask;
					if (IsPow2(blockMask)
						|| i < 6 && (linkMap & RegionMaps[column]) is []
						|| i >= 6 && (linkMap & RegionMaps[row]) is [])
					{
						continue;
					}

					int t = (linkMap - RegionMaps[i < 6 ? column : row])[0];
					int elimRegion = i < 6 ? t % 9 + 18 : t / 9 + 9;
					var elimCellMap = CandMaps[digit] & RegionMaps[elimRegion] & RegionMaps[i < 6 ? row : column];
					if (elimCellMap is not [var elimCell, ..])
					{
						continue;
					}

					if (grid.Exists(elimCell, digit) is not true)
					{
						continue;
					}

					// Gather all highlight candidates.
					var candidateOffsets = new List<CandidateViewNode>();
					var cpCells = new List<int>(2);
					foreach (int cell in RegionMaps[block] & CandMaps[digit])
					{
						candidateOffsets.Add(new(1, cell * 9 + digit));
					}
					foreach (int cell in linkMap)
					{
						candidateOffsets.Add(new(0, cell * 9 + digit));
						cpCells.Add(cell);
					}

					var step = new EmptyRectangleStep(
						ImmutableArray.Create(new Conclusion(ConclusionType.Elimination, elimCell, digit)),
						ImmutableArray.Create(View.Empty + candidateOffsets + new RegionViewNode(0, block)),
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


	/// <summary>
	/// Indicates all regions iterating on the specified block
	/// forming an empty rectangle.
	/// </summary>
	private static readonly int[,] LinkIds =
	{
		{ 12, 13, 14, 15, 16, 17, 21, 22, 23, 24, 25, 26 },
		{ 12, 13, 14, 15, 16, 17, 18, 19, 20, 24, 25, 26 },
		{ 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 },
		{ 9, 10, 11, 15, 16, 17, 21, 22, 23, 24, 25, 26 },
		{ 9, 10, 11, 15, 16, 17, 18, 19, 20, 24, 25, 26 },
		{ 9, 10, 11, 15, 16, 17, 18, 19, 20, 21, 22, 23 },
		{ 9, 10, 11, 12, 13, 14, 21, 22, 23, 24, 25, 26 },
		{ 9, 10, 11, 12, 13, 14, 18, 19, 20, 24, 25, 26 },
		{ 9, 10, 11, 12, 13, 14, 18, 19, 20, 21, 22, 23 }
	};
}
