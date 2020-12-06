using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Encapsulates an <b>empty rectangle intersection pair</b> (ERIP) technique.
	/// </summary>
	public sealed class EripTechniqueSearcher : AlsTechniqueSearcher
	{
		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(60, nameof(TechniqueCode.Erip)) { DisplayLevel = 2 };


		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, in SudokuGrid grid)
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
					if (new GridMap { c1, c2 }.InOneRegion)
					{
						continue;
					}

					int block1 = GetRegion(c1, RegionLabel.Block), block2 = GetRegion(c2, RegionLabel.Block);
					if (block1 % 3 == block2 % 3 || block1 / 3 == block2 / 3)
					{
						continue;
					}

					// Check the block that two cells both see.
					var interMap = new GridMap { c1, c2 }.PeerIntersection;
					var unionMap = new GridMap(c1) | new GridMap(c2);
					foreach (int interCell in interMap)
					{
						int block = GetRegion(interCell, RegionLabel.Block);
						var regionMap = RegionMaps[block];
						var checkingMap = regionMap - unionMap & regionMap;
						if (checkingMap.Overlaps(CandMaps[d1]) || checkingMap.Overlaps(CandMaps[d2]))
						{
							continue;
						}

						// Check whether two digits are both in the same empty rectangle.
						int inter1 = interMap.First;
						int inter2 = interMap.SetAt(1);
						int b1 = GetRegion(inter1, RegionLabel.Block);
						int b2 = GetRegion(inter2, RegionLabel.Block);
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
						int z = (interMap & regionMap).First;
						var c1Map = RegionMaps[new GridMap { z, c1 }.CoveredLine];
						var c2Map = RegionMaps[new GridMap { z, c2 }.CoveredLine];
						foreach (int elimCell in new GridMap(c1Map | c2Map) { ~c1, ~c2 } - erMap)
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

						var candidateOffsets = new List<DrawingInfo>();
						foreach (int digit in grid.GetCandidateMask(c1))
						{
							candidateOffsets.Add(new(0, c1 * 9 + digit));
						}
						foreach (int digit in grid.GetCandidateMask(c2))
						{
							candidateOffsets.Add(new(0, c2 * 9 + digit));
						}
						foreach (int cell in erCellsMap)
						{
							foreach (int digit in grid.GetCandidateMask(cell))
							{
								if (digit != d1 && digit != d2)
								{
									continue;
								}

								candidateOffsets.Add(new(1, cell * 9 + digit));
							}
						}

						accumulator.Add(
							new EripTechniqueInfo(
								conclusions,
								new View[] { new(null, candidateOffsets, new DrawingInfo[] { new(0, block) }, null) },
								c1,
								c2,
								block,
								d1,
								d2));
					}
				}
			}
		}
	}
}
