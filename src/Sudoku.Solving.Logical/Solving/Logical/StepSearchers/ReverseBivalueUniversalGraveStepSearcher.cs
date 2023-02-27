namespace Sudoku.Solving.Logical.StepSearchers;

[StepSearcher]
[StepSearcherRunningOptions(StepSearcherRunningOptions.OnlyForStandardSudoku)]
internal sealed partial class ReverseBivalueUniversalGraveStepSearcher : IReverseUniqueRectangleStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(scoped ref LogicalAnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		var accumulator = context.Accumulator!;
		var onlyFindOne = context.OnlyFindOne;
		foreach (CellMap pattern in UniqueRectanglePatterns)
		{
			if (pattern - EmptyCells is not { Count: >= 2 } nonemptyCells)
			{
				// No possible types will be found.
				continue;
			}

			// Gather digits used.
			var mask = grid.GetDigitsUnion(nonemptyCells);
			if (PopCount((uint)mask) != 2)
			{
				continue;
			}

			int d1 = TrailingZeroCount(mask), d2 = mask.GetNextSet(d1);
			var comparer = (short)(1 << d1 | 1 << d2);

			var emptyCells = pattern - nonemptyCells;
			var gathered = grid.GetDigitsUnion(emptyCells);
			if ((gathered & comparer) == 0)
			{
				continue;
			}

			if ((ValuesMap[d1] | ValuesMap[d2]) - pattern)
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
		ICollection<IStep> accumulator, bool onlyFindOne, int d1, int d2, scoped in CellMap pattern,
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
		foreach (var cell in pattern - emptyCell)
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
		scoped in CellMap pattern, scoped in CellMap emptyCells, short comparer)
	{
		var mask = grid.GetDigitsUnion(emptyCells);
		var extraDigitMask = (short)(mask & ~comparer);
		if (!IsPow2(extraDigitMask))
		{
			return null;
		}

		var extraDigit = TrailingZeroCount(extraDigitMask);
		if (emptyCells % CandidatesMap[extraDigit] is not (var elimMap and not []))
		{
			return null;
		}

		var cellOffsets = new List<CellViewNode>(4);
		foreach (var cell in pattern)
		{
			cellOffsets.Add(new(DisplayColorKind.Normal, cell));
		}
		var candidateOffsets = new List<CandidateViewNode>(4);
		foreach (var cell in emptyCells)
		{
			foreach (var digit in grid.GetCandidates(cell))
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
			from cell in elimMap select new Conclusion(Elimination, cell, extraDigit),
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
