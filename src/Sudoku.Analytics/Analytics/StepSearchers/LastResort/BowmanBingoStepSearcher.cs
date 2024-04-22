namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Bowman's Bingo</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Bowman's Bingo</item>
/// </list>
/// </summary>
[StepSearcher("StepSearcherName_BowmanBingoStepSearcher", Technique.BowmanBingo)]
public sealed partial class BowmanBingoStepSearcher : StepSearcher
{
	/// <summary>
	/// The singles searcher.
	/// </summary>
	private static readonly SingleStepSearcher SinglesSearcher = new() { EnableFullHouse = true, EnableLastDigit = true };


	/// <summary>
	/// All temporary conclusions.
	/// </summary>
	private readonly List<Conclusion> _tempConclusions = [];


	/// <summary>
	/// Indicates the maximum length of the bowman bingo you want to search for. The maximum possible value is 64 (81 - 17).
	/// The default value is 32.
	/// </summary>
	[SettingItemName(SettingItemNames.BowmanBingoMaxLength)]
	public int MaxLength { get; set; } = 32;


	/// <inheritdoc/>
	protected internal override Step? Collect(ref AnalysisContext context)
	{
		var tempAccumulator = new List<BowmanBingoStep>();
		ref readonly var grid = ref context.Grid;
		var accumulator = context.Accumulator!;
		var onlyFindOne = context.OnlyFindOne;
		var tempGrid = grid;
		for (var digit = 0; digit < 9; digit++)
		{
			foreach (var cell in CandidatesMap[digit])
			{
				_tempConclusions.Add(new(Assignment, cell, digit));
				var (candList, mask) = RecordUndoInfo(in tempGrid, cell, digit);

				// Try to fill this cell.
				tempGrid.SetDigit(cell, digit);
				var startCandidate = cell * 9 + digit;

				if (IsValidGrid(in grid, cell))
				{
					Collect(tempAccumulator, ref tempGrid, ref context, onlyFindOne, startCandidate, MaxLength - 1);
				}
				else
				{
					var candidateOffsets = new CandidateViewNode[_tempConclusions.Count];
					var i = 0;
					foreach (var (_, candidate) in _tempConclusions)
					{
						candidateOffsets[i++] = new(ColorIdentifier.Normal, candidate);
					}

					tempAccumulator.Add(
						new BowmanBingoStep(
							[new(Elimination, startCandidate)],
							[[.. candidateOffsets, .. GetLinks()]],
							context.Options,
							[.. _tempConclusions]
						)
					);
				}

				// Undo the operation.
				_tempConclusions.RemoveAt(_tempConclusions.Count - 1);
				UndoGrid(ref tempGrid, candList, cell, mask);
			}
		}

		accumulator.AddRange(
			from info in tempAccumulator
			orderby info.ContradictionLinks.Length, info.ContradictionLinks[0]
			select info
		);

		return null;
	}

	/// <summary>
	/// <inheritdoc cref="Collect(ref AnalysisContext)" path="/summary"/>
	/// </summary>
	/// <param name="result">The accumulator instance to gather the result.</param>
	/// <param name="grid">The sudoku grid to be checked.</param>
	/// <param name="context">The context.</param>
	/// <param name="onlyFindOne"><inheritdoc cref="AnalysisContext.OnlyFindOne"/></param>
	/// <param name="startCand">The start candidate to be assumed.</param>
	/// <param name="length">The whole length to be searched.</param>
	/// <returns><inheritdoc cref="Collect(ref AnalysisContext)" path="/returns"/></returns>
	private BowmanBingoStep? Collect(
		List<BowmanBingoStep> result,
		ref Grid grid,
		ref AnalysisContext context,
		bool onlyFindOne,
		Candidate startCand,
		int length
	)
	{
		var context2 = new AnalysisContext(in grid)
		{
			OnlyFindOne = true,
			IsSukaku = context.IsSukaku,
			Options = context.Options
		};
		if (length == 0 || SinglesSearcher.Collect(ref context2) is not SingleStep { Conclusions: [{ Cell: var c, Digit: var d } conclusion, ..] })
		{
			// Two cases we don't need to go on.
			// Case 1: The variable 'length' is 0.
			// Case 2: The searcher can't get any new steps, which means the expression
			// always returns the value null. Therefore, this case (grid[cell] = digit) is a bad try.
			goto ReturnNull;
		}

		// Try to fill.
		_tempConclusions.Add(conclusion);
		var (candList, mask) = RecordUndoInfo(in grid, c, d);

		grid.SetDigit(c, d);
		if (IsValidGrid(in grid, c))
		{
			// Sounds good.
			if (Collect(result, ref grid, ref context2, onlyFindOne, startCand, length - 1) is { } nestedStep)
			{
				return nestedStep;
			}
		}
		else
		{
			var candidateOffsets = new CandidateViewNode[_tempConclusions.Count];
			var i = 0;
			foreach (var (_, candidate) in _tempConclusions)
			{
				candidateOffsets[i++] = new(ColorIdentifier.Normal, candidate);
			}

			var step = new BowmanBingoStep(
				[new(Elimination, startCand)],
				[[.. candidateOffsets, .. GetLinks()]],
				context.Options,
				[.. _tempConclusions]
			);
			if (onlyFindOne)
			{
				return step;
			}

			result.Add(step);
		}

		// Undo grid.
		_tempConclusions.RemoveAt(_tempConclusions.Count - 1);
		UndoGrid(ref grid, candList, c, mask);

	ReturnNull:
		return null;
	}

	/// <summary>
	/// Get links.
	/// </summary>
	/// <returns>The links.</returns>
	private List<LinkViewNode> GetLinks()
	{
		var result = new List<LinkViewNode>();
		for (var i = 0; i < _tempConclusions.Count - 1; i++)
		{
			var c1 = _tempConclusions[i].Candidate;
			var c2 = _tempConclusions[i + 1].Candidate;
			result.Add(new(ColorIdentifier.Normal, new(c1 % 9, [c1 / 9]), new(c2 % 9, [c2 / 9]), Inference.Default));
		}

		return result;
	}


	/// <summary>
	/// Record all information to be used in undo grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>The result.</returns>
	private static (List<Candidate> CandidateList, Mask Mask) RecordUndoInfo(ref readonly Grid grid, Cell cell, Digit digit)
	{
		var list = new List<Candidate>();
		foreach (var c in PeersMap[cell] & CandidatesMap[digit])
		{
			list.Add(c * 9 + digit);
		}

		return (list, grid[cell]);
	}

	/// <summary>
	/// Undo the grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="list">The list.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="mask">The mask.</param>
	private static void UndoGrid(ref Grid grid, List<Candidate> list, Cell cell, Mask mask)
	{
		foreach (var candidate in list)
		{
			grid.SetExistence(candidate / 9, candidate % 9, true);
		}

		grid.SetMask(cell, mask);
	}

	/// <summary>
	/// To check the specified cell has a same digit filled in a cell
	/// which is same house with the current one.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="cell">The cell.</param>
	/// <returns>The result.</returns>
	private static bool IsValidGrid(ref readonly Grid grid, Cell cell)
	{
		var result = true;
		foreach (var peerCell in PeersMap[cell])
		{
			var state = grid.GetState(peerCell);
			if ((state != CellState.Empty && grid.GetDigit(peerCell) != grid.GetDigit(cell) || state == CellState.Empty)
				&& grid.GetCandidates(peerCell) != 0)
			{
				continue;
			}

			result = false;
			break;
		}

		return result;
	}
}
