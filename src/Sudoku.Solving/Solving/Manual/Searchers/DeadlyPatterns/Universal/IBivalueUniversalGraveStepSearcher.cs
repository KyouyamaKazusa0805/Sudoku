namespace Sudoku.Solving.Manual.Searchers.DeadlyPatterns.Universal;

/// <summary>
/// Defines a step searcher that searches for bi-value universal grave steps.
/// </summary>
public unsafe interface IBivalueUniversalGraveStepSearcher : IUniversalStepSearcher
{
	/// <summary>
	/// Indicates whether the searcher should call the extended BUG checker
	/// to search for all true candidates no matter how difficult searching.
	/// </summary>
	bool SearchExtendedTypes { get; set; }


	/// <summary>
	/// Finds all possible true candidates in this current grid.
	/// </summary>
	/// <param name="grid">The grid to find all possible true candidates.</param>
	/// <param name="trueCandidates">All possible true candidates returned.</param>
	/// <param name="maximumCellsToCheck">Indicates the maximum number of possible cells to check.</param>
	/// <returns>
	/// A <see cref="bool"/> value indicating whether the grid contains any possible true candidates.
	/// If so, the argument <paramref name="trueCandidates"/> won't be <see langword="null"/>.
	/// </returns>
	/// <remarks>
	/// A <b>true candidate</b> is a candidate that makes the puzzle containing no valid solution
	/// if it is eliminated from the current puzzle. It is a strict concept, which means sometimes the puzzle
	/// doesn't contain any satisfied candidate to make the puzzle containing no solution if being eliminated.
	/// </remarks>
	/// <exception cref="InvalidPuzzleException">
	/// Throws when the puzzle contains multiple solutions or even no solution.
	/// </exception>
	public static bool FindTrueCandidates(
		in Grid grid,
		[NotNullWhen(true)] out IReadOnlyList<int>? trueCandidates,
		int maximumCellsToCheck = 20
	)
	{
		if (!grid.IsValid())
		{
			throw new InvalidPuzzleException(grid, "the puzzle must be valid (containing a unique solution).");
		}

		InitializeMaps(grid);

		// Get the number of multivalue cells.
		// If the number of that is greater than the specified number,
		// here will return the default list directly.
		int multivalueCellsCount = 0;
		foreach (int value in EmptyMap)
		{
			switch (PopCount((uint)grid.GetCandidates(value)))
			{
				case 1: // The grid contains a naked single. We don't check on this grid now.
				case > 2 when ++multivalueCellsCount > maximumCellsToCheck:
				{
					goto ReturnFalse;
				}
			}
		}

		// Store all bivalue cells and construct the relations.
		var span = (stackalloc int[3]);
		var stack = new Cells[multivalueCellsCount + 1, 9];
		foreach (int cell in BivalueMap)
		{
			foreach (int digit in grid.GetCandidates(cell))
			{
				ref var map = ref stack[0, digit];
				map.AddAnyway(cell);

				span[0] = cell.ToRegion(RegionLabel.Row);
				span[1] = cell.ToRegion(RegionLabel.Column);
				span[2] = cell.ToRegion(RegionLabel.Block);
				foreach (int region in span)
				{
					if ((map & RegionMaps[region]).Count > 2)
					{
						// The specified region contains at least three positions to fill with the digit,
						// which is invalid in any BUG + n patterns.
						goto ReturnFalse;
					}
				}
			}
		}

		// Store all multivalue cells.
		// Suppose the pattern is the simplest BUG + 1 pattern (i.e. Only one multi-value cell).
		// The comments will help you to understand the processing.
		Unsafe.SkipInit(out short mask);
		short[,] pairs = new short[multivalueCellsCount, 37]; // 37 == (1 + 8) * 8 / 2 + 1
		int[] multivalueCells = (EmptyMap - BivalueMap).ToArray();
		for (int i = 0, length = multivalueCells.Length; i < length; i++)
		{
			// eg. { 2, 4, 6 } (42)
			mask = grid.GetCandidates(multivalueCells[i]);

			// eg. { 2, 4 }, { 4, 6 }, { 2, 6 } (10, 40, 34)
			short[] pairList = MaskMarshal.GetMaskSubsets(mask, 2);

			// eg. pairs[i, ..] = { 3, { 2, 4 }, { 4, 6 }, { 2, 6 } } ({ 3, 10, 40, 34 })
			pairs[i, 0] = (short)pairList.Length;
			for (int z = 1, pairListLength = pairList.Length; z <= pairListLength; z++)
			{
				pairs[i, z] = pairList[z - 1];
			}
		}

		// Now check the pattern.
		// If the pattern is a valid BUG + n, the processing here will give you one plan of all possible
		// combinations; otherwise, none will be found.
		var playground = (stackalloc int[3]);
		int currentIndex = 1;
		int[] chosen = new int[multivalueCellsCount + 1];
		var resultMap = new Cells[9];
		var result = new List<int>();
		do
		{
			int i;
			int currentCell = multivalueCells[currentIndex - 1];
			bool @continue = false;
			for (i = chosen[currentIndex] + 1; i <= pairs[currentIndex - 1, 0]; i++)
			{
				@continue = true;
				mask = pairs[currentIndex - 1, i];
				foreach (int digit in pairs[currentIndex - 1, i])
				{
					var temp = stack[currentIndex - 1, digit];
					temp.AddAnyway(currentCell);

					playground[0] = currentCell.ToRegion(RegionLabel.Block);
					playground[1] = currentCell.ToRegion(RegionLabel.Row);
					playground[2] = currentCell.ToRegion(RegionLabel.Column);
					foreach (int region in playground)
					{
						if ((temp & RegionMaps[region]).Count > 2)
						{
							@continue = false;
							break;
						}
					}

					if (!@continue) break;
				}

				if (@continue) break;
			}

			if (@continue)
			{
				for (int z = 0, stackLength = stack.GetLength(1); z < stackLength; z++)
				{
					stack[currentIndex, z] = stack[currentIndex - 1, z];
				}

				chosen[currentIndex] = i;
				int pos1 = TrailingZeroCount(mask);

				stack[currentIndex, pos1].AddAnyway(currentCell);
				stack[currentIndex, mask.GetNextSet(pos1)].AddAnyway(currentCell);
				if (currentIndex == multivalueCellsCount)
				{
					// Iterate on each digit.
					for (int digit = 0; digit < 9; digit++)
					{
						// Take the cell that doesn't contain in the map above.
						// Here, the cell is the "true candidate cell".
						ref var map = ref resultMap[digit];
						map = CandMaps[digit] - stack[currentIndex, digit];
						foreach (int cell in map)
						{
							result.Add(cell * 9 + digit);
						}
					}

					goto ReturnTrue;
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

	ReturnTrue:
		trueCandidates = result;
		return true;

	ReturnFalse:
		trueCandidates = null;
		return false;
	}
}
