using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Extensions;
using static Sudoku.Constants.Processings;

namespace Sudoku.Solving.Manual.Intersections
{
	/// <summary>
	/// Encapsulates an <b>almost locked candidates</b> (ALC) technique searcher.
	/// </summary>
	public sealed class AlcStepSearcher : IntersectionStepSearcher
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
		public AlcStepSearcher(bool checkAlq) => _checkAlq = checkAlq;


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(45, nameof(TechniqueCode.AlmostLockedPair))
		{
			DisplayLevel = 2
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			for (int size = 2, maxSize = _checkAlq ? 4 : 3; size <= maxSize; size++)
			{
				GetAll(accumulator, grid, size);
			}
		}


		/// <summary>
		/// Get all technique information instances by size.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="size">The size.</param>
		/// <returns>The result.</returns>
		private static void GetAll(IList<StepInfo> result, in SudokuGrid grid, int size)
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
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="size">The size.</param>
		/// <param name="baseSet">The base set.</param>
		/// <param name="coverSet">The cover set.</param>
		/// <param name="a">(<see langword="in"/> parameter) The left grid map.</param>
		/// <param name="b">(<see langword="in"/> parameter) The right grid map.</param>
		/// <param name="c">(<see langword="in"/> parameter) The intersection.</param>
		private static void GetAll(
			IList<StepInfo> result, in SudokuGrid grid, int size, int baseSet, int coverSet,
			in GridMap a, in GridMap b, in GridMap c)
		{
			foreach (int[] cells in (a & EmptyMap).ToArray().GetSubsets(size - 1))
			{
				short mask = grid.BitwiseOrMasks(cells);
				if (mask.PopCount() != size)
				{
					continue;
				}

				static bool overlaps(int d, in int coverSet) => ValueMaps[d].Overlaps(RegionMaps[coverSet]);
				var digits = mask.GetAllSets();
				unsafe
				{
					if (digits.Any(&overlaps, coverSet))
					{
						continue;
					}
				}

				short ahsMask = 0;
				foreach (int digit in digits)
				{
					ahsMask |= (RegionMaps[coverSet] & CandMaps[digit] & b).GetSubviewMask(coverSet);
				}
				if (ahsMask.PopCount() != size - 1)
				{
					continue;
				}

				var ahsCells = GridMap.Empty;
				foreach (int pos in ahsMask)
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

					foreach (int digit in mask & grid.GetCandidateMask(aCell))
					{
						conclusions.Add(new(ConclusionType.Elimination, aCell, digit));
					}
				}
				foreach (int digit in SudokuGrid.MaxCandidatesMask & ~mask)
				{
					foreach (int ahsCell in ahsCells & CandMaps[digit])
					{
						conclusions.Add(new(ConclusionType.Elimination, ahsCell, digit));
					}
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				// Gather highlight candidates.
				var candidateOffsets = new List<DrawingInfo>();
				foreach (int digit in digits)
				{
					foreach (int cell in cellsMap & CandMaps[digit])
					{
						candidateOffsets.Add(new(0, cell * 9 + digit));
					}
				}
				foreach (int cell in c)
				{
					foreach (int digit in mask & grid.GetCandidateMask(cell))
					{
						candidateOffsets.Add(new(1, cell * 9 + digit));
					}
				}
				foreach (int cell in ahsCells)
				{
					foreach (int digit in mask & grid.GetCandidateMask(cell))
					{
						candidateOffsets.Add(new(0, cell * 9 + digit));
					}
				}

				var valueCells = from cell in (cellsMap | ahsCells) - EmptyMap select new DrawingInfo(0, cell);
				bool hasValueCell = valueCells.Any();
				result.Add(
					new AlcStepInfo(
						conclusions,
						new View[]
						{
							new()
							{
								Cells = hasValueCell ? valueCells.ToArray() : null,
								Candidates = candidateOffsets,
								Regions = new DrawingInfo[] { new(0, baseSet), new(1, coverSet) }
							}
						},
						mask,
						cellsMap,
						ahsCells,
						hasValueCell));
			}
		}
	}
}
