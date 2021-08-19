namespace Sudoku.Solving.Manual.Checkers;

/// <summary>
/// Defines a searcher that searches for the true candidates of the current sudoku grid.
/// </summary>
public sealed unsafe class TrueCandidatesSearcher
{
	/// <summary>
	/// Initializes an instance with the specified grid.
	/// </summary>
	/// <param name="grid">The current puzzle grid.</param>
	/// <exception cref="InvalidPuzzleException">Throws when the puzzle is invalid.</exception>
	public TrueCandidatesSearcher(in Grid grid) => Puzzle = grid.IsValid()
		? grid
		: throw new InvalidPuzzleException(grid, "the puzzle must contain unique solution before checking.");


	/// <summary>
	/// Indicates the current grid is a BUG+n pattern.
	/// </summary>
	public bool IsBugPattern => !TrueCandidates.IsEmpty;

	/// <summary>
	/// The grid.
	/// </summary>
	public Grid Puzzle { get; }

	/// <summary>
	/// Indicates all true candidates (non-BUG candidates).
	/// </summary>
	public Candidates TrueCandidates => GetAllTrueCandidates(81 - 17);


	/// <summary>
	/// Get all true candidates when the number of empty cells
	/// is below than the argument.
	/// </summary>
	/// <param name="maximumEmptyCells">The maximum number of the empty cells.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
	/// <returns>All true candidates.</returns>
	public Candidates GetAllTrueCandidates(int maximumEmptyCells, CancellationToken cancellationToken = default)
	{
		InitializeMaps(Puzzle);

		// Get the number of multivalue cells.
		// If the number of that is greater than the specified number,
		// here will return the default list directly.
		int multivalueCellsCount = 0;
		foreach (int value in EmptyMap)
		{
			switch (PopCount((uint)Puzzle.GetCandidates(value)))
			{
				case 1:
				case > 2 when ++multivalueCellsCount > maximumEmptyCells:
				{
					return Array.Empty<int>();
				}
			}
		}

		// Store all bivalue cells and construct the relations.
		int* peerRegions = stackalloc int[3];
		var stack = new Cells[multivalueCellsCount + 1, 9];
		foreach (int cell in BivalueMap)
		{
			foreach (int digit in Puzzle.GetCandidates(cell))
			{
				ref var map = ref stack[0, digit];
				map.AddAnyway(cell);

				peerRegions[0] = cell.ToRegion(RegionLabel.Row);
				peerRegions[1] = cell.ToRegion(RegionLabel.Column);
				peerRegions[2] = cell.ToRegion(RegionLabel.Block);
				for (int i = 0; i < 3; i++)
				{
					if ((map & RegionMaps[peerRegions[i]]).Count > 2)
					{
						// The specified region contains at least three positions to fill with the digit,
						// which is invalid in any BUG + n patterns.
						return Array.Empty<int>();
					}
				}
			}
		}

		// Store all multivalue cells.
		// Suppose the pattern is the simplest BUG + 1 pattern (i.e. Only one multi-value cell).
		// The comments will help you to understand the processing.
		short mask;
		short[,] pairs = new short[multivalueCellsCount, 37]; // 37 == (1 + 8) * 8 / 2 + 1
		int[] multivalueCells = (EmptyMap - BivalueMap).ToArray();
		for (int i = 0, length = multivalueCells.Length; i < length; i++)
		{
			// eg. { 2, 4, 6 } (42)
			mask = Puzzle.GetCandidates(multivalueCells[i]);

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
		var result = Candidates.Empty;
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
				int pos1 = TrailingZeroCount(*&mask);

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

	/// <summary>
	/// Get all true candidates when the number of empty cells
	/// is below than the argument asynchronously.
	/// </summary>
	/// <param name="maximumEmptyCells">The maximum number of the empty cells.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
	/// <returns>The task to get all true candidates.</returns>
	public ValueTask<Candidates> GetAllTrueCandidatesAsync(
		int maximumEmptyCells, CancellationToken cancellationToken = default) =>
		new(GetAllTrueCandidates(maximumEmptyCells, cancellationToken));
}
