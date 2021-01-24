using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Models;
using Sudoku.Solving.Manual.Extensions;
using Sudoku.Techniques;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Encapsulates an <b>empty rectangle intersection pair</b> (ERIP) technique.
	/// </summary>
	public sealed class EripStepSearcher : AlsStepSearcher
	{
		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(28, nameof(Technique.Erip))
		{
			DisplayLevel = 2
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			for (int i = 0, length = BivalueMap.Count; i < length - 1; i++)
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

					int block1 = c1.ToRegion(RegionLabel.Block), block2 = c2.ToRegion(RegionLabel.Block);
					if (block1 % 3 == block2 % 3 || block1 / 3 == block2 / 3)
					{
						continue;
					}

					// Check the block that two cells both see.
					var interMap = new Cells { c1, c2 }.PeerIntersection;
					var unionMap = new Cells(c1) | new Cells(c2);
					foreach (int interCell in interMap)
					{
						int block = interCell.ToRegion(RegionLabel.Block);
						var regionMap = RegionMaps[block];
						var checkingMap = regionMap - unionMap & regionMap;
						if (!(checkingMap & CandMaps[d1]).IsEmpty || !(checkingMap & CandMaps[d2]).IsEmpty)
						{
							continue;
						}

						// Check whether two digits are both in the same empty rectangle.
						int[] offsets = interMap.ToArray();
						int inter1 = offsets[0], inter2 = offsets[1];
						int b1 = inter1.ToRegion(RegionLabel.Block);
						int b2 = inter2.ToRegion(RegionLabel.Block);
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

						var candidateOffsets = new List<DrawingInfo>();
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

						accumulator.Add(
							new EripStepInfo(
								conclusions,
								new View[]
								{
									new()
									{
										Candidates = candidateOffsets,
										Regions = new DrawingInfo[] { new(0, block) }
									}
								},
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
