using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Wings.Irregular
{
	/// <summary>
	/// Encapsulates an irregular wing technique searcher.
	/// </summary>
	public sealed class IrregularWingTechniqueSearcher : TechniqueSearcher
	{
		/// <inheritdoc/>
		public override IReadOnlyList<TechniqueInfo> TakeAll(Grid grid)
		{
			// Search for all conjugate pairs.
			//var conjugatePairs = grid.GetAllConjugatePairs();

			// Then search for all bivalue cells.
			var bivalueCellsMap = grid.GetBivalueCellsMap(out int bivalueCellsCount);
			var pair = (bivalueCellsMap, bivalueCellsCount);

			// Finally search all irregular wings.
			var result = new List<TechniqueInfo>();

			result.AddRange(TakeAllWWings(grid, in pair));
			// TODO: Find all M-Wings.
			// TODO: Find all Local-Wings.
			// TODO: Find all Split-Wings.
			// TODO: Find all Hybrid-Wings.

			return result;
		}


		#region Irregular wing utils
		/// <summary>
		/// Search for all W-Wings.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="pair">(<see langword="in"/> parameter) bivalue cell information pair.</param>
		/// <returns>All technique information instances.</returns>
		public static IReadOnlyList<IrregularWingTechniqueInfo> TakeAllWWings(
			Grid grid, in (GridMap _map, int _count) pair)
		{
			var (bivalueMap, count) = pair;
			if (count < 2)
			{
				return Array.Empty<IrregularWingTechniqueInfo>();
			}

			// Iterate on each cells.
			var result = new List<WWingTechniqueInfo>();
			for (int c1 = 0; c1 < 81; c1++)
			{
				if (!bivalueMap[c1] || grid.GetCellStatus(c1) != CellStatus.Empty)
				{
					continue;
				}

				// Iterate on each cells which are not peers in 'c1'.
				int[] digits = grid.GetCandidatesReversal(c1).GetAllSets().ToArray();
				foreach (int c2 in (~new GridMap(c1)).Offsets)
				{
					if (c2 <= c1
						|| grid.GetCellStatus(c2) != CellStatus.Empty
						|| grid.GetCandidates(c1) != grid.GetCandidates(c2))
					{
						continue;
					}

					var intersection = new GridMap(c1) & new GridMap(c2);
					if (intersection.Offsets.All(o => grid.GetCellStatus(o) != CellStatus.Empty))
					{
						continue;
					}

					ValueTuple<int, int, int> triplet1, triplet2;
					var (row1, column1, block1) = triplet1 = CellUtils.GetRegion(c1);
					var (row2, column2, block2) = triplet2 = CellUtils.GetRegion(c2);

					// Each rows.
					int region;
					for (region = 9; region < 18; region++)
					{
						if (row1 == region || row2 == region)
						{
							continue;
						}

						SearchWWingByRegions(
							result, grid, digits, region, c1, c2,
							in triplet1, in triplet2, intersection);
					}

					// Each columns.
					for (; region < 27; region++)
					{
						if (column1 == region || column2 == region)
						{
							continue;
						}

						SearchWWingByRegions(
							result, grid, digits, region, c1, c2,
							in triplet1, in triplet2, intersection);
					}
				}
			}

			return result;
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
		/// <param name="triplet1">(<see langword="in"/> parameter) The triplet 1.</param>
		/// <param name="triplet2">(<see langword="in"/> parameter) The triplet 2.</param>
		/// <param name="intersection">The intersection.</param>
		private static void SearchWWingByRegions(
			IList<WWingTechniqueInfo> result, Grid grid, int[] digits, int region,
			int c1, int c2, in (int _row, int _column, int _block) triplet1,
			in (int _row, int _column, int _block) triplet2, GridMap intersection)
		{
			for (int i = 0; i < 2; i++)
			{
				int digit = digits[i];
				int elimDigit = i == 0 ? digits[1] : digits[0];
				short mask = grid.GetDigitAppearingMask(digit, region);
				if (mask.CountSet() == 2)
				{
					int[] pos = mask.GetAllSets().ToArray();
					int bridgeStart = RegionUtils.GetCellOffset(region, pos[0]);
					int bridgeEnd = RegionUtils.GetCellOffset(region, pos[1]);
					if (c1 == bridgeStart || c2 == bridgeStart
						|| c1 == bridgeEnd || c2 == bridgeEnd)
					{
						continue;
					}

					var (a, b) = region switch
					{
						_ when region >= 9 && region < 18 => (triplet1._column, triplet2._column),
						_ when region >= 18 && region < 27 => (triplet1._row, triplet2._row),
						_ => throw new NotSupportedException("Out of range.")
					};
					if (pos[0] == a && pos[1] == b)
					{
						// W-Wing found.
						var conclusions = new List<Conclusion>();
						foreach (int offset in intersection.Offsets)
						{
							if (grid.CandidateExists(offset, elimDigit))
							{
								conclusions.Add(
									new Conclusion(
										ConclusionType.Elimination, offset, elimDigit));
							}
						}

						if (conclusions.Count != 0)
						{
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
											linkMasks: null)
									},
									startCellOffset: c1,
									endCellOffset: c2,
									conjugatePair: new ConjugatePair(bridgeStart, bridgeEnd, digit)));
						}
					}
				}
			}
		}
		#endregion
	}
}
