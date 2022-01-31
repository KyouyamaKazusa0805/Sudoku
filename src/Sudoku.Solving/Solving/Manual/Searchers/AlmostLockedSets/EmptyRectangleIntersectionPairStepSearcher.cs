using Sudoku.Collections;
using Sudoku.Data;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Steps;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.Buffer.FastProperties;

namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with an <b>Empty Rectangle Intersection Pair</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Empty Rectangle Intersection Pair</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe class EmptyRectangleIntersectionPairStepSearcher : IEmptyRectangleIntersectionPairStepSearcher
{
	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(26, DisplayingLevel.B);


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
				if (new Cells { c1, c2 }.InOneRegion)
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
				Cells interMap = new Cells { c1, c2 }.PeerIntersection, unionMap = new Cells(c1) | new Cells(c2);
				foreach (int interCell in interMap)
				{
					int block = interCell.ToRegionIndex(Region.Block);
					Cells regionMap = RegionMaps[block], checkingMap = regionMap - unionMap & regionMap;
					if (!(checkingMap & CandMaps[d1]).IsEmpty || !(checkingMap & CandMaps[d2]).IsEmpty)
					{
						continue;
					}

					// Check whether two digits are both in the same empty rectangle.
					int inter1 = interMap[0], inter2 = interMap[1];
					int b1 = inter1.ToRegionIndex(Region.Block);
					int b2 = inter2.ToRegionIndex(Region.Block);
					var erMap = (unionMap & RegionMaps[b1] - interMap) | (unionMap & RegionMaps[b2] - interMap);
					var erCellsMap = regionMap & erMap;
					short m = 0;
					foreach (int cell in erCellsMap)
					{
						m |= grid.GetCandidates(cell);
					}
					if ((m & mask) != mask)
					{
						continue;
					}

					// Check eliminations.
					var conclusions = new List<Conclusion>();
					int z = (interMap & regionMap)[0];
					var c1Map = RegionMaps[new Cells { z, c1 }.CoveredLine];
					var c2Map = RegionMaps[new Cells { z, c2 }.CoveredLine];
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

					var candidateOffsets = new List<(int, ColorIdentifier)>();
					foreach (int digit in grid.GetCandidates(c1))
					{
						candidateOffsets.Add((c1 * 9 + digit, (ColorIdentifier)0));
					}
					foreach (int digit in grid.GetCandidates(c2))
					{
						candidateOffsets.Add((c2 * 9 + digit, (ColorIdentifier)0));
					}
					foreach (int cell in erCellsMap)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							if (digit != d1 && digit != d2)
							{
								continue;
							}

							candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)1));
						}
					}

					var step = new EmptyRectangleIntersectionPairStep(
						ImmutableArray.CreateRange(conclusions),
						ImmutableArray.Create(new PresentationData
						{
							Candidates = candidateOffsets,
							Regions = new[] { (block, (ColorIdentifier)0) }
						}),
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
