namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Whip</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Whip</item>
/// </list>
/// </summary>
[StepSearcher("StepSearcherName_WhipStepSearcher", Technique.Whip, RuntimeFlags = StepSearcherRuntimeFlags.SpaceComplexity)]
public sealed partial class WhipStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates the maximum length of whip chains.
	/// </summary>
	public int MaxLength { get; set; } = 10;


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
				pendingNodes.AddLast(new WhipNode(startCandidate));

				// Iterate the pending queue and never stops, until all nodes are tried.
				while (pendingNodes.Count != 0)
				{
					// Deconstruct the object and apply digit into playground.
					var currentNode = pendingNodes.RemoveFirstNode();

					// Here we should check for 2 kinds of contradictions:
					//   1) No candidates in one empty cell
					//   2) No possible positions of a digit in one house
					// If we can find out such contradiction, we can conclude that the start assertion is failed.
					if (ExistsContradiction(grid, currentNode, out var failedSpace))
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

					// If here, we will know that such conclusions are based on the previous conclusion applied.
					// Now we should append a parent relation. Here, I'll use a chain node to connect them.
					if (((IParentLinkedNode<WhipNode>)currentNode).AncestorsLength > MaxLength)
					{
						continue;
					}

					foreach (var assignment in GetNextAssignments(grid, currentNode))
					{
						pendingNodes.AddLast(new WhipNode(assignment) >> currentNode);
					}
				}
			}
		}

		// No conclusions found, or just find for all possible steps. Return null.
		return null;
	}


	/// <summary>
	/// Try to assign the grid with all conclusions produced by the specified whip chain.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="currentNode">The node.</param>
	private static void UpdateGrid(ref Grid grid, WhipNode currentNode)
	{
		for (var node = currentNode; node is not null; node = node.Parent)
		{
			grid.Apply(new(Assignment, node.Assignment.Candidate));
		}
	}

	/// <summary>
	/// Determine whether the specified grid state contains any contradiction.
	/// </summary>
	/// <param name="playground">The grid.</param>
	/// <param name="currentNode">Indicates the current node.</param>
	/// <param name="failedSpace">The failed space.</param>
	/// <returns>A <see cref="bool"/> result whether any contradiction is found.</returns>
	private static bool ExistsContradiction(Grid playground, WhipNode currentNode, out Space failedSpace)
	{
		// Temporarily update the grid in order not to cache the full grid.
		UpdateGrid(ref playground, currentNode);

		// Check for cell.
		foreach (var cell in playground.EmptyCells)
		{
			if (playground.GetCandidates(cell) == 0)
			{
				failedSpace = Space.RowColumn(cell / 9, cell % 9);
				return true;
			}
		}

		// Check for house.
		var candidatesMap = playground.CandidatesMap;
		for (var house = 0; house < 27; house++)
		{
			foreach (var digit in playground[HousesMap[house], false])
			{
				if (!(candidatesMap[digit] & HousesMap[house]))
				{
					failedSpace = house switch
					{
						< 9 => Space.BlockNumber(house, digit),
						< 18 => Space.RowNumber(house - 9, digit),
						_ => Space.ColumnNumber(house - 18, digit)
					};
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
	/// <param name="playground">The grid.</param>
	/// <param name="currentNode">Indicates the current node.</param>
	/// <returns>All conclusions and reason why the assignment raised.</returns>
	private static ReadOnlySpan<WhipAssignment> GetNextAssignments(Grid playground, WhipNode currentNode)
	{
		// Temporarily update the grid in order not to cache the full grid.
		UpdateGrid(ref playground, currentNode);

		var emptyCells = playground.EmptyCells;
		var candidatesMap = playground.CandidatesMap;

		var result = new List<WhipAssignment>();
		var concludedCells = CellMap.Empty;
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
						appearedDigitsMask |= (Mask)(1 << playground.GetDigit(cell));
					}
				}

				var targetCandidate = fullHouseCell * 9 + Mask.Log2((Mask)(Grid.MaxCandidatesMask & ~appearedDigitsMask));
				if (concludedCells.Add(fullHouseCell))
				{
					result.Add(new(targetCandidate, Technique.FullHouse));
				}
			}

			// Check hidden singles.
			for (var digit = 0; digit < 9; digit++)
			{
				if ((candidatesMap[digit] & HousesMap[house]) is [var hiddenSingleCell])
				{
					var targetCandidate = hiddenSingleCell * 9 + digit;
					if (concludedCells.Add(hiddenSingleCell))
					{
						result.Add(
							new(
								targetCandidate,
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
		}

		// Check naked singles.
		foreach (var nakedSingleCell in emptyCells)
		{
			var digitsMask = playground.GetCandidates(nakedSingleCell);
			if (Mask.IsPow2(digitsMask))
			{
				var digit = Mask.Log2(digitsMask);
				var targetCandidate = nakedSingleCell * 9 + digit;
				if (concludedCells.Add(nakedSingleCell))
				{
					result.Add(new(targetCandidate, Technique.NakedSingle));
				}
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
		return new(
			new SingletonArray<Conclusion>(new(Elimination, startCandidate)),
			getViews(in initialGrid, out var burredCandidates, out var truths, out var links),
			context.Options,
			truths,
			links
		);


		View[] getViews(
			ref readonly Grid initialGrid,
			out CandidateMap burredCandidates,
			out ReadOnlyMemory<Space> truths,
			out ReadOnlyMemory<Space> links
		)
		{
			var candidateOffsets = new HashSet<CandidateViewNode>();
			var linkOffsets = new List<ChainLinkViewNode>();
			for (var node = contradictionNode; node is not null; node = node.Parent)
			{
				var currentCandidate = node.Assignment.Candidate;
				var reason = node.Assignment.Reason;
				candidateOffsets.Add(new(ColorIdentifier.Normal, node.Assignment.Candidate));

				// Due to design of this algorithm, we should append extra strong and weak links between two assignments.
				// Please note that the links are reversed, we should make a reversion
				// in order to keep the chain and node states correct.
				if (node.Parent is { Assignment.Candidate: var parentCandidate } && reason != Technique.None)
				{
					switch (reason)
					{
						case Technique.FullHouse or Technique.NakedSingle:
						{
							var interimCandidate = currentCandidate / 9 * 9 + parentCandidate % 9;
							candidateOffsets.Add(new(ColorIdentifier.Normal, parentCandidate));
							candidateOffsets.Add(new(ColorIdentifier.Normal, currentCandidate));
							candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, interimCandidate));

							linkOffsets.Add(
								new(
									ColorIdentifier.Normal,
									parentCandidate.AsCandidateMap(),
									interimCandidate.AsCandidateMap(),
									false
								)
							);
							linkOffsets.Add(
								new(
									ColorIdentifier.Normal,
									interimCandidate.AsCandidateMap(),
									currentCandidate.AsCandidateMap(),
									true
								)
							);
							break;
						}
						case var type and (Technique.CrosshatchingBlock or Technique.CrosshatchingRow or Technique.CrosshatchingColumn):
						{
							candidateOffsets.Add(new(ColorIdentifier.Normal, parentCandidate));
							candidateOffsets.Add(new(ColorIdentifier.Normal, currentCandidate));

							var currentCell = currentCandidate / 9;
							var currentDigit = currentCandidate % 9;
							var houseType = type switch
							{
								Technique.CrosshatchingBlock => HouseType.Block,
								Technique.CrosshatchingRow => HouseType.Row,
								_ => HouseType.Column
							};

							if (!PeersMap[parentCandidate / 9].Contains(currentCell))
							{
								var groupedCells = ((parentCandidate / 9).AsCellMap() + currentCell).PeerIntersection
									& HousesMap[currentCell.ToHouse(houseType)]
									& CandidatesMap[currentDigit];
								foreach (var cell in groupedCells)
								{
									candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, cell * 9 + currentDigit));
								}

								var interimMap = (from cell in groupedCells select cell * 9 + currentDigit).AsCandidateMap();
								linkOffsets.Add(new(ColorIdentifier.Normal, parentCandidate.AsCandidateMap(), interimMap, false));
								linkOffsets.Add(new(ColorIdentifier.Normal, interimMap, currentCandidate.AsCandidateMap(), true));
							}
							else
							{
								var interimCandidate = parentCandidate / 9 * 9 + currentDigit;
								candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, interimCandidate));
								linkOffsets.Add(
									new(
										ColorIdentifier.Normal,
										parentCandidate.AsCandidateMap(),
										interimCandidate.AsCandidateMap(),
										false
									)
								);
								linkOffsets.Add(
									new(
										ColorIdentifier.Normal,
										interimCandidate.AsCandidateMap(),
										currentCandidate.AsCandidateMap(),
										true
									)
								);
							}
							break;
						}
					}
				}
			}

			// Remove the node that is start.
			candidateOffsets.RemoveWhere(node => node.Candidate == startCandidate);

			var tryAndErrorCandidateOffsets = new List<CandidateViewNode>();
			var tryAndErrorLinkOffsets = new List<ChainLinkViewNode>();
			for (var node = contradictionNode; node is not null; node = node.Parent)
			{
				if (node.Assignment.Candidate != startCandidate)
				{
					// Skip for the start candidate on purpose.
					tryAndErrorCandidateOffsets.Add(new(ColorIdentifier.Normal, node.Assignment.Candidate));
				}

				if (node.Parent is { Assignment.Candidate: var parentCandidate })
				{
					tryAndErrorLinkOffsets.Add(
						new(
							ColorIdentifier.Normal,
							parentCandidate.AsCandidateMap(),
							node.Assignment.Candidate.AsCandidateMap(),
							false
						)
					);
				}
			}

			// Contradiction-related view nodes.
			ReadOnlySpan<ViewNode> contradictionViewNodes = [
				.. failedSpace.Type == SpaceType.RowColumn
					? [
						new CellViewNode(ColorIdentifier.Auxiliary1, failedSpace.Cell),
						..
						from digit in initialGrid.GetCandidates(failedSpace.Cell)
						select new CandidateViewNode(ColorIdentifier.Auxiliary1, failedSpace.Cell * 9 + digit)
					]
					: ReadOnlySpan<ViewNode>.Empty,
				.. failedSpace.Type != SpaceType.RowColumn
					? [
						new HouseViewNode(ColorIdentifier.Auxiliary1, failedSpace.House),
						..
						from cell in HousesMap[failedSpace.House] & CandidatesMap[failedSpace.Digit]
						select new CandidateViewNode(ColorIdentifier.Auxiliary1, cell * 9 + failedSpace.Digit)
					]
					: ReadOnlySpan<ViewNode>.Empty
			];

			burredCandidates = analyzeBurredCandidates(linkOffsets, in initialGrid, out truths, out links);
			var missingCandidateOffsets = new List<CandidateViewNode>();
			foreach (var candidate in burredCandidates)
			{
				missingCandidateOffsets.Add(new(ColorIdentifier.Auxiliary2, candidate));
			}

			return [
				[.. candidateOffsets, .. linkOffsets, .. contradictionViewNodes, .. missingCandidateOffsets],
				[.. tryAndErrorCandidateOffsets, .. tryAndErrorLinkOffsets, .. contradictionViewNodes]
			];
		}

		static CandidateMap analyzeBurredCandidates(
			List<ChainLinkViewNode> linkOffsets,
			ref readonly Grid initialGrid,
			out ReadOnlyMemory<Space> truths,
			out ReadOnlyMemory<Space> links
		)
		{
			var truthsList = new List<Space>();
			var linksList = new List<Space>();
			var result = CandidateMap.Empty;
			foreach (var link in linkOffsets)
			{
				// Analyzes the link type.
				// Because of design of algorithm, there're only 4 kinds of links used,
				// and can exactly corresponds to 4 kinds of spaces.
				var space = (link.Start, link.End) switch
				{
					({ Digits: var d1, Cells: [var c1] }, { Digits: var d2, Cells: [var c2] }) when d1 != d2 && c1 == c2
						=> Space.RowColumn(c1 / 9, c1 % 9),
					({ Digits: var d1, Cells: var c1 }, { Digits: var d2, Cells: var c2 })
					when d1 == d2 && Mask.IsPow2(d1) && Mask.Log2(d1) is var digit && (c1 | c2).FirstSharedHouse is var house
						=> house switch
						{
							< 9 => Space.BlockNumber(house, digit),
							< 18 => Space.RowNumber(house - 9, digit),
							_ => Space.ColumnNumber(house - 18, digit)
						}
				};
				(link.IsStrongLink ? truthsList : linksList).Add(space);

				if (link.IsStrongLink)
				{
					// If the link is strong, we should check which candidates are not hold for the link in the target space.
					if (space.Type == SpaceType.RowColumn)
					{
						var missingDigits = initialGrid.GetCandidates(space.Cell) & ~link.Start.Digits & ~link.End.Digits;
						foreach (var digit in missingDigits)
						{
							result.Add(space.Cell * 9 + digit);
						}
					}
					else
					{
						var missingCells = HousesMap[space.House] & CandidatesMap[space.Digit] & ~link.Start.Cells & ~link.End.Cells;
						foreach (var cell in missingCells)
						{
							result.Add(cell * 9 + space.Digit);
						}
					}
				}
			}

			(truths, links) = (truthsList.AsMemory(), linksList.AsMemory());
			return result;
		}
	}
}
