namespace Sudoku.Solving.Implementations.Searchers;

[StepSearcher]
internal sealed unsafe partial class BowmanBingoStepSearcher : IBowmanBingoStepSearcher
{
	/// <summary>
	/// The singles searcher.
	/// </summary>
	private readonly ISingleStepSearcher _searcher = new SingleStepSearcher { EnableFullHouse = true, EnableLastDigit = true };

	/// <summary>
	/// All temporary conclusions.
	/// </summary>
	private readonly IList<Conclusion> _tempConclusions = new List<Conclusion>();


	/// <inheritdoc/>
	[StepSearcherProperty]
	public int MaxLength { get; set; }


	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		var tempAccumulator = new List<BowmanBingoStep>();
		var tempGrid = grid;
		for (var digit = 0; digit < 9; digit++)
		{
			foreach (var cell in CandidatesMap[digit])
			{
				_tempConclusions.Add(new(Assignment, cell, digit));
				var (candList, mask) = RecordUndoInfo(tempGrid, cell, digit);

				// Try to fill this cell.
				tempGrid[cell] = digit;
				var startCandidate = cell * 9 + digit;

				if (IsValidGrid(grid, cell))
				{
					GetAll(tempAccumulator, ref tempGrid, onlyFindOne, startCandidate, MaxLength - 1);
				}
				else
				{
					var candidateOffsets = new CandidateViewNode[_tempConclusions.Count];
					var i = 0;
					foreach (var (_, candidate) in _tempConclusions)
					{
						candidateOffsets[i++] = new(DisplayColorKind.Normal, candidate);
					}

					tempAccumulator.Add(
						new BowmanBingoStep(
							ImmutableArray.Create(new Conclusion(Elimination, startCandidate)),
							ImmutableArray.Create(View.Empty | candidateOffsets | GetLinks()),
							ImmutableArray.CreateRange(_tempConclusions)
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

	private IStep? GetAll(
		ICollection<BowmanBingoStep> result, scoped ref Grid grid, bool onlyFindOne, int startCand, int length)
	{
		if (length == 0 || _searcher.GetAll(null!, grid, true) is not SingleStep singleInfo)
		{
			// Two cases we don't need to go on.
			// Case 1: the variable 'length' is 0.
			// Case 2: The searcher can't get any new steps, which means the expression
			// always returns the value null. Therefore, this case (grid[cell] = digit)
			// is a bad try.
			goto ReturnNull;
		}

		// Try to fill.
		var conclusion = singleInfo.Conclusions[0];
		_tempConclusions.Add(conclusion);
		_ = conclusion is { Cell: var c, Digit: var d };
		var (candList, mask) = RecordUndoInfo(grid, c, d);

		grid[c] = d;
		if (IsValidGrid(grid, c))
		{
			// Sounds good.
			if (GetAll(result, ref grid, onlyFindOne, startCand, length - 1) is { } nestedStep)
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
				candidateOffsets[i++] = new(DisplayColorKind.Normal, candidate);
			}

			var step = new BowmanBingoStep(
				ImmutableArray.Create(new Conclusion(Elimination, startCand)),
				ImmutableArray.Create(View.Empty | candidateOffsets | GetLinks()),
				ImmutableArray.CreateRange(_tempConclusions)
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
	private IList<LinkViewNode> GetLinks()
	{
		var result = new List<LinkViewNode>();
		for (int i = 0, iterationCount = _tempConclusions.Count - 1; i < iterationCount; i++)
		{
			int c1 = _tempConclusions[i].Candidate, c2 = _tempConclusions[i + 1].Candidate;
			result.Add(new(DisplayColorKind.Normal, new(c1 % 9, c1 / 9), new(c2 % 9, c2 / 9), Inference.Default));
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
	private static (IReadOnlyList<int> CandidateList, short Mask) RecordUndoInfo(scoped in Grid grid, int cell, int digit)
	{
		var list = new List<int>();
		foreach (var c in PeersMap[cell] & CandidatesMap[digit])
		{
			list.Add(c * 9 + digit);
		}

		return (list, grid.GetMask(cell));
	}

	/// <summary>
	/// Undo the grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="list">The list.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="mask">The mask.</param>
	private static void UndoGrid(scoped ref Grid grid, IReadOnlyList<int> list, int cell, short mask)
	{
		foreach (var cand in list)
		{
			grid[cand / 9, cand % 9] = true;
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
	private static bool IsValidGrid(scoped in Grid grid, int cell)
	{
		var result = true;
		foreach (var peerCell in Peers[cell])
		{
			var status = grid.GetStatus(peerCell);
			if (!(status != CellStatus.Empty && grid[peerCell] != grid[cell] || status == CellStatus.Empty)
				|| grid.GetCandidates(peerCell) == 0)
			{
				result = false;
				break;
			}
		}

		return result;
	}
}
