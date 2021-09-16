namespace Sudoku.Solving.Manual.Searchers.SingleDigitPatterns;

/// <summary>
/// Provides with a <b>Empty Rectangle</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Empty Rectangle</item>
/// </list>
/// </summary>
[StepSearcher<EmptyRectangleStepSearcher>]
public sealed class EmptyRectangleStepSearcher : IEmptyRectangleStepSearcher
{
	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(12, DisplayingLevel.B);


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
					if (blockMask != 0 && (blockMask & blockMask - 1) == 0
						|| i < 6 && (linkMap & RegionMaps[column]).IsEmpty
						|| i >= 6 && (linkMap & RegionMaps[row]).IsEmpty)
					{
						continue;
					}

					int[] t = (linkMap - (i < 6 ? RegionMaps[column] : RegionMaps[row])).ToArray();
					int elimRegion = i < 6 ? t[0] % 9 + 18 : t[0] / 9 + 9;
					var elimCellMap = i < 6
						? CandMaps[digit] & RegionMaps[elimRegion] & RegionMaps[row]
						: CandMaps[digit] & RegionMaps[elimRegion] & RegionMaps[column];

					if (elimCellMap.IsEmpty)
					{
						continue;
					}

					int elimCell = elimCellMap[0];
					if (grid.Exists(elimCell, digit) is not true)
					{
						continue;
					}

					// Gather all highlight candidates.
					var candidateOffsets = new List<(int, ColorIdentifier)>();
					var cpCells = new List<int>(2);
					foreach (int cell in RegionMaps[block] & CandMaps[digit])
					{
						candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)1));
					}
					foreach (int cell in linkMap)
					{
						candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)0));
						cpCells.Add(cell);
					}

					var step = new EmptyRectangleStep(
						ImmutableArray.Create(new Conclusion(ConclusionType.Elimination, elimCell, digit)),
						ImmutableArray.Create(new PresentationData
						{
							Candidates = candidateOffsets,
							Regions = new[] { (block, (ColorIdentifier)0) }
						}),
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
