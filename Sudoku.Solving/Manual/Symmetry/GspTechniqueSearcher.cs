using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Solving.Annotations;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Symmetry
{
	/// <summary>
	/// Encapsulates a <b>Gurth's symmetrical placement</b> (GSP) technique searcher.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.Gsp))]
	public sealed class GspTechniqueSearcher : SymmetryTechniqueSearcher
	{
		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(default) { IsReadOnly = true };


		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, Grid grid)
		{
			// To verify all kinds of symmetry.
			// Note that Gurth's symmetrical placement doesn't have X-axis and Y-axis type.
			unsafe
			{
				foreach (var act in new delegate*<IList<TechniqueInfo>, Grid, void>[]
				{
					&CheckCentral, &CheckDiagonal, &CheckAntiDiagonal
				})
				{
					act(accumulator, grid);
				}
			}
		}

		/// <summary>
		/// Check diagonal symmetry.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		private static void CheckDiagonal(IList<TechniqueInfo> result, Grid grid)
		{
			bool diagonalHasEmptyCell = false;
			for (int i = 0; i < 9; i++)
			{
				if (grid.GetStatus(i * 9 + i) == Empty)
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
					bool condition = grid.GetStatus(c1) == Empty;
					if (condition ^ grid.GetStatus(c2) == Empty)
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
						if ((o1, o2) is (null, not null) or (not null, null))
						{
							return;
						}

						if ((o1, o2) is (null, null))
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
				if (grid.GetStatus(cell) != Empty)
				{
					continue;
				}

				foreach (int digit in grid.GetCandidates(cell))
				{
					if (singleDigitList.Contains(digit))
					{
						candidateOffsets.Add(new(0, cell * 9 + digit));
						continue;
					}

					conclusions.Add(new(Elimination, cell, digit));
				}
			}

			if (conclusions.Count == 0)
			{
				return;
			}

			result.Add(
				new GspTechniqueInfo(
					conclusions,
					new View[] { new(candidateOffsets) },
					SymmetryType.Diagonal,
					mapping));
		}

		/// <summary>
		/// Check anti-diagonal symmetry.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		private static void CheckAntiDiagonal(IList<TechniqueInfo> result, Grid grid)
		{
			bool antiDiagonalHasEmptyCell = false;
			for (int i = 0; i < 9; i++)
			{
				if (grid.GetStatus(i * 9 + (8 - i)) == Empty)
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
					bool condition = grid.GetStatus(c1) == Empty;
					if (condition ^ grid.GetStatus(c2) == Empty)
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
						if ((o1, o2) is (null, not null) or (not null, null))
						{
							return;
						}

						if ((o1, o2) is (null, null))
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
				if (grid.GetStatus(cell) != Empty)
				{
					continue;
				}

				foreach (int digit in grid.GetCandidates(cell))
				{
					if (singleDigitList.Contains(digit))
					{
						candidateOffsets.Add(new(0, cell * 9 + digit));
						continue;
					}

					conclusions.Add(new(Elimination, cell, digit));
				}
			}

			if (conclusions.Count == 0)
			{
				return;
			}

			result.Add(
				new GspTechniqueInfo(
					conclusions,
					new View[] { new(candidateOffsets) },
					SymmetryType.AntiDiagonal,
					mapping));
		}

		/// <summary>
		/// Check central symmetry.
		/// </summary>
		/// <param name="result">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		private static void CheckCentral(IList<TechniqueInfo> result, Grid grid)
		{
			if (grid.GetStatus(40) != Empty)
			{
				// Has no conclusion even though the grid may be symmetrical.
				return;
			}

			int?[] mapping = new int?[9];
			for (int cell = 0; cell < 40; cell++)
			{
				int anotherCell = 80 - cell;
				bool condition = grid.GetStatus(cell) == Empty;
				if (condition ^ grid.GetStatus(anotherCell) == Empty)
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
					if ((o1, o2) is (null, not null) or (not null, null))
					{
						return;
					}

					if ((o1, o2) is (null, null))
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
						new GspTechniqueInfo(
							new Conclusion[] { new(Assignment, 40, digit) },
							new View[] { new(new DrawingInfo[] { new(0, 360 + digit) }) },
							SymmetryType.Central,
							mapping));

					return;
				}
			}
		}
	}
}
