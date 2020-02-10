using System.Collections.Generic;
using System.Linq;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.SingleDigitPatterns
{
	/// <summary>
	/// Encapsulates a two strong links technique searcher.
	/// </summary>
	public sealed class TwoStrongLinksTechniqueSearcher : SingleDigitPatternTechniqueSearcher
	{
		/// <inheritdoc/>
		public override IReadOnlyList<TechniqueInfo> TakeAll(Grid grid)
		{
			var result = new List<TwoStrongLinksTechniqueInfo>();

			for (int digit = 0; digit < 9; digit++)
			{
				for (int r1 = 0; r1 < 26; r1++)
				{
					for (int r2 = r1 + 1; r2 < 27; r2++)
					{
						if (r1 / 9 == r2 / 9)
						{
							continue;
						}

						// Get masks.
						short mask1 = grid.GetDigitAppearingMask(digit, r1);
						short mask2 = grid.GetDigitAppearingMask(digit, r2);
						if (mask1.CountSet() != 2 || mask2.CountSet() != 2)
						{
							continue;
						}

						// Get all cells.
						var cells1 = new List<int>();
						var cells2 = new List<int>();
						foreach (int pos1 in mask1.GetAllSets())
						{
							cells1.Add(RegionUtils.GetCellOffset(r1, pos1));
						}
						foreach (int pos2 in mask2.GetAllSets())
						{
							cells2.Add(RegionUtils.GetCellOffset(r2, pos2));
						}

						if (cells1.Any(c => cells2.Contains(c)))
						{
							continue;
						}

						// Check two cells have a same region.
						int sameBlock = -1;
						int c1Index = default;
						int c2Index = default;
						int headIndex = default;
						int tailIndex = default;
						for (int i = 0; i < cells1.Count; i++)
						{
							int cell1 = cells1[i];
							for (int j = 0; j < cells2.Count; j++)
							{
								int cell2 = cells2[j];
								int b1 = cell1 / 9 / 3 * 3 + cell1 % 9 / 3;
								if (b1 == cell2 / 9 / 3 * 3 + cell2 % 9 / 3)
								{
									(sameBlock, c1Index, c2Index, headIndex, tailIndex) =
										(b1, i, j, i == 0 ? 1 : 0, j == 0 ? 1 : 0);
									goto Label_Checking;
								}
							}
						}

						// Not same block.
						continue;

					Label_Checking:
						// Two strong link found.
						// Record all eliminations.
						int head, tail;
						head = cells1[headIndex];
						tail = cells2[tailIndex];
						var conclusions = new List<Conclusion>();
						var gridMap = new GridMap(head, false) & new GridMap(tail, false);
						if (gridMap.Count == 0)
						{
							continue;
						}

						foreach (int cell in gridMap.Offsets)
						{
							if (grid.CandidateExists(cell, digit))
							{
								conclusions.Add(new Conclusion(ConclusionType.Elimination, cell * 9 + digit));
							}
						}

						if (conclusions.Count == 0)
						{
							continue;
						}

						result.Add(
							new TwoStrongLinksTechniqueInfo(
								conclusions,
								views: new[]
								{
									new View(
										cellOffsets: null,
										candidateOffsets: new List<(int, int)>
										{
											(0, cells1[c1Index] * 9 + digit),
											(0, cells2[c2Index] * 9 + digit),
											(0, head * 9 + digit),
											(0, tail * 9 + digit)
										},
										regionOffsets: new List<(int, int)>
										{
											(0, r1),
											(0, r2),
											(1, sameBlock)
										},
										linkMasks: null)
								},
								digit,
								baseRegion: r1,
								targetRegion: r2));
					}
				}
			}

			return result;
		}
	}
}
