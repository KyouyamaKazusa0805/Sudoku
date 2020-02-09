using System.Collections.Generic;
using System.Linq;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using Intersection = System.ValueTuple<int, int, Sudoku.Data.Meta.GridMap, Sudoku.Data.Meta.GridMap>;

namespace Sudoku.Solving.Manual.Intersections
{
	/// <summary>
	/// Encapsulates a locked candidates technique searcher.
	/// </summary>
	public sealed class IntersectionTechniqueSearcher : TechniqueSearcher
	{
		/// <summary>
		/// All intersection series.
		/// </summary>
		private readonly Intersection[,] _intersection;


		/// <summary>
		/// Initializes an instance with the specified intersection table.
		/// </summary>
		/// <param name="intersection">The intersection table.</param>
		public IntersectionTechniqueSearcher(Intersection[,] intersection) =>
			_intersection = intersection;


		/// <inheritdoc/>
		public override IReadOnlyList<TechniqueInfo> TakeAll(Grid grid)
		{
			var result = new List<TechniqueInfo>();

			for (int i = 0; i < 18; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					var (baseSet, coverSet, left, right) = _intersection[i, j];
					var intersection = left & right;
					if (intersection.Offsets.All(o => grid.GetCellStatus(o) != CellStatus.Empty))
					{
						continue;
					}

					var a = left ^ intersection;
					var b = right ^ intersection;
					short mask1 = BitwiseAndMasks(grid, a);
					short mask2 = BitwiseAndMasks(grid, b);
					short mask3 = BitwiseAndMasks(grid, intersection);
					short mask = (short)((short)(mask3 | (short)~(mask1 ^ mask2)) & 511);
					if (mask == 511)
					{
						continue;
					}

					// Locked candidates found.
					var candidatesList = new List<(int, int)>();
					short temp = mask;
					for (int digit = 0; digit < 9; digit++, temp >>= 1)
					{
						if ((temp & 1) != 0)
						{
							continue;
						}

						// 'digit' is locked number.
						foreach (int offset in intersection.Offsets)
						{
							if (grid.GetCellStatus(offset) != CellStatus.Empty
								|| grid[offset, digit])
							{
								continue;
							}

							candidatesList.Add((0, offset * 9 + digit));
						}

						var conclusions = new List<Conclusion>();
						int[] lockedRegions = new int[2];
						bool isClaiming = true;
						foreach (int offset in a.Offsets)
						{
							if (grid.GetCellStatus(offset) != CellStatus.Empty
								|| grid[offset, digit])
							{
								continue;
							}

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
						if (isClaiming)
						{
							// Claiming.
							lockedRegions[0] = baseSet;
							lockedRegions[1] = coverSet;
							foreach (int offset in b.Offsets)
							{
								if (grid.GetCellStatus(offset) != CellStatus.Empty
									|| grid[offset, digit])
								{
									continue;
								}

								conclusions.Add(
									new Conclusion(
										conclusionType: ConclusionType.Elimination,
										cellOffset: offset,
										digit));
							}
						}

						if (conclusions.Count == 0)
						{
							continue;
						}

						result.Add(
							new IntersectionTechniqueInfo(
								conclusions,
								views: new[]
								{
									new View(
										cellOffsets: null,
										candidateOffsets: candidatesList,
										regionOffsets: null,
										linkMasks: null)
								},
								digit,
								baseSet: lockedRegions[0],
								coverSet: lockedRegions[1]));
					}
				}
			}

			return result;
		}

		#region Intersection utils
		/// <summary>
		/// Bitwise and all masks.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="map">The grid map.</param>
		/// <returns>The result.</returns>
		private static short BitwiseAndMasks(Grid grid, GridMap map)
		{
			short mask = 511;
			foreach (int offset in map.Offsets)
			{
				mask &= grid.GetMask(offset);
			}

			return mask;
		}
		#endregion
	}
}
