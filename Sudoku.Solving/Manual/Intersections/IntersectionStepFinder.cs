using System.Collections.Generic;
using System.Linq;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using static Sudoku.Solving.Utils.RegionUtils;

namespace Sudoku.Solving.Manual.Intersections
{
	public sealed class IntersectionStepFinder : StepFinder
	{
		//private static readonly int[,] IntersectionSegments = new int[9, 4]
		//{
		//	{ 1, 2, 3, 6 }, { 0, 2, 4, 7 }, { 0, 1, 5, 8 },
		//	{ 4, 5, 0, 6 }, { 3, 5, 1, 7 }, { 3, 4, 2, 8 },
		//	{ 7, 8, 0, 3 }, { 6, 8, 1, 4 }, { 6, 7, 2, 5 }
		//};

		private static readonly (int _base, int _cover, GridMap _left, GridMap _right)[,] IntersectionSeries = new (int, int, GridMap, GridMap)[18, 3];


		static IntersectionStepFinder()
		{
			int[] zz = { 0, 3, 6, 1, 4, 7, 2, 5, 8 };
			for (int i = 0; i < 18; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					int baseSet = i + 9;
					int coverSet = i < 9
						? i / 3 * 3 + j
						: zz[(i - 9) / 3 * 3 + j];
					IntersectionSeries[i, j] = (
						baseSet, coverSet,
						new GridMap(GetCellOffsets(baseSet)), new GridMap(GetCellOffsets(coverSet)));
				}
			}
		}


		public override IReadOnlyList<TechniqueInfo> TakeAll(Grid grid)
		{
			var result = new List<TechniqueInfo>();

			for (int i = 0; i < 18; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					short BitwiseAndMasks(GridMap map)
					{
						short mask = 511;
						foreach (int offset in map.Offsets)
						{
							mask &= grid.GetMask(offset);
						}

						return mask;
					}

					var (baseSet, coverSet, left, right) = IntersectionSeries[i, j];
					var intersection = left & right;
					if (intersection.Offsets.All(o => grid.GetCellStatus(o) != CellStatus.Empty))
					{
						continue;
					}

					var a = left ^ intersection;
					var b = right ^ intersection;
					short mask1 = BitwiseAndMasks(a);
					short mask2 = BitwiseAndMasks(b);
					short mask3 = BitwiseAndMasks(intersection);
					short mask = (short)((short)(mask3 | (short)~(mask1 ^ mask2)) & 511);
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
								foreach (int offset in intersection.Offsets)
								{
									if (grid.GetCellStatus(offset) == CellStatus.Empty
										&& !grid[offset, digit])
									{
										candidatesList.Add((0, offset * 9 + digit));
									}
								}

								var conclusions = new List<Conclusion>();
								int[] lockedRegions = new int[2];
								bool isClaiming = true;
								foreach (int offset in a.Offsets)
								{
									if (grid.GetCellStatus(offset) == CellStatus.Empty
										&& !grid[offset, digit])
									{
										// Pointing.
										if (isClaiming)
										{
											lockedRegions[0] = coverSet;
											lockedRegions[1] = baseSet;
											isClaiming = !isClaiming;
										}
										conclusions.Add(
											new Conclusion(
												conclusionType: ConclusionType.Elimination,
												cellOffset: offset,
												digit));
									}
								}
								if (isClaiming)
								{
									// Claiming.
									lockedRegions[0] = baseSet;
									lockedRegions[1] = coverSet;
									foreach (int offset in b.Offsets)
									{
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
											baseRegion: lockedRegions[0],
											coverRegion: lockedRegions[1]));
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
