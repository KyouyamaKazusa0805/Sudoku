#define HIDDEN_SINGLE_BLOCK_FIRST


namespace Sudoku.Solving.Manual.Singles;

/// <summary>
/// Encapsulates a <b>single</b> technique searcher.
/// </summary>
[DirectSearcher, IsOptionsFixed]
public sealed class SingleStepSearcher : StepSearcher
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
	/// Direct lines is a concept that describes the crosshatching information of a hidden single.
	/// For example, in this following grid:
	/// <code><![CDATA[
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
	/// ]]></code>
	/// The start point of the direct lines are:
	/// <list type="bullet">
	/// <item><c>r4c3(1)</c>, removes the cases of digit 1 for cells <c>r4c456</c></item>
	/// <item><c>r2c6(1)</c>, removes the cases of digit 1 for cells <c>r456c6</c></item>
	/// <item><c>r6c7(1)</c>, removes the cases of digit 1 for cells <c>r6c456</c></item>
	/// <item><c>r8c4(1)</c>, removes the cases of digit 1 for cells <c>r456c4</c></item>
	/// </list>
	/// </para>
	/// <para>
	/// All the end points may be displayed using a cross mark (<c>'x'</c>), and the start
	/// point may be used a circle mark (<c>'o'</c>).
	/// </para>
	/// </remarks>
	public bool ShowDirectLines { get; set; }

	/// <summary>
	/// Indicates the searcher properties.
	/// </summary>
	/// <remarks>
	/// Please note that all technique searches should contain
	/// this static property in order to display on settings window. If the searcher doesn't contain,
	/// when we open the settings window, it'll throw an exception to report about this.
	/// </remarks>
	[Obsolete($"Please use the property '{nameof(Options)}' instead.", false)]
	public static TechniqueProperties Properties { get; } = new(1, nameof(Technique.NakedSingle))
	{
		DisplayLevel = 0,
		IsReadOnly = true
	};


	/// <inheritdoc/>
	public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
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
			accumulator.Add(
				new FullHouseStepInfo(
					new Conclusion[] { new(ConclusionType.Assignment, resultCell, digit) },
					new View[]
					{
						new()
						{
							Candidates = new DrawingInfo[] { new(0, resultCell * 9 + digit) },
							Regions = new DrawingInfo[] { new(0, region) }
						}
					},
					resultCell,
					digit
				)
			);
		}
	CheckHiddenSingle:
#if HIDDEN_SINGLE_BLOCK_FIRST
		// If block first, we'll extract all blocks and iterate on them firstly.
		for (int region = 0; region < 9; region++)
		{
			for (int digit = 0; digit < 9; digit++)
			{
				if (!g(accumulator, grid, digit, region))
				{
					continue;
				}
			}
		}

		// Then secondly rows and columns.
		for (int region = 9; region < 27; region++)
		{
			for (int digit = 0; digit < 9; digit++)
			{
				if (!g(accumulator, grid, digit, region))
				{
					continue;
				}
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
				if (!g(accumulator, grid, digit, region))
				{
					continue;
				}
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
			List<(Cells, Cells)>? directLines = null;
			if (ShowDirectLines)
			{
				directLines = new();
				for (int i = 0; i < 9; i++)
				{
					if (digit != i)
					{
						bool flag = false;
						foreach (int peerCell in PeerMaps[cell])
						{
							if (grid[peerCell] == i)
							{
								directLines.Add((new() { peerCell }, Cells.Empty));
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

			accumulator.Add(
				new NakedSingleStepInfo(
					new Conclusion[] { new(ConclusionType.Assignment, cell, digit) },
					new View[]
					{
						new()
						{
							Candidates = new DrawingInfo[] { new(0, cell * 9 + digit) },
							DirectLines = directLines
						}
					},
					cell,
					digit
				)
			);
		}

		bool g(IList<StepInfo> accumulator, in SudokuGrid grid, int digit, int region)
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
				return false;
			}

			// Now here the digit is a hidden single. We should gather the information
			// (painting or text information) on the step in order to display onto the UI.
			bool enableAndIsLastDigit = false;
			var cellOffsets = new List<DrawingInfo>();
			if (EnableLastDigit)
			{
				// Sum up the number of appearing in the grid of 'digit'.
				int digitCount = 0;
				for (int i = 0; i < 81; i++)
				{
					if (grid[i] == digit)
					{
						digitCount++;
						cellOffsets.Add(new(0, i));
					}
				}

				enableAndIsLastDigit = digitCount == 8;
			}

			// Get direct lines.
			List<(Cells, Cells)>? directLines = null;
			if (!enableAndIsLastDigit && ShowDirectLines)
			{
				directLines = new();

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
						directLines.Add((new() { cell }, removableCells));
						tempMap -= removableCells;
					}
				}
			}

			accumulator.Add(
				new HiddenSingleStepInfo(
					new Conclusion[] { new(ConclusionType.Assignment, resultCell, digit) },
					new View[]
					{
						new()
						{
							Cells = enableAndIsLastDigit ? cellOffsets : null,
							Candidates = new DrawingInfo[] { new(0, resultCell * 9 + digit) },
							Regions = enableAndIsLastDigit ? null : new DrawingInfo[] { new(0, region) },
							DirectLines = directLines
						}
					},
					resultCell,
					digit,
					region,
					enableAndIsLastDigit
				)
			);

			return true;
		}
	}
}
