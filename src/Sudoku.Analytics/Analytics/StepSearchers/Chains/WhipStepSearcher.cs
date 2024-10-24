namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Whip</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Whip</item>
/// </list>
/// </summary>
[StepSearcher("StepSearcherName_WhipStepSearcher", Technique.Whip)]
public sealed partial class WhipStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(ref StepAnalysisContext context)
	{
		ref readonly var grid = ref context.Grid;

		// Iterate on each candidate that can be asserted as false in solution.
		foreach (var cell in EmptyCells)
		{
			var trueDigit = Solution.IsUndefined ? -1 : Solution.GetDigit(cell);
			foreach (var digit in grid.GetCandidates(cell))
			{
				if (trueDigit == digit)
				{
					// Skip for the true digits.
					continue;
				}

				// If the digit is not correct in solution, we can try to suppose it can be eliminated, and find any contradictions.
				// The basic algorithm is simply find try & error - just suppose it is true,
				// and then find the next true digit, and continue.
				var startCandidate = cell * 9 + digit;

				// Create a pending queue to record all interim cases, and a collection recording visited nodes.
				var pendingNodes = new LinkedList<WhipNode>();
				pendingNodes.AddLast(new WhipNode(startCandidate, in grid));

				// Iterate the pending queue and never stops, until all nodes are tried.
				while (pendingNodes.Count != 0)
				{
					// Deconstruct the object and apply digit into playground.
					var currentNode = pendingNodes.RemoveFirstNode();
					var currentCandidate = currentNode.Assignment.Candidate;
					ref var playground = ref currentNode.Grid;
					playground.Apply(new(Assignment, currentCandidate));

					// Here we should check for 2 kinds of contradictions:
					//   1) No candidates in one empty cell
					//   2) No possible positions of a digit in one house
					// If we can find out such contradiction, we can conclude that the start assertion is failed.
					if (ExistsContradiction(in playground, out var failedSpace))
					{
						// Contradiction is found. Now we can construct a step instance and return.
						var step = CreateStep(in context, currentNode, startCandidate, in grid, failedSpace);
						if (context.OnlyFindOne)
						{
							return step;
						}

						context.Accumulator.Add(step);
						break;
					}

					// And then collect all possible singles in the grid.
					var nextAssignments = GetNextAssignments(in playground);
					if (nextAssignments.Length == 0)
					{
						// The current branch is failed. Just continue.
						continue;
					}

					// If here, we will know that such conclusions are based on the previous conclusion applied.
					// Now we should append a parent relation. Here, I'll use a chain node to connect them.
					foreach (var assignment in nextAssignments)
					{
						pendingNodes.AddLast(new WhipNode(assignment, in playground) >> currentNode);
					}
				}
			}
		}

		// No conclusions found, or just find for all possible steps. Return null.
		return null;
	}


	/// <summary>
	/// Determine whether the specified grid state contains any contradiction.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="failedSpace">The failed space.</param>
	/// <returns>A <see cref="bool"/> result whether any contradiction is found.</returns>
	private static unsafe bool ExistsContradiction(ref readonly Grid grid, out Space failedSpace)
	{
		// Check for cell.
		foreach (var cell in grid.EmptyCells)
		{
			if (grid.GetCandidates(cell) == 0)
			{
				failedSpace = Space.RowColumn(cell / 9, cell % 9);
				return true;
			}
		}

		// Check for house.
		var emptyCells = grid.EmptyCells;
		var candidatesMap = grid.CandidatesMap;
		for (var house = 0; house < 27; house++)
		{
			foreach (var digit in grid[HousesMap[house] & emptyCells])
			{
				if (!(candidatesMap[digit] & HousesMap[house]))
				{
					delegate*<int, int, Space> spaceCreator = house switch
					{
						< 9 => &Space.BlockNumber,
						< 18 => &Space.RowNumber,
						_ => &Space.ColumnNumber
					};
					failedSpace = spaceCreator(house, digit);
					return true;
				}
			}
		}

		// No conclusions found.
		failedSpace = default;
		return false;
	}

	/// <summary>
	/// Try to find all possible conclusions inside the grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>All conclusions and reason why the assignment raised.</returns>
	private static ReadOnlySpan<WhipAssignment> GetNextAssignments(ref readonly Grid grid)
	{
		var emptyCells = grid.EmptyCells;
		var candidatesMap = grid.CandidatesMap;

		var result = new List<WhipAssignment>();
		for (var house = 0; house < 27; house++)
		{
			// Check for full houses.
			if ((HousesMap[house] & emptyCells) is [var fullHouseCell])
			{
				// Check for the missing digit.
				var appearedDigitsMask = (Mask)0;
				foreach (var cell in HousesMap[house])
				{
					if (!emptyCells.Contains(cell))
					{
						appearedDigitsMask |= (Mask)(1 << grid.GetDigit(cell));
					}
				}
				result.Add(new(fullHouseCell * 9 + Mask.Log2((Mask)(Grid.MaxCandidatesMask & ~appearedDigitsMask)), Technique.FullHouse));
			}

			// Check hidden singles.
			for (var digit = 0; digit < 9; digit++)
			{
				if ((candidatesMap[digit] & HousesMap[house]) is [var hiddenSingleCell])
				{
					result.Add(
						new(
							hiddenSingleCell * 9 + digit,
							house switch
							{
								< 9 => Technique.CrosshatchingBlock,
								< 18 => Technique.CrosshatchingRow,
								_ => Technique.CrosshatchingColumn
							}
						)
					);
				}
			}
		}

		// Check naked singles.
		foreach (var nakedSingleCell in emptyCells)
		{
			var digitsMask = grid.GetCandidates(nakedSingleCell);
			if (Mask.IsPow2(digitsMask))
			{
				var digit = Mask.Log2(digitsMask);
				result.Add(new(nakedSingleCell * 9 + digit, Technique.NakedSingle));
			}
		}
		return result.AsSpan();
	}

	/// <summary>
	/// Create a <see cref="WhipStep"/> instance via the current confliction rule.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="contradictionNode">The node that makes such contradiction.</param>
	/// <param name="startCandidate">Indicates the start candidate.</param>
	/// <param name="initialGrid">Indicates the initial grid.</param>
	/// <param name="failedSpace">Indicates the space indicating where such contradiction is raised.</param>
	/// <returns>The final <see cref="WhipStep"/> instance.</returns>
	private static WhipStep CreateStep(
		ref readonly StepAnalysisContext context,
		WhipNode contradictionNode,
		Candidate startCandidate,
		ref readonly Grid initialGrid,
		Space failedSpace
	)
	{
		var candidateOffsets = new List<CandidateViewNode>();
		var linkOffsets = new List<ChainLinkViewNode>();
		for (var node = contradictionNode; node is not null; node = node.Parent)
		{
			if (node.Assignment.Candidate != startCandidate)
			{
				// Skip for the start candidate on purpose.
				candidateOffsets.Add(new(ColorIdentifier.Normal, node.Assignment.Candidate));
			}

			if (node.Parent is { Assignment.Candidate: var parentCandidate })
			{
				linkOffsets.Add(
					new(
						ColorIdentifier.Normal,
						parentCandidate.AsCandidateMap(),
						node.Assignment.Candidate.AsCandidateMap(),
						false
					)
				);
			}
		}

		return new(
			new SingletonArray<Conclusion>(new(Elimination, startCandidate)),
			[
				[
					.. candidateOffsets,
					.. linkOffsets,
					.. failedSpace.Type == SpaceType.RowColumn
						? [new CellViewNode(ColorIdentifier.Normal, failedSpace.Row * 9 + failedSpace.Column)]
						: ReadOnlySpan<ViewNode>.Empty,
					.. failedSpace.Type != SpaceType.RowColumn
						? [
							new HouseViewNode(ColorIdentifier.Normal, failedSpace.House),
							..
							from cell in HousesMap[failedSpace.House] & CandidatesMap[failedSpace.Digit]
							select new CandidateViewNode(ColorIdentifier.Normal, cell * 9 + failedSpace.Digit)
						]
						: ReadOnlySpan<ViewNode>.Empty
				]
			],
			context.Options,
			ReadOnlyMemory<Space>.Empty,
			ReadOnlyMemory<Space>.Empty
		);
	}
}
