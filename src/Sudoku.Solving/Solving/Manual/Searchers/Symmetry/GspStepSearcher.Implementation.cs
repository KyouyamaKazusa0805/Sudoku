namespace Sudoku.Solving.Manual.Symmetry;

partial class GspStepSearcher
{
	/// <summary>
	/// Check diagonal symmetry.
	/// </summary>
	/// <param name="result">The result accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="info">The step information result.</param>
	private static unsafe partial void CheckDiagonal(
		IList<Conclusion> result, in SudokuGrid grid, out GspStepInfo? info)
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

		var mapping = (stackalloc int?[9]);
		mapping.Fill(null);
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < i; j++)
			{
				int c1 = i * 9 + j, c2 = j * 9 + i;
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
					if (o1 is not null ^ o2 is not null)
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

		var candidateOffsets = new List<DrawingInfo>();
		var conclusions = new List<Conclusion>();
		for (int i = 0; i < 9; i++)
		{
			int cell = i * 9 + i;
			if (grid.GetStatus(cell) != CellStatus.Empty)
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
			mapping.ToArray()
		);
	}

	/// <summary>
	/// Check anti-diagonal symmetry.
	/// </summary>
	/// <param name="result">The result accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="info">The step information result.</param>
	private static unsafe partial void CheckAntiDiagonal(
		IList<Conclusion> result, in SudokuGrid grid, out GspStepInfo? info)
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

		var mapping = (stackalloc int?[9]);
		mapping.Fill(null);
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0, iterationLength = 8 - i; j < iterationLength; j++)
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
					if (o1 is not null ^ o2 is not null)
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

		var candidateOffsets = new List<DrawingInfo>();
		var conclusions = new List<Conclusion>();
		for (int i = 0; i < 9; i++)
		{
			int cell = i * 9 + (8 - i);
			if (grid.GetStatus(cell) != CellStatus.Empty)
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
			mapping.ToArray()
		);
	}

	/// <summary>
	/// Check central symmetry.
	/// </summary>
	/// <param name="result">The result accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="info">The step information result.</param>
	private static unsafe partial void CheckCentral(
		IList<Conclusion> result, in SudokuGrid grid, out GspStepInfo? info)
	{
		info = null;

		if (grid.GetStatus(40) != CellStatus.Empty)
		{
			// Has no conclusion even though the grid may be symmetrical.
			return;
		}

		var mapping = (stackalloc int?[9]);
		mapping.Fill(null);
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
				if (o1 is not null ^ o2 is not null)
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
			if (mapping[digit] is null || mapping[digit] == digit)
			{
				var conclusion = new Conclusion(ConclusionType.Assignment, 40, digit);
				result.Add(conclusion);

				info = new(
					new[] { conclusion },
					new View[] { new() { Candidates = new DrawingInfo[] { new(0, 360 + digit) } } },
					SymmetryType.Central,
					mapping.ToArray()
				);

				return;
			}
		}
	}
}
