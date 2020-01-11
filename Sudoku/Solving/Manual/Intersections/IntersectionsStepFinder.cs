using System;
using System.Collections.Generic;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;
using static Sudoku.Solving.Utils.RegionUtils;

namespace Sudoku.Solving.Manual.Intersections
{
	public sealed class IntersectionsStepFinder : StepFinder
	{
		private static readonly int[,] IntersectionSegments = new int[9, 4]
		{
			{ 1, 2, 3, 6 }, { 0, 2, 4, 7 }, { 0, 1, 5, 8 },
			{ 4, 5, 0, 6 }, { 3, 5, 1, 7 }, { 3, 4, 2, 8 },
			{ 7, 8, 0, 3 }, { 6, 8, 1, 4 }, { 6, 7, 2, 5 }
		};


		public override IList<TechniqueInfo> TakeAll(Grid grid)
		{
			var result = new List<TechniqueInfo>();

			for (int region = 9; region <= 24; region += 3)
			{
				var seg = (Span<short>)stackalloc short[9];
				for (int i = 0; i < 3; i++)
				{
					for (int j = 0, z = j * 3; j < 3; j++, z = j * 3)
					{
						seg[i * 3 + j] = (short)(511
							& grid.GetMask(GetCellOffset(region + i, z))
							& grid.GetMask(GetCellOffset(region + i, z + 1))
							& grid.GetMask(GetCellOffset(region + i, z + 2)));
					}
				}

				for (int i = 0; i < 9; i++)
				{
					// 'i' means the current intersection (box-row or box-column).
					short currentSeg = seg[i];
					var (a, b) = (
						(short)(seg[IntersectionSegments[i, 0]] & seg[IntersectionSegments[i, 1]]),
						(short)(seg[IntersectionSegments[i, 2]] & seg[IntersectionSegments[i, 3]]));
					short mask = (short)(currentSeg | (short)~(a ^ b));
					if (mask != 511)
					{
						// Locked candidates found.
						var candidatesList = new List<(int, int)>();
						short temp = mask;
						for (int digit = 0; digit < 9; digit++, temp >>= 1)
						{
							if ((temp & 1) == 0)
							{
								// 'digit' is locked number.
								int offset = 0;
								for (int x = 0; x < 3; x++)
								{
									offset = GetCellOffset(i / 3 + region, i % 3 * 3 + x);
									if (grid.GetCellStatus(offset) == CellStatus.Empty
										&& !grid[offset, digit])
									{
										candidatesList.Add((0, offset * 9 + digit));
									}
								}

								int baseRegion;
								int[] lockedRegions = new int[2];
								if ((currentSeg | a) != 511)
								{
									// Claiming.
									baseRegion = GetRegionOffset(
										CellUtils.ToFullString(offset)[region < 18 ? 0..2 : 2..4]);
									lockedRegions[0] = IntersectionSegments[i, 0];
									lockedRegions[1] = IntersectionSegments[i, 1];
								}
								else
								{
									// Pointing.
									baseRegion = GetRegionOffset(CellUtils.ToFullString(offset)[4..6]);
									lockedRegions[0] = IntersectionSegments[i, 2];
									lockedRegions[1] = IntersectionSegments[i, 3];
								}

								var conclusions = new List<Conclusion>();
								for (int lockedRegionIndex = 0; lockedRegionIndex < 2; lockedRegionIndex++)
								{
									for (int x = 0; x < 3; x++)
									{
										offset = GetCellOffset(
											lockedRegions[lockedRegionIndex] / 3 + region,
											lockedRegions[lockedRegionIndex] % 3 * 3 + x);
										if (grid.GetCellStatus(offset) == CellStatus.Empty
											&& !grid[offset, digit])
										{
											conclusions.Add(
												new Conclusion(
													conclusionType: ConclusionType.Elimination,
													cellOffset: offset,
													digit));
										}
									}
								}

								if (conclusions.Count != 0)
								{
									result.Add(
										new IntersectionTechniqueInfo(
											conclusions,
											views: new List<View>
											{
												new View(
													cellOffsets: null,
													candidateOffsets: candidatesList,
													regionOffsets: null,
													linkMasks: null)
											},
											digit,
											baseRegion));
								}
							}
						}
					}
				}
			}

			return result;
		}
	}
}
