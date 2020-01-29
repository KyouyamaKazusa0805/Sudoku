using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;
using UrType1 = Sudoku.Solving.Manual.Uniqueness.Rectangles.UniqueRectangleType1DetailData;
using UrType2 = Sudoku.Solving.Manual.Uniqueness.Rectangles.UniqueRectangleType2DetailData;

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
					new[] { cells[2], cells[3] }, // 0, 1
					new[] { cells[1], cells[3] }, // 0, 2
					new[] { cells[1], cells[2] }, // 0, 3
					new[] { cells[0], cells[3] }, // 1, 2
					new[] { cells[0], cells[2] }, // 1, 3
					new[] { cells[0], cells[1] }, // 2, 3
				};

				// Traverse on 'cellTriplets'.
				// Check type 1.
				for (int i = 0; i < 4; i++)
				{
					int[] cellTriplet = cellTriplets[i];
					short totalMask = 511;
					foreach (int cell in cellTriplet)
					{
						totalMask &= grid.GetMask(cell);
					}

					// Check all different value kinds are no more than 2.
					if (totalMask.CountSet() != 7)
					{
						continue;
					}

					// Pattern found:
					// ab ab
					// ab ab+

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
						|| _checkIncompleted && elimCount == 0)
					{
						continue;
					}

					// Type 1.
					result.Add(
						new UniqueRectangleTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets,
									regionOffsets: null,
									linkMasks: null)
							},
							detailData: new UrType1(cells, digits.ToArray())));
				}

				// Traverse on 'cellPairs'.
				// Check type 2 / 4 / 5.
				for (int i = 0; i < 6; i++)
				{
					int[] cellPair = cellPairs[i];
					short cellPairMask = 511;
					foreach (int cell in cellPair)
					{
						cellPairMask &= grid.GetMask(cell);
					}

					if (cellPairMask.CountSet() != 7)
					{
						continue;
					}

					// Pattern found:
					// ab ab
					// ?  ?
					// or pattern:
					// ab ?
					// ?  ab
					int[] extraCells = i switch
					{
						0 => new[] { cells[0], cells[1] }, // Type 2.
						1 => new[] { cells[0], cells[2] }, // Type 2.
						2 => new[] { cells[0], cells[3] }, // Type 5.
						3 => new[] { cells[1], cells[2] }, // Type 5.
						4 => new[] { cells[1], cells[3] }, // Type 2.
						5 => new[] { cells[2], cells[3] }, // Type 2.
						_ => throw new Exception("Impossible case.")
					};

					short extraCellMask = 511;
					foreach (int cell in extraCells)
					{
						extraCellMask &= grid.GetMask(cell);
					}
					short totalMask = (short)(extraCellMask & cellPairMask);
					if (totalMask.CountSet() != 6)
					{
						continue;
					}

					// Type 2 / 5 found.
					// Record all highlight candidates.
					var candidateOffsets = new List<(int, int)>();
					var digits = (~grid.GetMask(cellPair[0]) & 511).GetAllSets();
					foreach (int cell in cellPair)
					{
						foreach (int digit in digits)
						{
							if (grid.CandidateExists(cell, digit))
							{
								candidateOffsets.Add((0, cell * 9 + digit));
							}
						}
					}
					foreach (int cell in extraCells)
					{
						foreach (int digit in digits)
						{
							if (grid.CandidateExists(cell, digit))
							{
								candidateOffsets.Add((1, cell * 9 + digit));
							}
						}
					}

					// Check whether elimination cells exist.
					var (a, b) = (extraCells[0], extraCells[1]);
					var elimMap = new GridMap(a) { [a] = false } & new GridMap(b) { [b] = false };
					if (elimMap.Count == 0)
					{
						continue;
					}

					// Record all eliminations.
					int extraDigit = (~totalMask & 511).GetAllSets().First(i => !digits.Contains(i));
					var conclusions = new List<Conclusion>();
					foreach (int cell in elimMap.Offsets)
					{
						if (grid.CandidateExists(cell, extraDigit))
						{
							conclusions.Add(
								new Conclusion(
									ConclusionType.Elimination, cell * 9 + extraDigit));
						}
					}

					int elimCount = conclusions.Count;
					if (!_checkIncompleted && candidateOffsets.Count != 8
						|| _checkIncompleted && elimCount == 0)
					{
						continue;
					}

					// Check if the type number is 2 or 5.
					bool isType5 = i switch
					{
						0 => false, 1 => false, 4 => false, 5 => false,
						2 => true, 3 => true,
						_ => throw new Exception("Impossible case.")
					};

					// Type 2 / 5.
					result.Add(
						new UniqueRectangleTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets,
									regionOffsets: null,
									linkMasks: null)
							},
							detailData: new UrType2(cells, digits.ToArray(), extraDigit, isType5)));
				}
			}

			return result;
		}
	}
}
