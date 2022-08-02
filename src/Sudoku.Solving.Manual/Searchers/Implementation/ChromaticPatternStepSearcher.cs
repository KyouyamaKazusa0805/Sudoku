namespace Sudoku.Solving.Manual.Searchers;

[StepSearcher]
internal sealed partial class ChromaticPatternStepSearcher : IChromaticPatternStepSearcher
{
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
			foreach (short tempBlocksMask in ChromaticPatternBlocksCombinations)
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

			foreach (var (a, b, c, d) in IChromaticPatternStepSearcher.PatternOffsets)
			{
				var pattern = f(a, c1) | f(b, c2) | f(c, c3) | f(d, c4);
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

				// Gather steps.
				if (CheckType1(accumulator, grid, onlyFindOne, pattern, blocks) is { } type1Step)
				{
					return type1Step;
				}
				if (CheckXz(accumulator, grid, onlyFindOne, pattern, blocks) is { } typeXzStep)
				{
					return typeXzStep;
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

	/// <summary>
	/// Checks for XZ rule.
	/// Here I give you 1 example to test this method:
	/// <list type="number">
	/// <item>
	/// <code>
	/// 0000000010000023400+45013602000+470036000089400004600000012500060403100520560000100:611 711 811 911 712 812 912 915 516 716 816 721 821 921 925 565 568 779 879 979 794 894 994 799 899 999
	/// </code>
	/// </item>
	/// </list>
	/// </summary>
	private Step? CheckXz(
		ICollection<Step> accumulator, scoped in Grid grid, bool onlyFindOne,
		scoped in Cells pattern, int[] blocks)
	{
		short allDigitsMask = grid.GetDigitsUnion(pattern);
		if (PopCount((uint)allDigitsMask) != 5)
		{
			// The pattern cannot find any possible eliminations because the number of extra digits
			// are not 2 will cause the extra digits not forming a valid strong link
			// as same behavior as ALS-XZ or BUG-XZ rule.
			return null;
		}

		foreach (int[] digits in allDigitsMask.GetAllSets().GetSubsets(3))
		{
			short patternDigitsMask = (short)(1 << digits[0] | 1 << digits[1] | 1 << digits[2]);
			short otherDigitsMask = (short)(allDigitsMask & ~patternDigitsMask);
			int d1 = TrailingZeroCount(otherDigitsMask);
			int d2 = otherDigitsMask.GetNextSet(d1);
			var otherDigitsCells = pattern & (CandidatesMap[d1] | CandidatesMap[d2]);
			if (otherDigitsCells is not [var c1, var c2])
			{
				continue;
			}

			foreach (int extraCell in (PeerMaps[c1] ^ PeerMaps[c2]) & BivalueCells)
			{
				if (grid.GetCandidates(extraCell) != otherDigitsMask)
				{
					continue;
				}

				// XZ rule found.
				var conclusions = new List<Conclusion>();
				bool condition = (Cells.Empty + c1 + extraCell).InOneHouse;
				int anotherCell = condition ? c2 : c1;
				int anotherDigit = condition ? d1 : d2;
				foreach (int peer in !(Cells.Empty + extraCell + anotherCell))
				{
					if (grid.Exists(peer, anotherDigit) is true)
					{
						conclusions.Add(new(ConclusionType.Elimination, peer, anotherDigit));
					}
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>(33);
				foreach (int patternCell in pattern)
				{
					foreach (int digit in grid.GetCandidates(patternCell))
					{
						candidateOffsets.Add(
							new(
								otherDigitsCells.Contains(patternCell) && (d1 == digit || d2 == digit)
									? DisplayColorKind.Auxiliary1
									: DisplayColorKind.Normal,
								patternCell * 9 + digit
							)
						);
					}
				}

				var step = new ChromaticPatternXzStep(
					conclusions.ToImmutableArray(),
					ImmutableArray.Create(
						View.Empty
							| candidateOffsets
							| new CellViewNode(DisplayColorKind.Normal, extraCell)
							| from block in blocks select new HouseViewNode(DisplayColorKind.Normal, block)
					),
					blocks,
					pattern,
					otherDigitsCells,
					extraCell,
					patternDigitsMask,
					otherDigitsMask
				);
				if (onlyFindOne)
				{
					return step;
				}

				accumulator.Add(step);
			}
		}

		return null;
	}
}
