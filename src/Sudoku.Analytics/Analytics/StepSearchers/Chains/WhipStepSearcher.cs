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
				var startNode = new Node((cell * 9 + digit).AsCandidateMap(), true, false);

				// Create a pending queue to record all interim cases, and a collection recording visited nodes.
				var pendingNodes = new LinkedList<(Node Node, Grid Grid)>();
				pendingNodes.AddLast((startNode, grid));

				// Iterate the pending queue and never stops, until all nodes are tried.
				while (pendingNodes.Count != 0)
				{
					// Deconstruct the object and apply digit into playground.
					var pair = pendingNodes.RemoveFirstNode();
					var currentNode = pair.Node;
					ref var playground = ref pair.Grid;
					playground.Apply(new(Assignment, currentNode.Map[0]));

					// And then collect all possible singles in the grid.
					var nextConclusions = GetNextConclusions(in playground);
					if (!nextConclusions)
					{
						// The current branch is failed. Just continue.
						continue;
					}

					// Here we should check for 2 kinds of contradictions:
					//   1) No candidates in one empty cell
					//   2) No possible positions of a digit in one house
					// If we can find out such contradiction, we can conclude that the start assertion is failed.
					if (ExistsContradiction(in playground, out var failedSpace))
					{
						// Contradiction is found. Now we can construct a step instance and return.
						; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ;
						// TODO: Implement later.
						; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ;
					}

					// If here, we will know that such conclusions are based on the previous conclusion applied.
					// Now we should append a parent relation. Here, I'll use a chain node to connect them.
					foreach (var nextConclusion in nextConclusions)
					{
						var nextNode = new Node(nextConclusion.AsCandidateMap(), true, false) >> currentNode;
						pendingNodes.AddLast((nextNode, playground));
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
		var housesMap = HousesMap;
		var candidatesMap = grid.CandidatesMap;
		for (var house = 0; house < 27; house++)
		{
			for (var digit = 0; digit < 9; digit++)
			{
				if (!(candidatesMap[digit] & housesMap[house]))
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
	/// <returns>All conclusions.</returns>
	private static CandidateMap GetNextConclusions(ref readonly Grid grid)
	{
		var emptyCells = grid.EmptyCells;
		var candidatesMap = grid.CandidatesMap;

		var result = CandidateMap.Empty;
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

				result.Add(fullHouseCell * 9 + Mask.Log2((Mask)(Grid.MaxCandidatesMask & ~appearedDigitsMask)));
			}

			// Check hidden singles.
			for (var digit = 0; digit < 9; digit++)
			{
				if ((candidatesMap[digit] & HousesMap[house]) is [var hiddenSingleCell])
				{
					result.Add(hiddenSingleCell * 9 + digit);
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
				result.Add(nakedSingleCell * 9 + digit);
			}
		}

		return result;
	}
}
