using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Sdps
{
	/// <summary>
	/// Encapsulates a <b>two strong links</b> technique searcher.
	/// </summary>
	public sealed class TwoStrongLinksStepSearcher : SdpStepSearcher
	{
		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(40, nameof(TechniqueCode.TurbotFish)) { DisplayLevel = 2 };


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			for (int digit = 0; digit < 9; digit++)
			{
				for (int r1 = 0; r1 < 26; r1++)
				{
					for (int r2 = r1 + 1; r2 < 27; r2++)
					{
						// Get masks.
						short mask1 = (RegionMaps[r1] & CandMaps[digit]).GetSubviewMask(r1);
						short mask2 = (RegionMaps[r2] & CandMaps[digit]).GetSubviewMask(r2);
						if ((mask1.PopCount(), mask2.PopCount()) != (2, 2))
						{
							continue;
						}

						// Get all cells.
						GridMap map1 = GridMap.Empty, map2 = GridMap.Empty;
						List<int> cells1 = new(), cells2 = new();
						foreach (int pos1 in mask1)
						{
							int cell1 = RegionCells[r1][pos1];
							cells1.Add(cell1);
							map1.AddAnyway(cell1);
						}
						foreach (int pos2 in mask2)
						{
							int cell2 = RegionCells[r2][pos2];
							cells2.Add(cell2);
							map2.AddAnyway(cell2);
						}

						if ((map1 & map2).IsNotEmpty)
						{
							continue;
						}

						// Check two cells share a same region.
						int sameRegion, headIndex, tailIndex, c1Index, c2Index;
						for (int i = 0; i < 2; i++)
						{
							int cell1 = cells1[i];
							for (int j = 0; j < 2; j++)
							{
								int cell2 = cells2[j];
								if (new GridMap { cell1, cell2 }.AllSetsAreInOneRegion(out sameRegion))
								{
									(c1Index, c2Index, headIndex, tailIndex) = (i, j, i == 0 ? 1 : 0, j == 0 ? 1 : 0);
									goto Checking;
								}
							}
						}

						// Not same block.
						continue;

					Checking:
						// Two strong link found.
						// Record all eliminations.
						int head = cells1[headIndex], tail = cells2[tailIndex];
						var conclusions = new List<Conclusion>();
						var gridMap = PeerMaps[head] & PeerMaps[tail] & CandMaps[digit];
						if (gridMap.IsEmpty)
						{
							continue;
						}

						foreach (int cell in gridMap)
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						accumulator.Add(
							new TwoStrongLinksStepInfo(
								conclusions,
								new View[]
								{
									new()
									{
										Candidates = new DrawingInfo[]
										{
											new(0, cells1[c1Index] * 9 + digit),
											new(0, cells2[c2Index] * 9 + digit),
											new(0, head * 9 + digit),
											new(0, tail * 9 + digit)
										},
										Regions = new DrawingInfo[] { new(1, sameRegion) }
									}
								},
								digit,
								r1,
								r2));
					}
				}
			}
		}
	}
}
