namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Reverse Unique Rectangle</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Reverse Unique Rectangle Type 1</item>
/// <!--
/// <item>Reverse Unique Rectangle Type 2</item>
/// <item>Reverse Unique Rectangle Type 3</item>
/// <item>Reverse Unique Rectangle Type 4</item>
/// -->
/// </list>
/// </summary>
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

			if (pattern - nonemptyCells is not [var emptyCell])
			{
				continue;
			}

			int d1 = TrailingZeroCount(mask), d2 = mask.GetNextSet(d1);
			short comparer = (short)(1 << d1 | 1 << d2);
			short gathered = grid.GetCandidates(emptyCell);
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

			if (CheckType1(accumulator, onlyFindOne, d1, d2, pattern, emptyCell, comparer) is { } type1Step)
			{
				return type1Step;
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
}
