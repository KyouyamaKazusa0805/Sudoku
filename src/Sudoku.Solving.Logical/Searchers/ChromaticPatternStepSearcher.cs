namespace Sudoku.Solving.Logics.Implementations.Searchers;

[StepSearcher]
internal sealed partial class ChromaticPatternStepSearcher : IChromaticPatternStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(scoped in LogicalAnalysisContext context)
	{
		if (EmptyCells.Count < 12)
		{
			// This technique requires at least 12 cells to be used.
			return null;
		}

		short satisfiedblocksMask = 0;
		for (var block = 0; block < 9; block++)
		{
			if ((EmptyCells & HousesMap[block]).Count >= 3)
			{
				satisfiedblocksMask |= (short)(1 << block);
			}
		}

		if (PopCount((uint)satisfiedblocksMask) < 4)
		{
			// At least four blocks should contain at least 3 cells.
			return null;
		}

		scoped ref readonly var grid = ref context.Grid;
		foreach (var blocks in satisfiedblocksMask.GetAllSets().GetSubsets(4))
		{
			short blocksMask = 0;
			foreach (var block in blocks)
			{
				blocksMask |= (short)(1 << block);
			}

			var flag = false;
			foreach (var tempBlocksMask in ChromaticPatternBlocksCombinations)
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

			var c1 = HouseCells[blocks[0]][0];
			var c2 = HouseCells[blocks[1]][0];
			var c3 = HouseCells[blocks[2]][0];
			var c4 = HouseCells[blocks[3]][0];

			foreach (var (a, b, c, d) in IChromaticPatternStepSearcher.PatternOffsets)
			{
				var pattern = f(a, c1) | f(b, c2) | f(c, c3) | f(d, c4);
				if ((EmptyCells & pattern) != pattern)
				{
					// All cells in this pattern should be empty.
					continue;
				}

				var containsNakedSingle = false;
				foreach (var cell in pattern)
				{
					var candidatesMask = grid.GetCandidates(cell);
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
				if (CheckType1(context, pattern, blocks) is { } type1Step)
				{
					return type1Step;
				}
				if (CheckXz(context, pattern, blocks) is { } typeXzStep)
				{
					return typeXzStep;
				}
			}
		}

		return null;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static CellMap f(int[] offsets, int currentOffset)
		{
			var a = offsets[0] + currentOffset;
			var b = offsets[1] + currentOffset;
			var c = offsets[2] + currentOffset;
			return CellsMap[a] + b + c;
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
	private IStep? CheckType1(scoped in LogicalAnalysisContext context, scoped in CellMap pattern, int[] blocks)
	{
		scoped ref readonly var grid = ref context.Grid;
		foreach (var extraCell in pattern)
		{
			var otherCells = pattern - extraCell;
			var digitsMask = grid.GetDigitsUnion(otherCells);
			if (PopCount((uint)digitsMask) != 3)
			{
				continue;
			}

			var elimDigitsMask = (short)(grid.GetCandidates(extraCell) & digitsMask);
			if (elimDigitsMask == 0)
			{
				// No eliminations.
				continue;
			}

			var conclusions = new List<Conclusion>();
			foreach (var digit in elimDigitsMask)
			{
				conclusions.Add(new(Elimination, extraCell, digit));
			}

			var candidateOffsets = new List<CandidateViewNode>((12 - 1) * 3);
			foreach (var otherCell in otherCells)
			{
				foreach (var otherDigit in grid.GetCandidates(otherCell))
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
			if (context.OnlyFindOne)
			{
				return step;
			}

			context.Accumulator.Add(step);
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
	private IStep? CheckXz(scoped in LogicalAnalysisContext context, scoped in CellMap pattern, int[] blocks)
	{
		scoped ref readonly var grid = ref context.Grid;
		var allDigitsMask = grid.GetDigitsUnion(pattern);
		if (PopCount((uint)allDigitsMask) != 5)
		{
			// The pattern cannot find any possible eliminations because the number of extra digits
			// are not 2 will cause the extra digits not forming a valid strong link
			// as same behavior as ALS-XZ or BUG-XZ rule.
			return null;
		}

		foreach (var digits in allDigitsMask.GetAllSets().GetSubsets(3))
		{
			var patternDigitsMask = (short)(1 << digits[0] | 1 << digits[1] | 1 << digits[2]);
			var otherDigitsMask = (short)(allDigitsMask & ~patternDigitsMask);
			var d1 = TrailingZeroCount(otherDigitsMask);
			var d2 = otherDigitsMask.GetNextSet(d1);
			var otherDigitsCells = pattern & (CandidatesMap[d1] | CandidatesMap[d2]);
			if (otherDigitsCells is not [var c1, var c2])
			{
				continue;
			}

			foreach (var extraCell in (PeersMap[c1] ^ PeersMap[c2]) & BivalueCells)
			{
				if (grid.GetCandidates(extraCell) != otherDigitsMask)
				{
					continue;
				}

				// XZ rule found.
				var conclusions = new List<Conclusion>();
				var condition = (CellsMap[c1] + extraCell).InOneHouse;
				var anotherCell = condition ? c2 : c1;
				var anotherDigit = condition ? d1 : d2;
				foreach (var peer in (CellsMap[extraCell] + anotherCell).PeerIntersection)
				{
					if (CandidatesMap[anotherDigit].Contains(peer))
					{
						conclusions.Add(new(Elimination, peer, anotherDigit));
					}
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>(33);
				foreach (var patternCell in pattern)
				{
					foreach (var digit in grid.GetCandidates(patternCell))
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
				if (context.OnlyFindOne)
				{
					return step;
				}

				context.Accumulator.Add(step);
			}
		}

		return null;
	}
}
