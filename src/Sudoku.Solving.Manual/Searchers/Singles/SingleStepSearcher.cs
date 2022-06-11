namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Single</b> step searcher. The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Full House</item>
/// <item>Last Digit</item>
/// <item>Hidden Single</item>
/// <item>Naked Single</item>
/// </list>
/// </summary>
[StepSearcher]
[StepSearcherOptions(IsDirect = true, IsOptionsFixed = true)]
public sealed unsafe partial class SingleStepSearcher : ISingleStepSearcher
{
	/// <inheritdoc/>
	public bool EnableFullHouse { get; set; }

	/// <inheritdoc/>
	public bool EnableLastDigit { get; set; }

	/// <inheritdoc/>
	public bool HiddenSinglesInBlockFirst { get; set; }

	/// <inheritdoc/>
	public bool ShowDirectLines { get; set; }


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
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
			List<CrosshatchViewNode>? directLines = null;
			if (ShowDirectLines)
			{
				directLines = new(6);
				for (int i = 0; i < 9; i++)
				{
					if (digit != i)
					{
						bool flag = false;
						foreach (int peerCell in PeerMaps[cell])
						{
							if (grid[peerCell] == i)
							{
								directLines.Add(
									new(DisplayColorKind.Normal, Cells.Empty + peerCell, Cells.Empty, digit)
								);
								
								flag = true;
								break;
							}
						}
						if (flag)
						{
							continue;
						}
					}
				}
			}

			var step = new NakedSingleStep(
				ImmutableArray.Create(new Conclusion(ConclusionType.Assignment, cell, digit)),
				ImmutableArray.Create(
					View.Empty
						| new CandidateViewNode(DisplayColorKind.Normal, cell * 9 + digit)
						| directLines
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


		Step? g(in Grid grid, int digit, int house)
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

			// Get direct lines.
			List<CrosshatchViewNode>? directLines = null;
			if (!enableAndIsLastDigit && ShowDirectLines)
			{
				directLines = new(6);

				// Step 1: Get all source cells that makes the result cell
				// can't be filled with the result digit.
				Cells crosshatchingCells = Cells.Empty, tempMap = Cells.Empty;
				foreach (int cell in HouseCells[house])
				{
					if (cell != resultCell && grid.GetStatus(cell) == CellStatus.Empty)
					{
						tempMap.Add(cell);
					}
				}
				foreach (int cell in tempMap)
				{
					foreach (int peerCell in PeerMaps[cell])
					{
						if (cell != resultCell && grid[peerCell] == digit)
						{
							crosshatchingCells.Add(peerCell);
						}
					}
				}

				// Step 2: Get all removed cells in this house.
				foreach (int cell in crosshatchingCells)
				{
					if ((PeerMaps[cell] & tempMap) is { Count: not 0 } removableCells)
					{
						directLines.Add(new(DisplayColorKind.Normal, Cells.Empty + cell, removableCells, digit));
						tempMap -= removableCells;
					}
				}
			}

			return new HiddenSingleStep(
				ImmutableArray.Create(new Conclusion(ConclusionType.Assignment, resultCell, digit)),
				ImmutableArray.Create(
					View.Empty
						| (enableAndIsLastDigit ? cellOffsets : null)
						| new CandidateViewNode(DisplayColorKind.Normal, resultCell * 9 + digit)
						| (enableAndIsLastDigit ? null : new HouseViewNode(DisplayColorKind.Normal, house))
						| directLines
				),
				resultCell,
				digit,
				house,
				enableAndIsLastDigit
			);
		}
	}
}
