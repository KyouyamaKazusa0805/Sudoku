namespace Sudoku.Solving.Logical.StepSearchers;

/// <summary>
/// Provides with a <b>Bi-value Universal Grave</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Basic types:
/// <list type="bullet">
/// <item>Bi-value Universal Grave Type 1</item>
/// <item>Bi-value Universal Grave Type 2</item>
/// <item>Bi-value Universal Grave Type 3</item>
/// <item>Bi-value Universal Grave Type 4</item>
/// </list>
/// </item>
/// <item>
/// Extended types:
/// <list type="bullet">
/// <item>Bi-value Universal Grave + n</item>
/// <item>Bi-value Universal Grave XZ</item>
/// </list>
/// </item>
/// <item>Bi-value Universal Grave False Candidate Type</item>
/// </list>
/// </summary>
public interface IBivalueUniversalGraveStepSearcher : IUniversalStepSearcher
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
	/// <exception cref="InvalidOperationException">
	/// Throws when the puzzle contains multiple solutions or even no solution.
	/// </exception>
	protected internal static unsafe bool FindTrueCandidates(
		scoped in Grid grid,
		[NotNullWhen(true)] out IReadOnlyList<int>? trueCandidates,
		int maximumCellsToCheck = 20
	)
	{
		Argument.ThrowIfInvalid(grid.IsValid(), "The puzzle must be valid (containing a unique solution).");

		InitializeMaps(grid);

		// Get the number of multi-value cells.
		// If the number of that is greater than the specified number,
		// here will return the default list directly.
		var multivalueCellsCount = 0;
		foreach (var value in EmptyCells)
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

		// Store all bi-value cells and construct the relations.
		scoped var span = (stackalloc int[3]);
		var stack = new CellMap[multivalueCellsCount + 1, 9];
		foreach (var cell in BivalueCells)
		{
			foreach (var digit in grid.GetCandidates(cell))
			{
				scoped ref var map = ref stack[0, digit];
				map.Add(cell);

				cell.CopyHouseInfo(ref span[0]);
				foreach (var house in span)
				{
					if ((map & HousesMap[house]).Count > 2)
					{
						// The specified house contains at least three positions to fill with the digit,
						// which is invalid in any BUG + n patterns.
						goto ReturnFalse;
					}
				}
			}
		}

		// Store all multi-value cells.
		// Suppose the pattern is the simplest BUG + 1 pattern (i.e. Only one multi-value cell).
		// The comments will help you to understand the processing.
		SkipInit(out short mask);
		var pairs = new short[multivalueCellsCount, 37]; // 37 == (1 + 8) * 8 / 2 + 1
		var multivalueCells = (EmptyCells - BivalueCells).ToArray();
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
		var currentIndex = 1;
		var chosen = new int[multivalueCellsCount + 1];
		var resultMap = new CellMap[9];
		var result = new List<int>();
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
					foreach (var house in playground)
					{
						if ((temp & HousesMap[house]).Count > 2)
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
						map = CandidatesMap[digit] - stack[currentIndex, digit];
						foreach (var cell in map)
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

	/// <summary>
	/// Checks whether the specified grid forms a BUG deadly pattern.
	/// This method does not use the cached buffers in type <see cref="FastProperties"/>.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	/// <seealso cref="FastProperties"/>
	protected internal static bool FormsPattern(scoped in Grid grid)
	{
		_ = grid is { BivalueCells: var bivalueCells, EmptyCells: var emptyCells, CandidatesMap: var candidatesMap };
		if (bivalueCells != emptyCells)
		{
			return false;
		}

		var housesCount = (stackalloc int[3]);
		foreach (var cell in emptyCells)
		{
			foreach (var digit in grid.GetCandidates(cell))
			{
				housesCount.Clear();

				for (var i = 0; i < 3; i++)
				{
					housesCount[i] = (candidatesMap[digit] & HousesMap[cell.ToHouseIndex((HouseType)i)]).Count;
				}

				if (housesCount is not [2, 2, 2])
				{
					return false;
				}
			}
		}

		return true;
	}
}
