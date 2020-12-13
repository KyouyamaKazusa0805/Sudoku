using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Solving.Annotations;

namespace Sudoku.Solving.Manual.Symmetry
{
	/// <summary>
	/// Encapsulates a <b>Gurth's symmetrical placement</b> (GSP) technique searcher.
	/// </summary>
	public sealed class GspStepSearcher : SymmetryStepSearcher
	{
		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(default, nameof(TechniqueCode.Gsp))
		{
			IsReadOnly = true
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			// To verify all kinds of symmetry.
			var conclusions = new List<Conclusion>();
			CheckDiagonal(conclusions, grid, out var diagonalInfo);
			CheckAntiDiagonal(conclusions, grid, out var antidiagonalInfo);
			CheckCentral(conclusions, grid, out var centralInfo);

			if (conclusions.Count == 0)
			{
				return;
			}

			accumulator.Add((diagonalInfo | antidiagonalInfo | centralInfo)!);
		}

		/// <summary>
		/// Check diagonal symmetry.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="info">(<see langword="out"/> parameter) The step information result.</param>
		private static void CheckDiagonal(IList<Conclusion> result, in SudokuGrid grid, out GspStepInfo? info)
		{
			info = null;

			bool diagonalHasEmptyCell = false;
			for (int i = 0; i < 9; i++)
			{
				if (grid.GetStatus(i * 9 + i) == CellStatus.Empty)
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
					bool condition = grid.GetStatus(c1) == CellStatus.Empty;
					if (condition ^ grid.GetStatus(c2) == CellStatus.Empty)
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
						if (!o1.HasValue)
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
						if (o1.HasValue ^ o2.HasValue)
						{
							return;
						}

						if (!o1.HasValue && !o2.HasValue)
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
				if (!mappingDigit.HasValue || mappingDigit == digit)
				{
					singleDigitList.Add(digit);
				}
			}

			var candidateOffsets = new List<DrawingInfo>();
			var conclusions = new List<Conclusion>();
			for (int i = 0; i < 9; i++)
			{
				int cell = i * 9 + i;
				if (grid.GetStatus(cell) != CellStatus.Empty)
				{
					continue;
				}

				foreach (int digit in grid.GetCandidateMask(cell))
				{
					if (singleDigitList.Contains(digit))
					{
						candidateOffsets.Add(new(0, cell * 9 + digit));
						continue;
					}

					conclusions.Add(new(ConclusionType.Elimination, cell, digit));
				}
			}

			if (conclusions.Count == 0)
			{
				return;
			}

			result.AddRange(conclusions);
			info = new(
				conclusions,
				new View[] { new() { Candidates = candidateOffsets } },
				SymmetryType.Diagonal,
				mapping);
		}

		/// <summary>
		/// Check anti-diagonal symmetry.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="info">(<see langword="out"/> parameter) The step information result.</param>
		private static void CheckAntiDiagonal(IList<Conclusion> result, in SudokuGrid grid, out GspStepInfo? info)
		{
			info = null;

			bool antiDiagonalHasEmptyCell = false;
			for (int i = 0; i < 9; i++)
			{
				if (grid.GetStatus(i * 9 + (8 - i)) == CellStatus.Empty)
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
					bool condition = grid.GetStatus(c1) == CellStatus.Empty;
					if (condition ^ grid.GetStatus(c2) == CellStatus.Empty)
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
						if (!o1.HasValue)
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
						if (o1.HasValue ^ o2.HasValue)
						{
							return;
						}

						if (!o1.HasValue && !o2.HasValue)
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
				if (!mappingDigit.HasValue || mappingDigit == digit)
				{
					singleDigitList.Add(digit);
				}
			}

			var candidateOffsets = new List<DrawingInfo>();
			var conclusions = new List<Conclusion>();
			for (int i = 0; i < 9; i++)
			{
				int cell = i * 9 + (8 - i);
				if (grid.GetStatus(cell) != CellStatus.Empty)
				{
					continue;
				}

				foreach (int digit in grid.GetCandidateMask(cell))
				{
					if (singleDigitList.Contains(digit))
					{
						candidateOffsets.Add(new(0, cell * 9 + digit));
						continue;
					}

					conclusions.Add(new(ConclusionType.Elimination, cell, digit));
				}
			}

			if (conclusions.Count == 0)
			{
				return;
			}

			result.AddRange(conclusions);
			info = new(
				conclusions,
				new View[] { new() { Candidates = candidateOffsets } },
				SymmetryType.AntiDiagonal,
				mapping);
		}

		/// <summary>
		/// Check central symmetry.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="info">(<see langword="out"/> parameter) The step information result.</param>
		private static void CheckCentral(IList<Conclusion> result, in SudokuGrid grid, out GspStepInfo? info)
		{
			info = null;

			if (grid.GetStatus(40) != CellStatus.Empty)
			{
				// Has no conclusion even though the grid may be symmetrical.
				return;
			}

			int?[] mapping = new int?[9];
			for (int cell = 0; cell < 40; cell++)
			{
				int anotherCell = 80 - cell;
				bool condition = grid.GetStatus(cell) == CellStatus.Empty;
				if (condition ^ grid.GetStatus(anotherCell) == CellStatus.Empty)
				{
					// One of two cell is empty, not central symmetry type.
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
					if (!o1.HasValue)
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
					if (o1.HasValue ^ o2.HasValue)
					{
						return;
					}

					if (!o1.HasValue && !o2.HasValue)
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
				if (mapping[digit] == digit || !mapping[digit].HasValue)
				{
					var conclusion = new Conclusion(ConclusionType.Assignment, 40, digit);
					result.Add(conclusion);

					info = new(
						new[] { conclusion },
						new View[] { new() { Candidates = new DrawingInfo[] { new(0, 360 + digit) } } },
						SymmetryType.Central,
						mapping);

					return;
				}
			}
		}
	}
}
