namespace Sudoku.Solving.Manual.Searchers;

[StepSearcher]
[StepSearcherOptions(IsDirect = true, IsOptionsFixed = true)]
internal sealed unsafe partial class SingleStepSearcher : ISingleStepSearcher
{
	/// <inheritdoc/>
	public bool EnableFullHouse { get; set; }

	/// <inheritdoc/>
	public bool EnableLastDigit { get; set; }

	/// <inheritdoc/>
	public bool HiddenSinglesInBlockFirst { get; set; }


	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		if (!EnableFullHouse)
		{
			goto CheckHiddenSingle;
		}

		for (int house = 0; house < 27; house++)
		{
			int count = 0, resultCell = -1;
			bool flag = true;
			foreach (int cell in HouseMaps[house])
			{
				if (grid.GetStatus(cell) == CellStatus.Empty)
				{
					resultCell = cell;
					if (++count > 1)
					{
						flag = false;
						break;
					}
				}
			}
			if (!flag || count == 0)
			{
				continue;
			}

			int digit = TrailingZeroCount(grid.GetCandidates(resultCell));
			var step = new FullHouseStep(
				ImmutableArray.Create(new Conclusion(ConclusionType.Assignment, resultCell, digit)),
				ImmutableArray.Create(
					View.Empty
						| new CandidateViewNode(DisplayColorKind.Normal, resultCell * 9 + digit)
						| new HouseViewNode(DisplayColorKind.Normal, house)
				),
				resultCell,
				digit
			);

			if (onlyFindOne)
			{
				return step;
			}

			accumulator.Add(step);
		}
	CheckHiddenSingle:
		if (HiddenSinglesInBlockFirst)
		{
			// If block first, we'll extract all blocks and iterate on them firstly.
			for (int house = 0; house < 9; house++)
			{
				for (int digit = 0; digit < 9; digit++)
				{
					if (g(grid, digit, house) is not { } step)
					{
						continue;
					}

					if (onlyFindOne)
					{
						return step;
					}

					accumulator.Add(step);
				}
			}

			// Then secondly rows and columns.
			for (int house = 9; house < 27; house++)
			{
				for (int digit = 0; digit < 9; digit++)
				{
					if (g(grid, digit, house) is not { } step)
					{
						continue;
					}

					if (onlyFindOne)
					{
						return step;
					}

					accumulator.Add(step);
				}
			}
		}
		else
		{
			// We'll directly iterate on each house.
			// Theoretically, this iteration should be faster than above one, but in practice,
			// we may found hidden singles in block much more times than in row or column.
			for (int digit = 0; digit < 9; digit++)
			{
				for (int house = 0; house < 27; house++)
				{
					if (g(grid, digit, house) is not { } step)
					{
						continue;
					}

					if (onlyFindOne)
					{
						return step;
					}

					accumulator.Add(step);
				}
			}
		}

		for (int cell = 0; cell < 81; cell++)
		{
			if (grid.GetStatus(cell) != CellStatus.Empty)
			{
				continue;
			}

			short mask = grid.GetCandidates(cell);
			if (!IsPow2(mask))
			{
				continue;
			}

			int digit = TrailingZeroCount(mask);

			var step = new NakedSingleStep(
				ImmutableArray.Create(new Conclusion(ConclusionType.Assignment, cell, digit)),
				ImmutableArray.Create(
					View.Empty
						| new CandidateViewNode(DisplayColorKind.Normal, cell * 9 + digit)
				),
				cell,
				digit
			);

			if (onlyFindOne)
			{
				return step;
			}

			accumulator.Add(step);
		}

		return null;


		IStep? g(scoped in Grid grid, int digit, int house)
		{
			// The main idea of hidden single is to search for a digit can only appear once in a house,
			// so we should check all possibilities in a house to found whether the house exists a digit
			// that only appears once indeed.
			int count = 0, resultCell = -1;
			bool flag = true;
			foreach (int cell in HouseMaps[house])
			{
				if (grid.Exists(cell, digit) is true)
				{
					resultCell = cell;
					if (++count > 1)
					{
						flag = false;
						break;
					}
				}
			}
			if (!flag || count == 0)
			{
				// The digit has been filled into the house, or the digit appears more than once,
				// the digit will be invalid for a hidden single. Just skip it.
				return null;
			}

			// Now here the digit is a hidden single. We should gather the information
			// (painting or text information) on the step in order to display onto the UI.
			bool enableAndIsLastDigit = false;
			var cellOffsets = new List<CellViewNode>();
			if (EnableLastDigit)
			{
				// Sum up the number of appearing in the grid of 'digit'.
				int digitCount = 0;
				for (int i = 0; i < 81; i++)
				{
					if (grid[i] == digit)
					{
						digitCount++;
						cellOffsets.Add(new(DisplayColorKind.Normal, i));
					}
				}

				enableAndIsLastDigit = digitCount == 8;
			}

			return new HiddenSingleStep(
				ImmutableArray.Create(new Conclusion(ConclusionType.Assignment, resultCell, digit)),
				ImmutableArray.Create(
					View.Empty
						| (enableAndIsLastDigit ? cellOffsets : null)
						| new CandidateViewNode(DisplayColorKind.Normal, resultCell * 9 + digit)
						| (enableAndIsLastDigit ? null : new HouseViewNode(DisplayColorKind.Normal, house))
				),
				resultCell,
				digit,
				house,
				enableAndIsLastDigit
			);
		}
	}
}
