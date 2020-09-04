using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Constants.RegionLabel;
using static Sudoku.Data.ConclusionType;
using static Sudoku.Data.GridMap.InitializationOption;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Encapsulates an <b>empty rectangle intersection pair</b> technique.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.Erip))]
	[SearcherProperty(60)]
	public sealed class ErIntersectionPairTechniqueSearcher : AlsTechniqueSearcher
	{
		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, Grid grid)
		{
			int[] bivalueCells = BivalueMap.ToArray();
			for (int i = 0, length = bivalueCells.Length; i < length - 1; i++)
			{
				int c1 = bivalueCells[i];

				short mask = grid.GetCandidateMask(c1);
				int d1 = mask.FindFirstSet(), d2 = mask.GetNextSet(d1);
				for (int j = i + 1; j < length; j++)
				{
					int c2 = bivalueCells[j];

					// Check the candidates that cell holds is totally same with 'c1'.
					if (grid.GetCandidateMask(c2) != mask)
					{
						continue;
					}

					// Check the two cells are not in same region.
					if (new GridMap { c1, c2 }.AllSetsAreInOneRegion(out _))
					{
						continue;
					}

					int block1 = GetRegion(c1, Block), block2 = GetRegion(c2, Block);
					if (block1 % 3 == block2 % 3 || block1 / 3 == block2 / 3)
					{
						continue;
					}

					// Check the block that two cells both see.
					var interMap = new GridMap(stackalloc[] { c1, c2 }, ProcessPeersWithoutItself);
					var unionMap = new GridMap(c1) | new GridMap(c2);
					foreach (int interCell in interMap)
					{
						int block = GetRegion(interCell, Block);
						var regionMap = RegionMaps[block];
						var checkingMap = regionMap - unionMap & regionMap;
						if (checkingMap.Overlaps(CandMaps[d1]) || checkingMap.Overlaps(CandMaps[d2]))
						{
							continue;
						}

						// Check whether two digits are both in the same empty rectangle.
						int inter1 = interMap.SetAt(0);
						int inter2 = interMap.SetAt(1);
						int b1 = GetRegion(inter1, Block);
						int b2 = GetRegion(inter2, Block);
						var erMap = (unionMap & RegionMaps[b1] - interMap) | (unionMap & RegionMaps[b2] - interMap);
						var erCellsMap = regionMap & erMap;
						short m = 0;
						foreach (int cell in erCellsMap)
						{
							m |= grid.GetCandidateMask(cell);
						}
						if ((m & mask) != mask)
						{
							continue;
						}

						// Check eliminations.
						var conclusions = new List<Conclusion>();
						int z = (interMap & regionMap).SetAt(0);
						var c1Map = RegionMaps[new GridMap { z, c1 }.CoveredLine];
						var c2Map = RegionMaps[new GridMap { z, c2 }.CoveredLine];
						foreach (int elimCell in new GridMap(c1Map | c2Map) { [c1] = false, [c2] = false } - erMap)
						{
							if (grid.Exists(elimCell, d1) is true)
							{
								conclusions.Add(new(Elimination, elimCell, d1));
							}
							if (grid.Exists(elimCell, d2) is true)
							{
								conclusions.Add(new(Elimination, elimCell, d2));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<(int, int)>();
						foreach (int digit in grid.GetCandidates(c1))
						{
							candidateOffsets.Add((0, c1 * 9 + digit));
						}
						foreach (int digit in grid.GetCandidates(c2))
						{
							candidateOffsets.Add((0, c2 * 9 + digit));
						}
						foreach (int cell in erCellsMap)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								if (digit != d1 && digit != d2)
								{
									continue;
								}

								candidateOffsets.Add((1, cell * 9 + digit));
							}
						}

						accumulator.Add(
							new ErIntersectionPairTechniqueInfo(
								conclusions,
								views: new[]
								{
									new View(new[] { (0, c1), (0, c2) }, candidateOffsets, new[] { (0, block) }, null)
								},
								startCell: c1,
								endCell: c2,
								region: block,
								digit1: d1,
								digit2: d2));
					}
				}
			}
		}
	}
}
