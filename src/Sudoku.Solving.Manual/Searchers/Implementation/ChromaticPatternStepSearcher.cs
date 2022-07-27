namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Chromatic Pattern</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Chromatic Pattern type 1</item>
/// <!--
/// <item>Chromatic Pattern type 2</item>
/// <item>Chromatic Pattern type 3</item>
/// <item>Chromatic Pattern type 4</item>
/// -->
/// </list>
/// </summary>
[StepSearcher]
internal sealed partial class ChromaticPatternStepSearcher : IChromaticPatternStepSearcher
{
	/// <summary>
	/// All possible blocks combinations.
	/// </summary>
	private static readonly short[] BlocksCombinations = new short[]
	{
		0b000_011_011, 0b000_101_101, 0b000_110_110,
		0b011_000_011, 0b101_000_101, 0b110_000_110,
		0b011_011_000, 0b101_101_000, 0b110_110_000
	};

	/// <summary>
	/// The possible pattern offsets.
	/// </summary>
	private static readonly (int[], int[], int[], int[])[] PatternOffsets;


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static ChromaticPatternStepSearcher()
	{
		int[][] InnerCombinations = new[]
		{
			new[] { 0, 10, 20 },
			new[] { 1, 11, 18 },
			new[] { 2,  9, 19 },
			new[] { 0, 11, 19 },
			new[] { 1,  9, 20 },
			new[] { 2, 10, 18 }
		};

		var patternOffsetsList = new List<(int[], int[], int[], int[])>();

		int[][] diagonalCases = InnerCombinations[..3];
		int[][] antidiagonalCases = InnerCombinations[3..];

		foreach (var (aCase, bCase, cCase, dCase) in stackalloc[]
		{
			(true, false, false, false),
			(false, true, false, false),
			(false, false, true, false),
			(false, false, false, true)
		})
		{
			// Phase 1.
			foreach (int[] a in aCase ? diagonalCases : antidiagonalCases)
			{
				foreach (int[] b in bCase ? diagonalCases : antidiagonalCases)
				{
					foreach (int[] c in cCase ? diagonalCases : antidiagonalCases)
					{
						foreach (int[] d in dCase ? diagonalCases : antidiagonalCases)
						{
							patternOffsetsList.Add((a, b, c, d));
						}
					}
				}
			}

			// Phase 2.
			foreach (int[] a in aCase ? antidiagonalCases : diagonalCases)
			{
				foreach (int[] b in bCase ? antidiagonalCases : diagonalCases)
				{
					foreach (int[] c in cCase ? antidiagonalCases : diagonalCases)
					{
						foreach (int[] d in dCase ? antidiagonalCases : diagonalCases)
						{
							patternOffsetsList.Add((a, b, c, d));
						}
					}
				}
			}
		}

		PatternOffsets = patternOffsetsList.ToArray();
	}


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		if (EmptyCells.Count < 12)
		{
			// This technique requires at least 12 cells to be used.
			return null;
		}

		short satisfiedblocksMask = 0;
		for (int block = 0; block < 9; block++)
		{
			if ((EmptyCells & HouseMaps[block]).Count >= 3)
			{
				satisfiedblocksMask |= (short)(1 << block);
			}
		}

		if (PopCount((uint)satisfiedblocksMask) < 4)
		{
			// At least four blocks should contain at least 3 cells.
			return null;
		}

		foreach (int[] blocks in satisfiedblocksMask.GetAllSets().GetSubsets(4))
		{
			short blocksMask = 0;
			foreach (int block in blocks)
			{
				blocksMask |= (short)(1 << block);
			}

			bool flag = false;
			foreach (short tempBlocksMask in BlocksCombinations)
			{
				if ((tempBlocksMask & blocksMask) == blocksMask)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				continue;
			}

			int c1 = HouseCells[blocks[0]][0];
			int c2 = HouseCells[blocks[1]][0];
			int c3 = HouseCells[blocks[2]][0];
			int c4 = HouseCells[blocks[3]][0];

			foreach (var (a, b, c, d) in PatternOffsets)
			{
				var pattern = Cells.Empty | f(a, c1) | f(b, c2) | f(c, c3) | f(d, c4);
				if ((EmptyCells & pattern) != pattern)
				{
					// All cells in this pattern should be empty.
					continue;
				}

				bool containsNakedSingle = false;
				foreach (int cell in pattern)
				{
					short candidatesMask = grid.GetCandidates(cell);
					if (IsPow2(candidatesMask))
					{
						containsNakedSingle = true;
						break;
					}
				}
				if (containsNakedSingle)
				{
					continue;
				}

				if (CheckType1(accumulator, grid, onlyFindOne, pattern, blocks) is { } type1Step)
				{
					return type1Step;
				}
			}
		}

		return null;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static Cells f(int[] offsets, int currentOffset)
		{
			int a = offsets[0] + currentOffset;
			int b = offsets[1] + currentOffset;
			int c = offsets[2] + currentOffset;
			return Cells.Empty + a + b + c;
		}
	}

	/// <summary>
	/// Checks for the type 1.
	/// Here I give you 2 examples to test this method:
	/// <list type="number">
	/// <item>
	/// <see href="http://forum.enjoysudoku.com/the-tridagon-rule-t39859.html#p318380">the first one</see>
	/// </item>
	/// <item>
	/// <see href="http://forum.enjoysudoku.com/the-tridagon-rule-t39859.html#p318378">the second one</see>
	/// </item>
	/// </list>
	/// </summary>
	private Step? CheckType1(
		ICollection<Step> accumulator, scoped in Grid grid, bool onlyFindOne,
		scoped in Cells pattern, int[] blocks)
	{
		foreach (int extraCell in pattern)
		{
			var otherCells = pattern - extraCell;
			short digitsMask = grid.GetDigitsUnion(otherCells);
			if (PopCount((uint)digitsMask) != 3)
			{
				continue;
			}

			short elimDigitsMask = (short)(grid.GetCandidates(extraCell) & digitsMask);
			if (elimDigitsMask == 0)
			{
				// No eliminations.
				continue;
			}

			var conclusions = new List<Conclusion>();
			foreach (int digit in elimDigitsMask)
			{
				conclusions.Add(new(ConclusionType.Elimination, extraCell, digit));
			}

			var candidateOffsets = new List<CandidateViewNode>((12 - 1) * 3);
			foreach (int otherCell in otherCells)
			{
				foreach (int otherDigit in grid.GetCandidates(otherCell))
				{
					candidateOffsets.Add(new(DisplayColorKind.Normal, otherCell * 9 + otherDigit));
				}
			}

			var step = new ChromaticPatternType1Step(
				conclusions.ToImmutableArray(),
				ImmutableArray.Create(
					View.Empty
						| candidateOffsets
						| from house in blocks select new HouseViewNode(DisplayColorKind.Normal, house)
				),
				blocks,
				pattern,
				extraCell,
				digitsMask
			);
			if (onlyFindOne)
			{
				return step;
			}

			accumulator.Add(step);
		}

		return null;
	}
}
