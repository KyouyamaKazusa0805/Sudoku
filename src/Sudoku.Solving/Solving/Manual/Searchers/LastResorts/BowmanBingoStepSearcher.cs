namespace Sudoku.Solving.Manual.Searchers.LastResorts;

/// <summary>
/// Provides with a <b>Bowman's Bingo</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Bowman's Bingo</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe class BowmanBingoStepSearcher : IBowmanBingoStepSearcher
{
	/// <summary>
	/// The singles searcher.
	/// </summary>
	private readonly SingleStepSearcher _searcher = new() { EnableFullHouse = true, EnableLastDigit = true };

	/// <summary>
	/// All temporary conclusions.
	/// </summary>
	private readonly IList<Conclusion> _tempConclusions = new List<Conclusion>();


	/// <inheritdoc/>
	public int MaxLength { get; set; }

	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(19, DisplayingLevel.C)
	{
		EnabledAreas = EnabledAreas.None,
		DisabledReason = DisabledReason.LastResort | DisabledReason.TooSlow
	};


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		var tempAccumulator = new List<BowmanBingoStep>();
		var tempGrid = grid;
		for (int digit = 0; digit < 9; digit++)
		{
			foreach (int cell in CandMaps[digit])
			{
				_tempConclusions.Add(new(ConclusionType.Assignment, cell, digit));
				var (candList, mask) = RecordUndoInfo(tempGrid, cell, digit);

				// Try to fill this cell.
				tempGrid[cell] = digit;
				int startCandidate = cell * 9 + digit;

				if (IsValidGrid(grid, cell))
				{
					GetAll(tempAccumulator, ref tempGrid, onlyFindOne, startCandidate, MaxLength - 1);
				}
				else
				{
					var candidateOffsets = new (int, ColorIdentifier)[_tempConclusions.Count];
					int i = 0;
					foreach (var (_, candidate) in _tempConclusions)
					{
						candidateOffsets[i++] = (candidate, (ColorIdentifier)0);
					}

					tempAccumulator.Add(
						new BowmanBingoStep(
							ImmutableArray.Create(new Conclusion(ConclusionType.Elimination, startCandidate)),
							ImmutableArray.Create(new PresentationData
							{
								Candidates = candidateOffsets,
								Links = GetLinks()
							}),
							_tempConclusions.ToImmutableArray()
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

	private Step? GetAll(
		ICollection<BowmanBingoStep> result,
		ref Grid grid,
		bool onlyFindOne,
		int startCand,
		int length
	)
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
			var candidateOffsets = new (int, ColorIdentifier)[_tempConclusions.Count];
			int i = 0;
			foreach (var (_, candidate) in _tempConclusions)
			{
				candidateOffsets[i++] = (candidate, (ColorIdentifier)0);
			}

			var step = new BowmanBingoStep(
				ImmutableArray.Create(new Conclusion(ConclusionType.Elimination, startCand)),
				ImmutableArray.Create(new PresentationData
				{
					Candidates = candidateOffsets,
					Links = GetLinks()
				}),
				_tempConclusions.ToImmutableArray()
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
	private IList<(Link, ColorIdentifier)> GetLinks()
	{
		var result = new List<(Link, ColorIdentifier)>();
		for (int i = 0, iterationCount = _tempConclusions.Count - 1; i < iterationCount; i++)
		{
			int c1 = _tempConclusions[i].Candidate, c2 = _tempConclusions[i + 1].Candidate;
			result.Add((new(c1, c2, LinkKind.Default), (ColorIdentifier)0));
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
	private static (IReadOnlyList<int> CandidateList, short Mask) RecordUndoInfo(in Grid grid, int cell, int digit)
	{
		var list = new List<int>();
		foreach (int c in PeerMaps[cell] & CandMaps[digit])
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
	private static void UndoGrid(ref Grid grid, IReadOnlyList<int> list, int cell, short mask)
	{
		foreach (int cand in list)
		{
			grid[cand / 9, cand % 9] = true;
		}

		grid.SetMask(cell, mask);
	}

	/// <summary>
	/// To check the specified cell has a same digit filled in a cell
	/// which is same region with the current one.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="cell">The cell.</param>
	/// <returns>The result.</returns>
	private static bool IsValidGrid(in Grid grid, int cell)
	{
		bool result = true;
		foreach (int peerCell in Peers[cell])
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
