using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;
using static Sudoku.Solving.Constants.Processings;

namespace Sudoku.Solving.Manual.Intersections
{
	/// <summary>
	/// Encapsulates an <b>almost locked candidates</b> (ALC) technique searcher.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.AlmostLockedPair))]
	[SearcherProperty(45)]
	public sealed class AlcTechniqueSearcher : IntersectionTechniqueSearcher
	{
		/// <summary>
		/// Indicates the searcher will check almost locked quadruple (ALQ).
		/// </summary>
		private readonly bool _checkAlq;


		/// <summary>
		/// Initializes an instance with the intersection table.
		/// </summary>
		/// <param name="checkAlq">
		/// Indicates whether the searcher should check almost locked quadruple.
		/// </param>
		public AlcTechniqueSearcher(bool checkAlq) => _checkAlq = checkAlq;


		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, Grid grid)
		{
			for (int size = 2; size <= (_checkAlq ? 4 : 3); size++)
			{
				GetAll(accumulator, grid, size);
			}
		}


		/// <summary>
		/// Get all technique information instances by size.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="size">The size.</param>
		/// <returns>The result.</returns>
		private static void GetAll(IList<TechniqueInfo> result, Grid grid, int size)
		{
			foreach (var ((baseSet, coverSet), (a, b, c)) in IntersectionMaps)
			{
				if (c.Overlaps(EmptyMap))
				{
					// Process for 2 cases.
					GetAll(result, grid, size, baseSet, coverSet, a, b, c);
					GetAll(result, grid, size, coverSet, baseSet, b, a, c);
				}
			}
		}

		/// <summary>
		/// Process the calculation.
		/// </summary>
		/// <param name="result">The result.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="size">The size.</param>
		/// <param name="baseSet">The base set.</param>
		/// <param name="coverSet">The cover set.</param>
		/// <param name="a">The left grid map.</param>
		/// <param name="b">The right grid map.</param>
		/// <param name="c">The intersection.</param>
		private static void GetAll(
			IList<TechniqueInfo> result, Grid grid, int size, int baseSet, int coverSet,
			GridMap a, GridMap b, GridMap c)
		{
			foreach (int[] cells in (a & EmptyMap).ToArray().GetSubsets(size - 1))
			{
				short mask = BitwiseOrMasks(grid, cells);
				if (mask.CountSet() != size)
				{
					continue;
				}

				var digits = mask.GetAllSets();
				if (digits.Any(d => ValueMaps[d].Overlaps(RegionMaps[coverSet])))
				{
					continue;
				}

				short ahsMask = 0;
				foreach (int digit in digits)
				{
					ahsMask |= (RegionMaps[coverSet] & CandMaps[digit] & b).GetSubviewMask(coverSet);
				}
				if (ahsMask.CountSet() != size - 1)
				{
					continue;
				}

				var ahsCells = GridMap.Empty;
				foreach (int pos in ahsMask.GetAllSets())
				{
					ahsCells.AddAnyway(RegionCells[coverSet][pos]);
				}

				// Record all eliminations.
				var cellsMap = new GridMap(cells);
				var conclusions = new List<Conclusion>();
				foreach (int aCell in a)
				{
					if (cellsMap[aCell])
					{
						continue;
					}

					foreach (int digit in (mask & grid.GetCandidateMask(aCell)).GetAllSets())
					{
						conclusions.Add(new(Elimination, aCell, digit));
					}
				}
				foreach (int digit in (Grid.MaxCandidatesMask & ~mask).GetAllSets())
				{
					foreach (int ahsCell in ahsCells & CandMaps[digit])
					{
						conclusions.Add(new(Elimination, ahsCell, digit));
					}
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				// Gather highlight candidates.
				var candidateOffsets = new List<(int, int)>();
				foreach (int digit in digits)
				{
					foreach (int cell in cellsMap & CandMaps[digit])
					{
						candidateOffsets.Add((0, cell * 9 + digit));
					}
				}
				foreach (int cell in c)
				{
					foreach (int digit in (mask & grid.GetCandidateMask(cell)).GetAllSets())
					{
						candidateOffsets.Add((1, cell * 9 + digit));
					}
				}
				foreach (int cell in ahsCells)
				{
					foreach (int digit in (mask & grid.GetCandidateMask(cell)).GetAllSets())
					{
						candidateOffsets.Add((0, cell * 9 + digit));
					}
				}

				var valueCells = from Cell in (cellsMap | ahsCells) - EmptyMap select (0, Cell);
				bool hasValueCell = valueCells.Any();
				result.Add(
					new AlcTechniqueInfo(
						conclusions,
						views: new[]
						{
							new View(
								hasValueCell ? valueCells.ToArray() : null,
								candidateOffsets, new[] { (0, baseSet), (1, coverSet) },  null)
						},
						digits: mask,
						baseCells: cellsMap,
						targetCells: ahsCells,
						hasValueCell));
			}
		}
	}
}
