namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Reverse Unique Rectangle</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Reverse Unique Rectangle Type 1</item>
/// <item>Reverse Unique Rectangle Type 2</item>
/// <!--
/// <item>Reverse Unique Rectangle Type 3</item>
/// <item>Reverse Unique Rectangle Type 4</item>
/// -->
/// </list>
/// </summary>
/// <!--
/// Test examples (May or may not be used):
/// 
/// 1) Split Reverse UR pairs
/// ......9...812...6.6.2.3.7.8...4...8....5.6....9...7...3.5.8.1.4.4...123...9......
/// -->
public interface IReverseUniqueRectangleStepSearcher : IReverseBivalueUniversalGraveStepSearcher
{
}

[StepSearcher]
internal sealed partial class ReverseBivalueUniversalGraveStepSearcher : IReverseUniqueRectangleStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		foreach (Cells pattern in UniqueRectanglePatterns)
		{
			if (pattern - EmptyCells is not { Count: >= 2 } nonemptyCells)
			{
				// No possible types will be found.
				continue;
			}

			// Gather digits used.
			short mask = grid.GetDigitsUnion(nonemptyCells);
			if (PopCount((uint)mask) != 2)
			{
				continue;
			}

			int d1 = TrailingZeroCount(mask), d2 = mask.GetNextSet(d1);
			short comparer = (short)(1 << d1 | 1 << d2);

			var emptyCells = pattern - nonemptyCells;
			short gathered = grid.GetDigitsUnion(emptyCells);
			if ((gathered & comparer) == 0)
			{
				continue;
			}

			if ((ValuesMap[d1] | ValuesMap[d2]) - pattern is not [])
			{
				// The grid contains any other cells that is a value cell of digit used.
				// In fact, we can continue checking the last cells recursively,
				// but that implementation is costly.
				continue;
			}

			if (emptyCells is [var emptyCell]
				&& CheckType1(accumulator, onlyFindOne, d1, d2, pattern, emptyCell, comparer) is { } type1Step)
			{
				return type1Step;
			}
			if (CheckType2(accumulator, onlyFindOne, grid, pattern, emptyCells, comparer) is { } type2Step)
			{
				return type2Step;
			}
		}

		return null;
	}

	/// <summary>
	/// Checks for type 1.
	/// </summary>
	private IStep? CheckType1(
		ICollection<IStep> accumulator, bool onlyFindOne, int d1, int d2, scoped in Cells pattern,
		int emptyCell, short comparer)
	{
		using scoped var conclusions = new ValueList<Conclusion>(1);
		if (CandidatesMap[d1].Contains(emptyCell))
		{
			conclusions.Add(new(Elimination, emptyCell, d1));
		}
		if (CandidatesMap[d2].Contains(emptyCell))
		{
			conclusions.Add(new(Elimination, emptyCell, d2));
		}
		if (conclusions is [])
		{
			return null;
		}

		var cellOffsets = new List<CellViewNode>(3);
		foreach (int cell in pattern - emptyCell)
		{
			cellOffsets.Add(new(DisplayColorKind.Normal, cell));
		}

		var step = new ReverseUniqueRectangleType1Step(
			conclusions.ToImmutableArray(),
			ImmutableArray.Create(View.Empty | cellOffsets),
			pattern,
			comparer,
			emptyCell,
			CandidatesMap[d1].Contains(emptyCell) ? d1 : d2
		);
		if (onlyFindOne)
		{
			return step;
		}

		accumulator.Add(step);
		return null;
	}

	/// <summary>
	/// Checks for type 2. In this type we also check the generalized case that looks like a UR type 5.
	/// </summary>
	private IStep? CheckType2(
		ICollection<IStep> accumulator, bool onlyFindOne, scoped in Grid grid,
		scoped in Cells pattern, scoped in Cells emptyCells, short comparer)
	{
		short mask = grid.GetDigitsUnion(emptyCells);
		short extraDigitMask = (short)(mask & ~comparer);
		if (!IsPow2(extraDigitMask))
		{
			return null;
		}

		int extraDigit = TrailingZeroCount(extraDigitMask);
		if (emptyCells % CandidatesMap[extraDigit] is not (var elimMap and not []))
		{
			return null;
		}

		scoped var conclusions = from cell in elimMap select new Conclusion(Elimination, cell, extraDigit);
		var cellOffsets = new List<CellViewNode>(4);
		foreach (int cell in pattern)
		{
			cellOffsets.Add(new(DisplayColorKind.Normal, cell));
		}
		var candidateOffsets = new List<CandidateViewNode>(4);
		foreach (int cell in emptyCells)
		{
			foreach (int digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add(
					new(
						digit == extraDigit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
						cell * 9 + digit
					)
				);
			}
		}

		var step = new ReverseUniqueRectangleType2Step(
			conclusions.ToImmutableArray(),
			ImmutableArray.Create(View.Empty | cellOffsets | candidateOffsets),
			pattern,
			comparer,
			extraDigit,
			emptyCells & CandidatesMap[extraDigit]
		);
		if (onlyFindOne)
		{
			return step;
		}

		accumulator.Add(step);
		return null;
	}
}
