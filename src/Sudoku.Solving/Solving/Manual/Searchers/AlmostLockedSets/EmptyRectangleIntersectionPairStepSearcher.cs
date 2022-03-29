namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with an <b>Empty Rectangle Intersection Pair</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Empty Rectangle Intersection Pair</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe partial class EmptyRectangleIntersectionPairStepSearcher :
	IEmptyRectangleIntersectionPairStepSearcher
{
	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		for (int i = 0, length = BivalueMap.Count, iterationLength = length - 1; i < iterationLength; i++)
		{
			int c1 = BivalueMap[i];

			short mask = grid.GetCandidates(c1);
			int d1 = TrailingZeroCount(mask), d2 = mask.GetNextSet(d1);
			for (int j = i + 1; j < length; j++)
			{
				int c2 = BivalueMap[j];

				// Check the candidates that cell holds is totally same with 'c1'.
				if (grid.GetCandidates(c2) != mask)
				{
					continue;
				}

				// Check the two cells are not in same region.
				if ((Cells.Empty + c1 + c2).InOneRegion)
				{
					continue;
				}

				int block1 = c1.ToRegionIndex(Region.Block);
				int block2 = c2.ToRegionIndex(Region.Block);
				if (block1 % 3 == block2 % 3 || block1 / 3 == block2 / 3)
				{
					continue;
				}

				// Check the block that two cells both see.
				var interMap = !(Cells.Empty + c1 + c2);
				var unionMap = new Cells(c1) | new Cells(c2);
				foreach (int interCell in interMap)
				{
					int block = interCell.ToRegionIndex(Region.Block);
					var regionMap = RegionMaps[block];
					var checkingMap = regionMap - unionMap & regionMap;
					if ((checkingMap & CandMaps[d1]) is not [] || (checkingMap & CandMaps[d2]) is not [])
					{
						continue;
					}

					// Check whether two digits are both in the same empty rectangle.
					int b1 = c1.ToRegionIndex(Region.Block);
					int b2 = c2.ToRegionIndex(Region.Block);
					var erMap = (unionMap & RegionMaps[b1] - interMap) | (unionMap & RegionMaps[b2] - interMap);
					var erCellsMap = regionMap & erMap;
					short m = grid.GetDigitsUnion(erCellsMap);
					if ((m & mask) != mask)
					{
						continue;
					}

					// Check eliminations.
					var conclusions = new List<Conclusion>();
					int z = (interMap & regionMap)[0];
					var c1Map = RegionMaps[(Cells.Empty + z + c1).CoveredLine];
					var c2Map = RegionMaps[(Cells.Empty + z + c2).CoveredLine];
					foreach (int elimCell in (c1Map | c2Map) - c1 - c2 - erMap)
					{
						if (grid.Exists(elimCell, d1) is true)
						{
							conclusions.Add(new(ConclusionType.Elimination, elimCell, d1));
						}
						if (grid.Exists(elimCell, d2) is true)
						{
							conclusions.Add(new(ConclusionType.Elimination, elimCell, d2));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<CandidateViewNode>();
					foreach (int digit in grid.GetCandidates(c1))
					{
						candidateOffsets.Add(new(0, c1 * 9 + digit));
					}
					foreach (int digit in grid.GetCandidates(c2))
					{
						candidateOffsets.Add(new(0, c2 * 9 + digit));
					}
					foreach (int cell in erCellsMap)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							if (digit != d1 && digit != d2)
							{
								continue;
							}

							candidateOffsets.Add(new(1, cell * 9 + digit));
						}
					}

					var step = new EmptyRectangleIntersectionPairStep(
						ImmutableArray.CreateRange(conclusions),
						ImmutableArray.Create(
							View.Empty
								+ candidateOffsets
								+ new RegionViewNode(0, block)
						),
						c1,
						c2,
						block,
						d1,
						d2
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
