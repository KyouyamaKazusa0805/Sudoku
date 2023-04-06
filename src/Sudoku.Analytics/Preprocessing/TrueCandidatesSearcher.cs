namespace Sudoku.Preprocessing;

/// <summary>
/// Defines a searcher that searches for the true candidates of the current sudoku grid.
/// </summary>
public static class TrueCandidatesSearcher
{
	/// <summary>
	/// Get all true candidates when the number of empty cells
	/// is below than the argument.
	/// </summary>
	/// <param name="grid">Indicates the puzzle.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
	/// <returns>All true candidates.</returns>
	/// <exception cref="ArgumentException">Throws when the puzzle is invalid.</exception>
	public static unsafe CandidateMap GetAllTrueCandidates(scoped in Grid grid, CancellationToken cancellationToken = default)
	{
		if (!grid.IsValid)
		{
			throw new ArgumentException("The puzzle must be valid.", nameof(grid));
		}

		// Get the number of multi-value cells.
		// If the number of that is greater than the specified number, here will return the default list directly.
		var multivalueCellsCount = 0;
		foreach (var cell in grid.EmptyCells)
		{
			switch (PopCount((uint)grid.GetCandidates(cell)))
			{
				case 1:
				{
					return CandidateMap.Empty;
				}
				case >= 3:
				{
					multivalueCellsCount++;
					break;
				}
			}
		}

		// Store all bi-value cells and construct the relations.
		scoped var peerHouses = (stackalloc int[3]);
		var stack = new CellMap[multivalueCellsCount + 1, 9];
		foreach (var cell in grid.BivalueCells)
		{
			foreach (var digit in grid.GetCandidates(cell))
			{
				scoped ref var map = ref stack[0, digit];
				map.Add(cell);

				cell.CopyHouseInfo(ref peerHouses[0]);
				for (var i = 0; i < 3; i++)
				{
					if ((map & HousesMap[peerHouses[i]]).Count > 2)
					{
						// The specified house contains at least three positions to fill with the digit,
						// which is invalid in any BUG + n patterns.
						return CandidateMap.Empty;
					}
				}
			}
		}

		// Store all multi-value cells.
		// Suppose the pattern is the simplest BUG + 1 pattern (i.e. Only one multi-value cell).
		// The comments will help you to understand the processing.
		SkipInit(out short mask);
		var pairs = new short[multivalueCellsCount, (1 + 8) * 8 / 2 + 1];
		var multivalueCells = (grid.EmptyCells - grid.BivalueCells).ToArray();
		for (var i = 0; i < multivalueCells.Length; i++)
		{
			// e.g. { 2, 4, 6 } (42)
			mask = grid.GetCandidates(multivalueCells[i]);

			// e.g. { 2, 4 }, { 4, 6 }, { 2, 6 } (10, 40, 34)
			var pairList = MaskOperations.GetMaskSubsets(mask, 2);

			// e.g. pairs[i, ..] = { 3, { 2, 4 }, { 4, 6 }, { 2, 6 } } ({ 3, 10, 40, 34 })
			pairs[i, 0] = (short)pairList.Length;
			for (var z = 1; z <= pairList.Length; z++)
			{
				pairs[i, z] = pairList[z - 1];
			}
		}

		// Now check the pattern.
		// If the pattern is a valid BUG + n, the processing here will give you one plan of all possible
		// combinations; otherwise, none will be found.
		scoped var playground = (stackalloc int[3]);
		scoped var chosen = (stackalloc int[multivalueCellsCount + 1]);
		scoped var resultMap = (stackalloc CellMap[9]);
		var currentIndex = 1;
		var result = CandidateMap.Empty;
		do
		{
			int i;
			var currentCell = multivalueCells[currentIndex - 1];
			var @continue = false;
			for (i = chosen[currentIndex] + 1; i <= pairs[currentIndex - 1, 0]; i++)
			{
				@continue = true;
				mask = pairs[currentIndex - 1, i];
				foreach (var digit in pairs[currentIndex - 1, i])
				{
					var temp = stack[currentIndex - 1, digit];
					temp.Add(currentCell);

					currentCell.CopyHouseInfo(ref playground[0]);
					foreach (var houseIndex in playground)
					{
						if ((temp & HousesMap[houseIndex]).Count > 2)
						{
							@continue = false;
							break;
						}
					}

					if (!@continue) { break; }
				}

				if (@continue) { break; }
			}

			if (@continue)
			{
				for (var z = 0; z < stack.GetLength(1); z++)
				{
					stack[currentIndex, z] = stack[currentIndex - 1, z];
				}

				chosen[currentIndex] = i;
				var pos1 = TrailingZeroCount(mask);

				stack[currentIndex, pos1].Add(currentCell);
				stack[currentIndex, mask.GetNextSet(pos1)].Add(currentCell);
				if (currentIndex == multivalueCellsCount)
				{
					// Iterate on each digit.
					for (var digit = 0; digit < 9; digit++)
					{
						// Take the cell that doesn't contain in the map above.
						// Here, the cell is the "true candidate cell".
						scoped ref var map = ref resultMap[digit];
						map = grid.CandidatesMap[digit] - stack[currentIndex, digit];
						foreach (var cell in map)
						{
							result.Add(cell * 9 + digit);
						}
					}

					return result;
				}
				else
				{
					currentIndex++;
				}
			}
			else
			{
				chosen[currentIndex--] = 0;
			}
		} while (currentIndex > 0);

		return result;
	}
}
