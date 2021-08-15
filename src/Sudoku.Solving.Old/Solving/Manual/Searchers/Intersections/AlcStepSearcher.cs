namespace Sudoku.Solving.Manual.Intersections;

/// <summary>
/// Encapsulates an <b>almost locked candidates</b> (ALC) technique searcher.
/// </summary>
public sealed class AlcStepSearcher : IntersectionStepSearcher
{
	/// <summary>
	/// Indicates whether the user checks the almost locked quadruple.
	/// </summary>
	public bool CheckAlmostLockedQuadruple { get; set; }

	/// <inheritdoc/>
	public override SearchingOptions Options { get; set; } = new(9, DisplayingLevel.B);

	/// <summary>
	/// Indicates the searcher properties.
	/// </summary>
	/// <remarks>
	/// Please note that all technique searches should contain
	/// this static property in order to display on settings window. If the searcher doesn't contain,
	/// when we open the settings window, it'll throw an exception to report about this.
	/// </remarks>
	[Obsolete($"Please use the property '{nameof(Options)}' instead.", false)]
	public static TechniqueProperties Properties { get; } = new(9, nameof(Technique.AlmostLockedPair))
	{
		DisplayLevel = 2
	};


	/// <inheritdoc/>
	public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
	{
		for (int size = 2, maxSize = CheckAlmostLockedQuadruple ? 4 : 3; size <= maxSize; size++)
		{
			foreach (var ((baseSet, coverSet), (a, b, c, _)) in IntersectionMaps)
			{
				if (!(c & EmptyMap).IsEmpty)
				{
					// Process for 2 cases.
					GetAll(accumulator, grid, size, baseSet, coverSet, a, b, c);
					GetAll(accumulator, grid, size, coverSet, baseSet, b, a, c);
				}
			}
		}
	}


	/// <summary>
	/// Process the calculation.
	/// </summary>
	/// <param name="result">The result.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="size">The size.</param>
	/// <param name="baseSet">The base set.</param>
	/// <param name="coverSet">The cover set.</param>
	/// <param name="a">The left grid map.</param>
	/// <param name="b">The right grid map.</param>
	/// <param name="c">The intersection.</param>
	private static void GetAll(
		IList<StepInfo> result, in SudokuGrid grid, int size, int baseSet, int coverSet,
		in Cells a, in Cells b, in Cells c)
	{
		// The diagrams:
		//   ALP:
		//   abx aby | ab
		//   abz     |
		//
		//   ALT:
		//   abcw abcx | abc abc
		//   abcy abcz |
		//
		// Algorithm:
		// If the cell 'ab' (in ALP) or 'abc' (in ALT) is filled with the digit 'p',
		// then the cells 'abx' and 'aby' (in ALP) and 'abcw' and 'abcx' (in ALT) can't
		// fill the digit 'p'. Therefore the digit 'p' can only be filled into the left-side block.
		// If the block only contains those cells that can contain the digit 'p',
		// the ALP or ALT will be formed, and the elimination is 'z' (in ALP) and 'y' and 'z' (in ALT).

		// Iterate on each cell combination.
		foreach (int[] cells in (a & EmptyMap).ToArray().GetSubsets(size - 1))
		{
			// Gather the mask. The cell combination must contain the specified number of digits.
			short mask = 0;
			foreach (int cell in cells)
			{
				mask |= grid.GetCandidates(cell);
			}
			if (PopCount((uint)mask) != size)
			{
				continue;
			}

			// Get all possible digits (Digit 'a', 'b' and 'c' in those diagrams).
			var digits = mask.GetAllSets();

			// Check whether overlapped.
			bool isOverlapped = false;
			foreach (int digit in digits)
			{
				if (!(ValueMaps[digit] & RegionMaps[coverSet]).IsEmpty)
				{
					isOverlapped = true;
					break;
				}
			}
			if (isOverlapped)
			{
				continue;
			}

			// Then check whether the another region (left-side block in those diagrams)
			// forms an AHS (i.e. those digits must appear in the specified cells).
			short ahsMask = 0;
			foreach (int digit in digits)
			{
				ahsMask |= (RegionMaps[coverSet] & CandMaps[digit] & b) / coverSet;
			}
			if (PopCount((uint)ahsMask) != size - 1)
			{
				continue;
			}

			// Gather the AHS cells.
			var ahsCells = Cells.Empty;
			foreach (int pos in ahsMask)
			{
				ahsCells.AddAnyway(RegionCells[coverSet][pos]);
			}

			// Gather all eliminations.
			var cellsMap = new Cells(cells);
			var conclusions = new List<Conclusion>();
			foreach (int aCell in a)
			{
				if (cellsMap.Contains(aCell))
				{
					continue;
				}

				foreach (int digit in mask & grid.GetCandidates(aCell))
				{
					conclusions.Add(new(ConclusionType.Elimination, aCell, digit));
				}
			}
			foreach (int digit in SudokuGrid.MaxCandidatesMask & ~mask)
			{
				foreach (int ahsCell in ahsCells & CandMaps[digit])
				{
					conclusions.Add(new(ConclusionType.Elimination, ahsCell, digit));
				}
			}

			// Check whether any eliminations exists.
			if (conclusions.Count == 0)
			{
				continue;
			}

			// Gather highlight candidates.
			var candidateOffsets = new List<DrawingInfo>();
			foreach (int digit in digits)
			{
				foreach (int cell in cellsMap & CandMaps[digit])
				{
					candidateOffsets.Add(new(0, cell * 9 + digit));
				}
			}
			foreach (int cell in c)
			{
				foreach (int digit in mask & grid.GetCandidates(cell))
				{
					candidateOffsets.Add(new(1, cell * 9 + digit));
				}
			}
			foreach (int cell in ahsCells)
			{
				foreach (int digit in mask & grid.GetCandidates(cell))
				{
					candidateOffsets.Add(new(0, cell * 9 + digit));
				}
			}

			var map = (cellsMap | ahsCells) - EmptyMap;
			var valueCells = new DrawingInfo[map.Count];
			int i = 0;
			foreach (int cell in map)
			{
				valueCells[i++] = new(0, cell);
			}
			bool hasValueCell = valueCells.Length != 0;
			result.Add(
				new AlcStepInfo(
					conclusions,
					new View[]
					{
						new()
						{
							Cells = hasValueCell ? valueCells : null,
							Candidates = candidateOffsets,
							Regions = new DrawingInfo[] { new(0, baseSet), new(2, coverSet) }
						}
					},
					mask,
					cellsMap,
					ahsCells,
					hasValueCell
				)
			);
		}
	}
}
