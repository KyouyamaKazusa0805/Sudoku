using System.Collections.Generic;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Constants.RegionLabel;
using static Sudoku.Data.ConclusionType;
using static Sudoku.Data.GridMap.InitializeOption;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Encapsulates an <b>empty rectangle intersection pair</b> technique.
	/// </summary>
	[TechniqueDisplay("Empty Rectangle Intersection Pair")]
	public sealed class ErIntersectionPairTechniqueSearcher : AlsTechniqueSearcher
	{
		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 60;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			int[] bivalueCells = BivalueMap.ToArray();
			for (int i = 0, length = bivalueCells.Length; i < length - 1; i++)
			{
				int c1 = bivalueCells[i];

				short mask = grid.GetCandidatesReversal(c1);
				int d1 = mask.FindFirstSet(), d2 = mask.GetNextSet(d1);
				for (int j = i + 1; j < length; j++)
				{
					int c2 = bivalueCells[j];

					// Check the candidates that cell holds is totally same with 'c1'.
					if (grid.GetCandidatesReversal(c2) != mask)
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
					var intersectionMap = new GridMap(stackalloc[] { c1, c2 }, ProcessPeersWithoutItself);
					var unionMap = new GridMap(c1) | new GridMap(c2);
					foreach (int interCell in intersectionMap)
					{
						int block = GetRegion(interCell, Block);
						var regionMap = RegionMaps[block];
						var checkingMap = regionMap - unionMap & regionMap;
						if (checkingMap.Overlaps(CandMaps[d1]) || checkingMap.Overlaps(CandMaps[d2]))
						{
							continue;
						}

						// Check whether two digits are both in the same empty rectangle.
						int inter1 = intersectionMap.SetAt(0);
						int inter2 = intersectionMap.SetAt(1);
						int b1 = GetRegion(inter1, Block);
						int b2 = GetRegion(inter2, Block);
						var erMap =
							(unionMap & RegionMaps[b1] - intersectionMap) | (unionMap & RegionMaps[b2] - intersectionMap);
						var erCellsMap = regionMap & erMap;
						short m = 0;
						foreach (int cell in erCellsMap)
						{
							m |= grid.GetCandidatesReversal(cell);
						}
						if ((m & mask) != mask)
						{
							continue;
						}

						// Check eliminations.
						var conclusions = new List<Conclusion>();
						int z = (intersectionMap & regionMap).SetAt(0);
						var c1Map = RegionMaps[new GridMap { z, c1 }.CoveredLine];
						var c2Map = RegionMaps[new GridMap { z, c2 }.CoveredLine];
						foreach (int elimCell in new GridMap(c1Map | c2Map) { [c1] = false, [c2] = false } - erMap)
						{
							if (grid.Exists(elimCell, d1) is true)
							{
								conclusions.Add(new Conclusion(Elimination, elimCell, d1));
							}
							if (grid.Exists(elimCell, d2) is true)
							{
								conclusions.Add(new Conclusion(Elimination, elimCell, d2));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<(int, int)>();
						foreach (int digit in grid.GetCandidatesReversal(c1).GetAllSets())
						{
							candidateOffsets.Add((0, c1 * 9 + digit));
						}
						foreach (int digit in grid.GetCandidatesReversal(c2).GetAllSets())
						{
							candidateOffsets.Add((0, c2 * 9 + digit));
						}
						foreach (int cell in erCellsMap)
						{
							foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
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
									new View(
										cellOffsets: new[] { (0, c1), (0, c2) },
										candidateOffsets,
										regionOffsets: new[] { (0, block) },
										links: null)
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
