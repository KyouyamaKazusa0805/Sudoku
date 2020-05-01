using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Utils;
using static Sudoku.Data.CellStatus;
using static Sudoku.GridProcessings;
using static Sudoku.Solving.ConclusionType;

namespace Sudoku.Solving.Manual.Wings.Irregular
{
	/// <summary>
	/// Encapsulates an <b>irregular wing</b> technique searcher.
	/// </summary>
	[TechniqueDisplay("Irregular Wing")]
	public sealed class IrregularWingTechniqueSearcher : TechniqueSearcher
	{
		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 44;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			(_, var bivalueCellsMap, _, _) = grid;

			// Finally search all irregular wings.
			// Hybrid-Wings, Local-Wings, Split-Wings and M-Wings can
			// be found in another searcher.
			// These wings are not elementary and necessary techniques
			// so we does not need to list them.
			TakeAllWWings(accumulator, grid, bivalueCellsMap);
		}

		/// <summary>
		/// Search for all W-Wings.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="bivalueCellsMap">(<see langword="in"/> parameter) bivalue cell information pair.</param>
		/// <returns>All technique information instances.</returns>
		public static void TakeAllWWings(IBag<TechniqueInfo> result, IReadOnlyGrid grid, GridMap bivalueCellsMap)
		{
			if (bivalueCellsMap.Count < 2)
			{
				return;
			}

			// Iterate on each cells.
			for (int c1 = 0; c1 < 72; c1++)
			{
				if (!bivalueCellsMap[c1] || grid.GetStatus(c1) != Empty)
				{
					continue;
				}

				// Iterate on each cells which are not peers in 'c1'.
				int[] digits = grid.GetCandidatesReversal(c1).GetAllSets().ToArray();
				foreach (int c2 in (~new GridMap(c1)).Offsets)
				{
					if (c2 < c1 || grid.GetCandidatesReversal(c1) != grid.GetCandidatesReversal(c2))
					{
						continue;
					}

					var intersection = new GridMap(c1, false) & new GridMap(c2, false);
					if (intersection.Offsets.All(o => grid.GetStatus(o) != Empty))
					{
						continue;
					}

					var (row1, column1, block1) = CellUtils.GetRegion(c1);
					var (row2, column2, block2) = CellUtils.GetRegion(c2);

					// Each rows.
					int region;
					for (region = 9; region < 18; region++)
					{
						if (row1 == region || row2 == region)
						{
							continue;
						}

						SearchWWingByRegions(result, grid, digits, region, c1, c2, intersection);
					}

					// Each columns.
					for (; region < 27; region++)
					{
						if (column1 == region || column2 == region)
						{
							continue;
						}

						SearchWWingByRegions(result, grid, digits, region, c1, c2, intersection);
					}
				}
			}
		}

		/// <summary>
		/// Searches W-Wing technique by region.
		/// </summary>
		/// <param name="result">The result.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="digits">The digits.</param>
		/// <param name="region">The region.</param>
		/// <param name="c1">Cell 1.</param>
		/// <param name="c2">Cell 2.</param>
		/// <param name="intersection">The intersection.</param>
		private static void SearchWWingByRegions(
			IBag<TechniqueInfo> result, IReadOnlyGrid grid, int[] digits, int region,
			int c1, int c2, GridMap intersection)
		{
			for (int i = 0; i < 2; i++)
			{
				int digit = digits[i];
				short mask = grid.GetDigitAppearingMask(digit, region);
				if (mask.CountSet() != 2)
				{
					continue;
				}

				int[] pos = mask.GetAllSets().ToArray();
				int bridgeStart = RegionCells[region][pos[0]];
				int bridgeEnd = RegionCells[region][pos[1]];
				if (c1 == bridgeStart || c2 == bridgeStart || c1 == bridgeEnd || c2 == bridgeEnd)
				{
					continue;
				}

				static bool c(int c1, int c2) => (new GridMap(c1, false) & new GridMap(c2, false)).CoveredRegions.Any();
				if (!(c(bridgeStart, c1) && c(bridgeEnd, c2)) && !(c(bridgeStart, c2) && c(bridgeEnd, c1)))
				{
					continue;
				}

				// W-Wing found.
				var conclusions = new List<Conclusion>();
				int elimDigit = i == 0 ? digits[1] : digits[0];
				foreach (int offset in intersection.Offsets)
				{
					if (!(grid.Exists(offset, elimDigit) is true))
					{
						continue;
					}

					conclusions.Add(new Conclusion(Elimination, offset, elimDigit));
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				result.Add(
					new WWingTechniqueInfo(
						conclusions,
						views: new[]
						{
							new View(
								cellOffsets: null,
								candidateOffsets: new List<(int, int)>
								{
									(1, c1 * 9 + elimDigit),
									(0, c1 * 9 + digit),
									(0, bridgeStart * 9 + digit),
									(0, bridgeEnd * 9 + digit),
									(0, c2 * 9 + digit),
									(1, c2 * 9 + elimDigit)
								},
								regionOffsets: new[] { (0, region) },
								links: null)
						},
						startCellOffset: c1,
						endCellOffset: c2,
						conjugatePair: new ConjugatePair(bridgeStart, bridgeEnd, digit)));
			}
		}
	}
}
