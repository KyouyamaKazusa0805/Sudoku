using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Symmetry
{
	/// <summary>
	/// Encapsulates a Gurth's symmetrical placement technique searcher.
	/// </summary>
	public sealed partial class GurthSymmetricalPlacementTechniqueSearcher : SymmetryTechniqueSearcher
	{
		/// <inheritdoc/>
		public override int Priority => 0;


		/// <inheritdoc/>
		public override IReadOnlyList<TechniqueInfo> TakeAll(Grid grid)
		{
			var result = new List<GurthSymmetricalPlacementTechniqueInfo>();

			CheckCentral(result, grid);
			CheckDiagonal(result, grid);
			CheckAntiDiagonal(result, grid);

			// These validation is always redundant.
			// The grid having Gurth's symmetrical placements should not be
			// X-axis or Y-axis symmetry.
			//CheckX(result, grid);
			//CheckY(result, grid);

			return result;
		}


		/// <summary>
		/// Check x-axis symmetry.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		[Obsolete("The method is always redundant.")]
		[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
		private void CheckX(
			IList<GurthSymmetricalPlacementTechniqueInfo> result, Grid grid)
		{
			bool xHasEmptyCell = false;
			for (int i = 0; i < 9; i++)
			{
				if (grid.GetCellStatus(36 + i) == CellStatus.Empty)
				{
					xHasEmptyCell = true;
					break;
				}
			}
			if (!xHasEmptyCell)
			{
				return;
			}

			int?[] mapping = new int?[9];
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					int c1 = i * 9 + j;
					int c2 = (8 - i) * 9 + j;
					bool condition = grid.GetCellStatus(c1) == CellStatus.Empty;
					if (condition ^ grid.GetCellStatus(c2) == CellStatus.Empty)
					{
						return;
					}

					if (condition)
					{
						continue;
					}

					int d1 = grid[c1], d2 = grid[c2];
					if (d1 == d2)
					{
						int? o1 = mapping[d1];
						if (o1 is null)
						{
							mapping[d1] = d1;
							continue;
						}

						if (o1 != d1)
						{
							return;
						}
					}
					else
					{
						int? o1 = mapping[d1], o2 = mapping[d2];
						if (o1 is null ^ o2 is null)
						{
							return;
						}

						if (o1 is null && o2 is null)
						{
							mapping[d1] = d2;
							mapping[d2] = d1;
							continue;
						}

						// 'o1' and 'o2' are both not null.
						if (o1 != d2 || o2 != d1)
						{
							return;
						}
					}
				}
			}

			var singleDigitList = new List<int>();
			for (int digit = 0; digit < 9; digit++)
			{
				int? mappingDigit = mapping[digit];
				if (mappingDigit is null || mappingDigit == digit)
				{
					singleDigitList.Add(digit);
				}
			}

			var candidateOffsets = new List<(int, int)>();
			var conclusions = new List<Conclusion>();
			for (int i = 0; i < 9; i++)
			{
				int cell = 36 + i;
				if (grid.GetCellStatus(cell) != CellStatus.Empty)
				{
					continue;
				}

				foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
				{
					if (singleDigitList.Contains(digit))
					{
						candidateOffsets.Add((0, cell * 9 + digit));
						continue;
					}

					conclusions.Add(
						new Conclusion(ConclusionType.Elimination, cell, digit));
				}
			}

			if (conclusions.Count == 0)
			{
				return;
			}

			result.Add(
				new GurthSymmetricalPlacementTechniqueInfo(
					conclusions,
					views: new[]
					{
						new View(
							cellOffsets: null,
							candidateOffsets,
							regionOffsets: null,
							linkMasks: null)
					},
					symmetricalType: SymmetricalType.XAxis,
					mappingTable: mapping));
		}

		/// <summary>
		/// Check y-axis symmetry.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		[Obsolete("The method is always redundant.")]
		[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
		private void CheckY(
			IList<GurthSymmetricalPlacementTechniqueInfo> result, Grid grid)
		{
			bool yHasEmptyCell = false;
			for (int i = 0; i < 9; i++)
			{
				if (grid.GetCellStatus(i * 9 + 4) == CellStatus.Empty)
				{
					yHasEmptyCell = true;
					break;
				}
			}
			if (!yHasEmptyCell)
			{
				return;
			}

			int?[] mapping = new int?[9];
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					int c1 = i * 9 + j;
					int c2 = i * 9 + (8 - j);
					bool condition = grid.GetCellStatus(c1) == CellStatus.Empty;
					if (condition ^ grid.GetCellStatus(c2) == CellStatus.Empty)
					{
						return;
					}

					if (condition)
					{
						continue;
					}

					int d1 = grid[c1], d2 = grid[c2];
					if (d1 == d2)
					{
						int? o1 = mapping[d1];
						if (o1 is null)
						{
							mapping[d1] = d1;
							continue;
						}

						if (o1 != d1)
						{
							return;
						}
					}
					else
					{
						int? o1 = mapping[d1], o2 = mapping[d2];
						if (o1 is null ^ o2 is null)
						{
							return;
						}

						if (o1 is null && o2 is null)
						{
							mapping[d1] = d2;
							mapping[d2] = d1;
							continue;
						}

						// 'o1' and 'o2' are both not null.
						if (o1 != d2 || o2 != d1)
						{
							return;
						}
					}
				}
			}

			var singleDigitList = new List<int>();
			for (int digit = 0; digit < 9; digit++)
			{
				int? mappingDigit = mapping[digit];
				if (mappingDigit is null || mappingDigit == digit)
				{
					singleDigitList.Add(digit);
				}
			}

			var candidateOffsets = new List<(int, int)>();
			var conclusions = new List<Conclusion>();
			for (int i = 0; i < 9; i++)
			{
				int cell = i * 9 + 4;
				if (grid.GetCellStatus(cell) != CellStatus.Empty)
				{
					continue;
				}

				foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
				{
					if (singleDigitList.Contains(digit))
					{
						candidateOffsets.Add((0, cell * 9 + digit));
						continue;
					}

					conclusions.Add(
						new Conclusion(ConclusionType.Elimination, cell, digit));
				}
			}

			if (conclusions.Count == 0)
			{
				return;
			}

			result.Add(
				new GurthSymmetricalPlacementTechniqueInfo(
					conclusions,
					views: new[]
					{
						new View(
							cellOffsets: null,
							candidateOffsets,
							regionOffsets: null,
							linkMasks: null)
					},
					symmetricalType: SymmetricalType.YAxis,
					mappingTable: mapping));
		}

		/// <summary>
		/// Check diagonal symmetry.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		private void CheckDiagonal(
			IList<GurthSymmetricalPlacementTechniqueInfo> result, Grid grid)
		{
			bool diagonalHasEmptyCell = false;
			for (int i = 0; i < 9; i++)
			{
				if (grid.GetCellStatus(i * 9 + i) == CellStatus.Empty)
				{
					diagonalHasEmptyCell = true;
					break;
				}
			}
			if (!diagonalHasEmptyCell)
			{
				// No conclusion.
				return;
			}

			int?[] mapping = new int?[9];
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < i; j++)
				{
					int c1 = i * 9 + j;
					int c2 = j * 9 + i;
					bool condition = grid.GetCellStatus(c1) == CellStatus.Empty;
					if (condition ^ grid.GetCellStatus(c2) == CellStatus.Empty)
					{
						// One of two cells is empty. Not this symmetry.
						return;
					}

					if (condition)
					{
						continue;
					}

					int d1 = grid[c1], d2 = grid[c2];
					if (d1 == d2)
					{
						int? o1 = mapping[d1];
						if (o1 is null)
						{
							mapping[d1] = d1;
							continue;
						}

						if (o1 != d1)
						{
							return;
						}
					}
					else
					{
						int? o1 = mapping[d1], o2 = mapping[d2];
						if (o1 is null ^ o2 is null)
						{
							return;
						}

						if (o1 is null && o2 is null)
						{
							mapping[d1] = d2;
							mapping[d2] = d1;
							continue;
						}

						// 'o1' and 'o2' are both not null.
						if (o1 != d2 || o2 != d1)
						{
							return;
						}
					}
				}
			}

			var singleDigitList = new List<int>();
			for (int digit = 0; digit < 9; digit++)
			{
				int? mappingDigit = mapping[digit];
				if (mappingDigit is null || mappingDigit == digit)
				{
					singleDigitList.Add(digit);
				}
			}

			var candidateOffsets = new List<(int, int)>();
			var conclusions = new List<Conclusion>();
			for (int i = 0; i < 9; i++)
			{
				int cell = i * 9 + i;
				if (grid.GetCellStatus(cell) != CellStatus.Empty)
				{
					continue;
				}

				foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
				{
					if (singleDigitList.Contains(digit))
					{
						candidateOffsets.Add((0, cell * 9 + digit));
						continue;
					}

					conclusions.Add(
						new Conclusion(ConclusionType.Elimination, cell, digit));
				}
			}

			if (conclusions.Count == 0)
			{
				return;
			}

			result.Add(
				new GurthSymmetricalPlacementTechniqueInfo(
					conclusions,
					views: new[]
					{
						new View(
							cellOffsets: null,
							candidateOffsets,
							regionOffsets: null,
							linkMasks: null)
					},
					symmetricalType: SymmetricalType.Diagonal,
					mappingTable: mapping));
		}

		/// <summary>
		/// Check anti-diagonal symmetry.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		private void CheckAntiDiagonal(
			IList<GurthSymmetricalPlacementTechniqueInfo> result, Grid grid)
		{
			bool antiDiagonalHasEmptyCell = false;
			for (int i = 0; i < 9; i++)
			{
				if (grid.GetCellStatus(i * 9 + (8 - i)) == CellStatus.Empty)
				{
					antiDiagonalHasEmptyCell = true;
					break;
				}
			}
			if (!antiDiagonalHasEmptyCell)
			{
				// No conclusion.
				return;
			}

			int?[] mapping = new int?[9];
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 8 - i; j++)
				{
					int c1 = i * 9 + j;
					int c2 = (8 - j) * 9 + (8 - i);
					bool condition = grid.GetCellStatus(c1) == CellStatus.Empty;
					if (condition ^ grid.GetCellStatus(c2) == CellStatus.Empty)
					{
						// One of two cells is empty. Not this symmetry.
						return;
					}

					if (condition)
					{
						continue;
					}

					int d1 = grid[c1], d2 = grid[c2];
					if (d1 == d2)
					{
						int? o1 = mapping[d1];
						if (o1 is null)
						{
							mapping[d1] = d1;
							continue;
						}

						if (o1 != d1)
						{
							return;
						}
					}
					else
					{
						int? o1 = mapping[d1], o2 = mapping[d2];
						if (o1 is null ^ o2 is null)
						{
							return;
						}

						if (o1 is null && o2 is null)
						{
							mapping[d1] = d2;
							mapping[d2] = d1;
							continue;
						}

						// 'o1' and 'o2' are both not null.
						if (o1 != d2 || o2 != d1)
						{
							return;
						}
					}
				}
			}

			var singleDigitList = new List<int>();
			for (int digit = 0; digit < 9; digit++)
			{
				int? mappingDigit = mapping[digit];
				if (mappingDigit is null || mappingDigit == digit)
				{
					singleDigitList.Add(digit);
				}
			}

			var candidateOffsets = new List<(int, int)>();
			var conclusions = new List<Conclusion>();
			for (int i = 0; i < 9; i++)
			{
				int cell = i * 9 + (8 - i);
				if (grid.GetCellStatus(cell) != CellStatus.Empty)
				{
					continue;
				}

				foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
				{
					if (singleDigitList.Contains(digit))
					{
						candidateOffsets.Add((0, cell * 9 + digit));
						continue;
					}

					conclusions.Add(
						new Conclusion(ConclusionType.Elimination, cell, digit));
				}
			}

			if (conclusions.Count == 0)
			{
				return;
			}

			result.Add(
				new GurthSymmetricalPlacementTechniqueInfo(
					conclusions,
					views: new[]
					{
						new View(
							cellOffsets: null,
							candidateOffsets,
							regionOffsets: null,
							linkMasks: null)
					},
					symmetricalType: SymmetricalType.AntiDiagonal,
					mappingTable: mapping));
		}

		/// <summary>
		/// Check central symmetry.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		private static void CheckCentral(
			IList<GurthSymmetricalPlacementTechniqueInfo> result, Grid grid)
		{
			if (grid.GetCellStatus(40) != CellStatus.Empty)
			{
				// Has no conclusion even though the grid may be symmetrical.
				return;
			}

			int?[] mapping = new int?[9];
			for (int cell = 0; cell < 40; cell++)
			{
				int anotherCell = 80 - cell;
				bool condition = grid.GetCellStatus(cell) == CellStatus.Empty;
				if (condition ^ grid.GetCellStatus(anotherCell) == CellStatus.Empty)
				{
					// One of two cell is empty, not central symmetrical type.
					return;
				}

				if (condition)
				{
					continue;
				}

				int d1 = grid[cell], d2 = grid[anotherCell];
				if (d1 == d2)
				{
					int? o1 = mapping[d1];
					if (o1 is null)
					{
						mapping[d1] = d1;
						continue;
					}

					if (o1 != d1)
					{
						return;
					}
				}
				else
				{
					int? o1 = mapping[d1], o2 = mapping[d2];
					if (o1 is null ^ o2 is null)
					{
						return;
					}

					if (o1 is null && o2 is null)
					{
						mapping[d1] = d2;
						mapping[d2] = d1;
						continue;
					}

					// 'o1' and 'o2' are both not null.
					if (o1 != d2 || o2 != d1)
					{
						return;
					}
				}
			}

			for (int digit = 0; digit < 9; digit++)
			{
				if (mapping[digit] == digit || mapping[digit] is null)
				{
					result.Add(
						new GurthSymmetricalPlacementTechniqueInfo(
							conclusions: new[]
							{
								new Conclusion(ConclusionType.Assignment, 40, digit)
							},
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets: null,
									regionOffsets: null,
									linkMasks: null)
							},
							symmetricalType: SymmetricalType.Central,
							mappingTable: mapping));

					return;
				}
			}
		}
	}
}
