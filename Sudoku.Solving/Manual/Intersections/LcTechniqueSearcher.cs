using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using static Sudoku.GridProcessings;
using static Sudoku.Solving.ConclusionType;

namespace Sudoku.Solving.Manual.Intersections
{
	/// <summary>
	/// Encapsulates a <b>locked candidates</b> (LC) technique searcher.
	/// </summary>
	[TechniqueDisplay("Locked Candidates")]
	public sealed class LcTechniqueSearcher : IntersectionTechniqueSearcher
	{
		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 26;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			var (emptyCellsMap, _, digitDistributions) = grid;

			var r = (Span<int>)stackalloc int[2];
			foreach (var ((baseSet, coverSet), (a, b, c)) in IntersectionMaps)
			{
				if (!emptyCellsMap.Overlaps(c))
				{
					continue;
				}

				short m1 = BitwiseOrMasks(grid, a);
				short m2 = BitwiseOrMasks(grid, b);
				short m3 = BitwiseOrMasks(grid, c);
				short m = (short)(m3 & (m1 ^ m2));
				if (m == 0)
				{
					continue;
				}

				foreach (int digit in m.GetAllSets())
				{
					GridMap elimMap;
					var conclusions = new List<Conclusion>();
					(r[0], r[1], elimMap) =
						a.Overlaps(digitDistributions[digit]) ? (coverSet, baseSet, a) : (baseSet, coverSet, b);

					foreach (int cell in elimMap.Offsets)
					{
						if (!(grid.Exists(cell, digit) is true))
						{
							continue;
						}

						conclusions.Add(new Conclusion(Elimination, cell, digit));
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					accumulator.Add(
						new LcTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets:
										new List<(int, int)>(
											from cell in c.Offsets
											where grid.Exists(cell, digit) is true
											select (0, cell * 9 + digit)),
									regionOffsets: new[] { (0, r[0]), (1, r[1]) },
									links: null)
							},
							digit,
							baseSet: r[0],
							coverSet: r[1]));
				}
			}
		}


		/// <summary>
		/// Bitwise or all masks.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="map">The grid map.</param>
		/// <returns>The result.</returns>
		private static short BitwiseOrMasks(IReadOnlyGrid grid, GridMap map)
		{
			short mask = 0;
			foreach (int offset in map.Offsets)
			{
				mask |= grid.GetCandidatesReversal(offset);
			}

			return mask;
		}
	}
}
