using System.Collections.Generic;
using System.Linq;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;
using UrType1 = Sudoku.Solving.Manual.Uniqueness.Rectangles.UniqueRectangleType1DetailData;

namespace Sudoku.Solving.Manual.Uniqueness.Rectangles
{
	/// <summary>
	/// Encapsulates a unique rectangle technique searcher.
	/// </summary>
	public sealed partial class UniqueRectangleTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <summary>
		/// Indicates whether the solver should check incompleted URs.
		/// </summary>
		private readonly bool _checkIncompleted;


		/// <summary>
		/// Initializes an instance with the checking option.
		/// </summary>
		/// <param name="checkIncompletedUniquenessPatterns">
		/// Indicates whether the solver should check incompleted URs.
		/// </param>
		public UniqueRectangleTechniqueSearcher(bool checkIncompletedUniquenessPatterns) =>
			_checkIncompleted = checkIncompletedUniquenessPatterns;


		/// <inheritdoc/>
		public override IReadOnlyList<TechniqueInfo> TakeAll(Grid grid)
		{
			var result = new List<UniquenessTechniqueInfo>();

			foreach (int[] cells in TraversingTable)
			{
				if (cells.Any(c => grid.GetCellStatus(c) != CellStatus.Empty))
				{
					continue;
				}

				var cellTriplets = new int[4][]
				{
					new[] { cells[1], cells[2], cells[3] }, // 0
					new[] { cells[0], cells[2], cells[3] }, // 1
					new[] { cells[0], cells[1], cells[3] }, // 2
					new[] { cells[0], cells[1], cells[2] }, // 3
				};
				var cellPairs = new int[6][]
				{
					new[] { cells[0], cells[1] }, // 2, 3
					new[] { cells[0], cells[2] }, // 1, 3
					new[] { cells[0], cells[3] }, // 1, 2
					new[] { cells[1], cells[2] }, // 0, 3
					new[] { cells[1], cells[3] }, // 0, 2
					new[] { cells[2], cells[3] }, // 0, 1
				};

				// Traverse on 'cellTriplets'.
				// Check type 1.
				for (int i = 0; i < 4; i++)
				{
					int[] cellTriplet = cellTriplets[i];
					short mask = 511;
					foreach (int cell in cellTriplet)
					{
						mask &= grid.GetMask(cell);
					}

					if (mask.CountSet() == 7)
					{
						// Pattern found:
						// a(b) (a)b
						// (a)b a(b)+

						// The index is 'i', which also represents the index of the extra cell.
						int extraCell = cells[i];

						// Record all highlight candidates.
						var candidateOffsets = new List<(int, int)>();
						var digits = (~grid.GetMask(cellTriplet[0]) & 511).GetAllSets();
						foreach (int cell in cellTriplet)
						{
							foreach (int digit in digits)
							{
								if (grid.CandidateExists(cell, digit))
								{
									candidateOffsets.Add((0, cell * 9 + digit));
								}
							}
						}

						// Record all eliminations.
						var conclusions = new List<Conclusion>();
						foreach (int digit in (~grid.GetMask(extraCell) & 511).GetAllSets())
						{
							if (grid.CandidateExists(extraCell, digit) && digits.Contains(digit))
							{
								conclusions.Add(
									new Conclusion(
										ConclusionType.Elimination, extraCell * 9 + digit));
							}
						}

						// Check the number of candidates and eliminations.
						int elimCount = conclusions.Count;
						if (!_checkIncompleted && (candidateOffsets.Count != 6 || elimCount != 2)
							|| _checkIncompleted && elimCount != 0)
						{
							continue;
						}

						// Type 1 found.
						result.Add(
							new UniqueRectangleTechniqueInfo(
								conclusions,
								views: new List<View>
								{
									new View(
										cellOffsets: null,
										candidateOffsets,
										regionOffsets: null,
										linkMasks: null)
								},
								detailData: new UrType1(cells, digits.ToArray())));
					}
				}

				// Traverse on 'cellPairs'.
				// Check type 2.
				//for (int i = 0; i < 6; i++)
				//{
				//	int[] cellPair = cellPairs[i];
				//
				//}
			}

			return result;
		}
	}
}
