using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static System.Algorithms;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Encapsulates a <b>Borescoper's deadly pattern</b> technique searcher.
	/// </summary>
	[TechniqueDisplay("Borescoper's Deadly Pattern")]
	public sealed partial class BdpTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <summary>
		/// All different patterns.
		/// </summary>
		/// <remarks>
		/// All possible heptagons and octagons are in here.
		/// </remarks>
		private static readonly Pattern[] Patterns = new Pattern[14580];


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 53;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			if (EmptyMap.Count < 7)
			{
				return;
			}

			for (int i = 0, end = EmptyMap.Count == 7 ? 14580 : 11664; i < end; i++)
			{
				var pattern = Patterns[i];
				if ((EmptyMap | pattern.Map) != EmptyMap)
				{
					// The pattern contains non-empty cells.
					continue;
				}

				short cornerMask1 = GetMask(grid, pattern.Pair1Map);
				short cornerMask2 = GetMask(grid, pattern.Pair2Map);
				short centerMask = GetMask(grid, pattern.CenterCellsMap);
				CheckType1(accumulator, grid, pattern, cornerMask1, cornerMask2, centerMask);
			}
		}

		private void CheckType1(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, Pattern pattern, short cornerMask1,
			short cornerMask2, short centerMask)
		{
			// ab  ab     | abc abc
			// abc abc ab | abc abc abc
			//     abc ab |     abc abc
			short orMask = (short)((short)(cornerMask1 | cornerMask2) | centerMask);
			if (orMask.CountSet() != (pattern.IsHeptagon ? 4 : 5))
			{
				return;
			}

			// Iterate on each combination.
			var map = pattern.Map;
			foreach (int[] digits in GetCombinationsOfArray(orMask.GetAllSets().ToArray(), pattern.IsHeptagon ? 3 : 4))
			{
				short tempMask = 0;
				foreach (int digit in digits)
				{
					tempMask |= (short)(1 << digit);
				}

				int otherDigit = (orMask & ~tempMask).FindFirstSet();
				var mapContainingThatDigit = map & CandMaps[otherDigit];
				if (mapContainingThatDigit.Count != 1)
				{
					continue;
				}

				int elimCell = mapContainingThatDigit.SetAt(0);
				short elimMask = (short)(grid.GetCandidatesReversal(elimCell) & tempMask);
				if (elimMask == 0)
				{
					continue;
				}

				var conclusions = new List<Conclusion>();
				foreach (int digit in elimMask.GetAllSets())
				{
					conclusions.Add(new Conclusion(Elimination, elimCell, digit));
				}

				var candidateOffsets = new List<(int, int)>();
				foreach (int cell in map.Offsets)
				{
					if (mapContainingThatDigit[cell])
					{
						continue;
					}

					foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
					{
						candidateOffsets.Add((0, cell * 9 + digit));
					}
				}

				accumulator.Add(
					new BdpType1TechniqueInfo(
						conclusions,
						views: new[]
						{
							new View(
								cellOffsets: null,
								candidateOffsets,
								regionOffsets: null,
								links: null)
						},
						digitsMask: tempMask,
						map));
			}
		}


		/// <summary>
		/// Get the mask.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="map">The map.</param>
		/// <returns>The mask.</returns>
		private static short GetMask(IReadOnlyGrid grid, GridMap map)
		{
			short mask = 0;
			foreach (int cell in map.Offsets)
			{
				mask |= grid.GetCandidatesReversal(cell);
			}

			return mask;
		}
	}
}
