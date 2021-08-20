#define HIDDEN_SINGLE_BLOCK_FIRST

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
[IsDirect]
[IsOptionsFixed]
public sealed class SingleStepSearcher : IStepSearcher
{
	/// <summary>
	/// Indicates whether the solver enables the technique full house.
	/// </summary>
	public bool EnableFullHouse { get; set; }

	/// <summary>
	/// Indicates whether the solver enables the technique last digit.
	/// </summary>
	public bool EnableLastDigit { get; set; }

	/// <summary>
	/// Indicates whether the solver shows the direct lines (cross-hatching information).
	/// </summary>
	/// <remarks>
	/// <para>
	/// <b>Direct line</b>s is a concept that describes the crosshatching information of a hidden single.
	/// For example, in this following grid:
	/// <code>
	/// .---------.---------.---------.
	/// | .  .  . | .  .  . | .  .  . |
	/// | .  .  . | .  .  1 | .  .  . |
	/// | .  .  . | .  .  . | .  .  . |
	/// :---------+---------+---------:
	/// | .  .  1 | x  x  x | .  .  . |
	/// | .  .  . | x  .  x | .  .  . |
	/// | .  .  . | x  x  x | 1  .  . |
	/// :---------+---------+---------:
	/// | .  .  . | .  .  . | .  .  . |
	/// | .  .  . | 1  .  . | .  .  . |
	/// | .  .  . | .  .  . | .  .  . |
	/// '---------'---------'---------'
	/// </code>
	/// The start point of the direct lines are:
	/// <list type="bullet">
	/// <item><c>r4c3(1)</c>, removes the cases of digit 1 for cells <c>r4c456</c>.</item>
	/// <item><c>r2c6(1)</c>, removes the cases of digit 1 for cells <c>r456c6</c>.</item>
	/// <item><c>r6c7(1)</c>, removes the cases of digit 1 for cells <c>r6c456</c>.</item>
	/// <item><c>r8c4(1)</c>, removes the cases of digit 1 for cells <c>r456c4</c>.</item>
	/// </list>
	/// </para>
	/// <para>
	/// All the end points may be displayed using a cross mark ('<c>x</c>'), and the start
	/// point may be used a circle mark ('<c>o</c>').
	/// </para>
	/// </remarks>
	public bool ShowDirectLines { get; set; }

	/// <inheritdoc/>
	public SearcherIdentifier Identifier => SearcherIdentifier.Single;

	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(1, DisplayingLevel.A);


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		if (!EnableFullHouse)
		{
			goto CheckHiddenSingle;
		}

		for (int region = 0; region < 27; region++)
		{
			int count = 0, resultCell = -1;
			bool flag = true;
			foreach (int cell in RegionMaps[region])
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
				new Conclusion[] { new(ConclusionType.Assignment, resultCell, digit) }.ToImmutableArray(),
				new PresentationData[]
				{
					new()
					{
						Candidates = new[] { (resultCell * 9 + digit, (ColorIdentifier)0) },
						Regions = new[] { (region, (ColorIdentifier)0) }
					}
				}.ToImmutableArray(),
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
#if HIDDEN_SINGLE_BLOCK_FIRST
		// If block first, we'll extract all blocks and iterate on them firstly.
		for (int region = 0; region < 9; region++)
		{
			for (int digit = 0; digit < 9; digit++)
			{
				if (g(accumulator, grid, digit, region) is not { } step)
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
		for (int region = 9; region < 27; region++)
		{
			for (int digit = 0; digit < 9; digit++)
			{
				if (g(accumulator, grid, digit, region) is not { } step)
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
#else
		// We'll directly iterate on each region.
		// Theoretically, this iteration should be faster than above one, but in practice,
		// we may found hidden singles in block much more times than in row or column.
		for (int digit = 0; digit < 9; digit++)
		{
			for (int region = 0; region < 27; region++)
			{
				if (g(accumulator, grid, digit, region) is not { } step)
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
#endif
		for (int cell = 0; cell < 81; cell++)
		{
			if (grid.GetStatus(cell) != CellStatus.Empty)
			{
				continue;
			}

			short mask = grid.GetCandidates(cell);
			if (mask == 0 || (mask & mask - 1) != 0)
			{
				continue;
			}

			int digit = TrailingZeroCount(mask);
			List<(Crosshatch, ColorIdentifier)>? directLines = null;
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
								directLines.Add((new(new Cells { peerCell }, Cells.Empty), (ColorIdentifier)0));
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
				new Conclusion[] { new(ConclusionType.Assignment, cell, digit) }.ToImmutableArray(),
				new PresentationData[]
				{
					new()
					{
						Candidates = new[] { (cell * 9 + digit, (ColorIdentifier)0) },
						DirectLines = directLines
					}
				}.ToImmutableArray(),
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


		Step? g(ICollection<Step> accumulator, in Grid grid, int digit, int region)
		{
			// The main idea of hidden single is to search for a digit can only appear once in a region,
			// so we should check all possibilities in a region to found whether the region exists a digit
			// that only appears once indeed.
			int count = 0, resultCell = -1;
			bool flag = true;
			foreach (int cell in RegionMaps[region])
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
				// The digit has been filled into the region, or the digit
				// appears more than once, which means the digit is invalid case for hidden single.
				// Just skip it.
				return null;
			}

			// Now here the digit is a hidden single. We should gather the information
			// (painting or text information) on the step in order to display onto the UI.
			bool enableAndIsLastDigit = false;
			var cellOffsets = new List<(int, ColorIdentifier)>();
			if (EnableLastDigit)
			{
				// Sum up the number of appearing in the grid of 'digit'.
				int digitCount = 0;
				for (int i = 0; i < 81; i++)
				{
					if (grid[i] == digit)
					{
						digitCount++;
						cellOffsets.Add((i, (ColorIdentifier)0));
					}
				}

				enableAndIsLastDigit = digitCount == 8;
			}

			// Get direct lines.
			List<(Crosshatch, ColorIdentifier)>? directLines = null;
			if (!enableAndIsLastDigit && ShowDirectLines)
			{
				directLines = new(6);

				// Step 1: Get all source cells that makes the result cell
				// can't be filled with the result digit.
				Cells crosshatchingCells = Cells.Empty, tempMap = Cells.Empty;
				foreach (int cell in RegionCells[region])
				{
					if (cell != resultCell && grid.GetStatus(cell) == CellStatus.Empty)
					{
						tempMap.AddAnyway(cell);
					}
				}
				foreach (int cell in tempMap)
				{
					foreach (int peerCell in PeerMaps[cell])
					{
						if (cell != resultCell && grid[peerCell] == digit)
						{
							crosshatchingCells.AddAnyway(peerCell);
						}
					}
				}

				// Step 2: Get all removed cells in this region.
				foreach (int cell in crosshatchingCells)
				{
					var removableCells = PeerMaps[cell] & tempMap;
					if (!removableCells.IsEmpty)
					{
						directLines.Add((new(new() { cell }, removableCells), (ColorIdentifier)0));
						tempMap -= removableCells;
					}
				}
			}

			return new HiddenSingleStep(
				new Conclusion[] { new(ConclusionType.Assignment, resultCell, digit) }.ToImmutableArray(),
				new PresentationData[]
				{
					new()
					{
						Cells = enableAndIsLastDigit ? cellOffsets : null,
						Candidates = new[] { (resultCell * 9 + digit, (ColorIdentifier)0) },
						Regions = enableAndIsLastDigit ? null : new[] { (region, (ColorIdentifier)0) },
						DirectLines = directLines
					}
				}.ToImmutableArray(),
				resultCell,
				digit,
				region,
				enableAndIsLastDigit
			);
		}
	}
}
