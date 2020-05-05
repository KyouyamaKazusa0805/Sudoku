using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using static Sudoku.GridProcessings;

namespace Sudoku.Solving.Manual.Sdps
{
	/// <summary>
	/// Encapsulates a two strong links technique searcher.
	/// </summary>
	[TechniqueDisplay("Two Strong Links")]
	public sealed class TwoStrongLinksTechniqueSearcher : SdpTechniqueSearcher
	{
		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 40;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			for (int digit = 0; digit < 9; digit++)
			{
				for (int r1 = 0; r1 < 26; r1++)
				{
					for (int r2 = r1 + 1; r2 < 27; r2++)
					{
						// Get masks.
						short mask1 = grid.GetDigitAppearingMask(digit, r1);
						short mask2 = grid.GetDigitAppearingMask(digit, r2);
						if (mask1.CountSet() != 2 || mask2.CountSet() != 2)
						{
							continue;
						}

						// Get all cells.
						var map1 = GridMap.Empty;
						var map2 = GridMap.Empty;
						var cells1 = new List<int>();
						var cells2 = new List<int>();
						foreach (int pos1 in mask1.GetAllSets())
						{
							int cell1 = RegionCells[r1][pos1];
							cells1.Add(cell1);
							map1.Add(cell1);
						}
						foreach (int pos2 in mask2.GetAllSets())
						{
							int cell2 = RegionCells[r2][pos2];
							cells2.Add(cell2);
							map2.Add(cell2);
						}

						if ((map1 & map2).IsNotEmpty)
						{
							continue;
						}

						// Check two cells share a same region.
						int sameRegion;
						int headIndex, tailIndex, c1Index, c2Index;
						for (int i = 0; i < 2; i++)
						{
							int cell1 = cells1[i];
							for (int j = 0; j < 2; j++)
							{
								int cell2 = cells2[j];
								if (new GridMap { cell1, cell2 }.AllSetsAreInOneRegion(out sameRegion))
								{
									(c1Index, c2Index, headIndex, tailIndex) = (
										i, j, (i == 0).ToInt32(), (j == 0).ToInt32());
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
						if (gridMap.IsEmpty)
						{
							continue;
						}

						foreach (int cell in gridMap.Offsets)
						{
							if (!(grid.Exists(cell, digit) is true))
							{
								continue;
							}

							conclusions.Add(new Conclusion(ConclusionType.Elimination, cell, digit));
						}

						if (conclusions.Count == 0)
						{
							continue;
						}

						accumulator.Add(
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
											(1, sameRegion)
										},
										links: null)
								},
								digit,
								baseRegion: r1,
								targetRegion: r2));
					}
				}
			}
		}
	}
}
