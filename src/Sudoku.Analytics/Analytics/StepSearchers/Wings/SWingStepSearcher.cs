namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with an <b>S-Wing (Split Wing)</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>S-Wing</item>
/// <item>Grouped S-Wing</item>
/// </list>
/// </summary>
[StepSearcher(Technique.SWing, Technique.GroupedSWing)]
[StepSearcherRuntimeName("StepSearcherName_SWingStepSearcher")]
public sealed partial class SWingStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="g/developer-notes"/>
	/// A valid pattern of S-Wing is <c><![CDATA[x=x-(x=y)-y=y]]></c>, symmetric.
	/// Please note that the head and tail of the chain is not of a same digit, meaning it will contain eliminations
	/// if those two cells share a same house.
	/// </remarks>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		if (!BivalueCells)
		{
			// A valid S-Wing should use one bi-value cell.
			return null;
		}

		// Cache for strong links.
		var strongLinks = new Dictionary<Digit, List<StrongLinkInfo>>();
		for (var house = 0; house < 27; house++)
		{
			for (var digit = 0; digit < 9; digit++)
			{
				if ((HousesMap[house] & CandidatesMap[digit]) is var cells
					&& Grouped.IsGroupedStrongLink(in cells, digit, house, out var spannedHousesList))
				{
					foreach (var spannedHouses in spannedHousesList)
					{
						if (TrailingZeroCount(spannedHouses) is var firstHouse
							&& spannedHouses.GetNextSet(firstHouse) is var secondHouse
							&& (cells & HousesMap[firstHouse], cells & HousesMap[secondHouse]) is var (p, q)
							&& new StrongLinkInfo(house, in p, in q, spannedHouses) is var link
							&& !strongLinks.TryAdd(digit, [link]))
						{
							strongLinks[digit].Add(link);
						}
					}
				}
			}
		}

		scoped ref readonly var grid = ref context.Grid;
		var iterableDigitsMask = (Mask)0;
		foreach (var digit in strongLinks.Keys)
		{
			iterableDigitsMask |= (Mask)(1 << digit);
		}

		if (PopCount((uint)iterableDigitsMask) < 2)
		{
			// A valid S-Wing will use 2 (grouped) strong links, with different digits.
			return null;
		}

		foreach (var supportsGroupedNode in (false, true))
		{
			if (CollectCore(ref context, in grid, supportsGroupedNode, iterableDigitsMask, strongLinks) is { } step)
			{
				return step;
			}
		}

		return null;
	}

	/// <summary>
	/// The internal collect method.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="supportsGroupedNode">Indicates whether the method calculate in grouped nodes.</param>
	/// <param name="iterableDigitsMask">The digits should be iterated.</param>
	/// <param name="strongLinks">All possible strong links.</param>
	/// <returns>The found step.</returns>
	private SWingStep? CollectCore(
		scoped ref AnalysisContext context,
		scoped ref readonly Grid grid,
		bool supportsGroupedNode,
		Mask iterableDigitsMask,
		Dictionary<Digit, List<StrongLinkInfo>> strongLinks
	)
	{
		// Iterate on two digits 'x' and 'y', to collect (grouped) strong links for two digits.
		foreach (var digitPair in iterableDigitsMask.GetAllSets().GetSubsets(2))
		{
			var (digitX, digitY) = (digitPair[0], digitPair[1]);
			foreach (var (house1, xNode1, xNode2, spannedHouses1) in strongLinks[digitX])
			{
				var containsGroupedNodesForDigitX = xNode1.Count * xNode2.Count != 1;
				if (!supportsGroupedNode && containsGroupedNodesForDigitX)
				{
					continue;
				}

				foreach (var (house2, yNode1, yNode2, spannedHouses2) in strongLinks[digitY])
				{
					var containsGroupedNodesForDigitY = yNode1.Count * yNode2.Count != 1;
					if (!supportsGroupedNode && containsGroupedNodesForDigitY
						|| supportsGroupedNode && !(containsGroupedNodesForDigitX || containsGroupedNodesForDigitY))
					{
						continue;
					}

					// A valid S-Wing must use two nodes that both can see a same bi-value cell,
					// this corresponds to the part 'x-(x=y)-y'.
					// So, just iterate all possible cases for the nodes, determining whether the nodes can see a same bi-value cell.
					foreach (var (xStart, xEnd) in ((xNode1, xNode2), (xNode2, xNode1)))
					{
						foreach (var (yStart, yEnd) in ((yNode1, yNode2), (yNode2, yNode1)))
						{
							// Check whether both cells 'xEnd' and 'yStart' can see a same bi-value cell containing the digits 'x' and 'y'.
							var possibleBivalueCells = ((xEnd | yStart).PeerIntersection & BivalueCells) - xStart - yEnd;
							possibleBivalueCells &= CandidatesMap[digitX] & CandidatesMap[digitY];
							if (!possibleBivalueCells)
							{
								continue;
							}

							// Check whether 'xStart' and 'yEnd' can lead to any possible conclusions. All cases:
							//
							//   1) If (node A).count > 1 and (node B).count > 1:
							//     There won't lead to any possible conclusions no matter what relation of position of those two nodes.
							//
							//   2) If ((node A).count > 1 and (node B).count = 1) or ((node A).count = 1 and (node B).count > 1):
							//   3) If (node A).count = 1 and (node B).count = 1:
							//     There will lead to conclusions if they share a same house.
							var conclusions = new List<Conclusion>(2);
							switch (xStart.Count, yEnd.Count)
							{
								case (1, 1): // Case 3
								{
									var (xCell, yCell) = (xStart[0], yEnd[0]);
									if (!PeersMap[xCell].Contains(yCell))
									{
										continue;
									}

									if ((grid.GetCandidates(xCell) >> digitY & 1) != 0)
									{
										conclusions.Add(new(Elimination, xCell, digitY));
									}
									if ((grid.GetCandidates(yCell) >> digitX & 1) != 0)
									{
										conclusions.Add(new(Elimination, yCell, digitX));
									}
									break;
								}
								case (1, not 1): // Case 2
								{
									var xCell = xStart[0];
									if (!yEnd.PeerIntersection.Contains(xCell) || yEnd.Contains(xCell))
									{
										continue;
									}

									if ((grid.GetCandidates(xCell) >> digitY & 1) != 0)
									{
										conclusions.Add(new(Elimination, xCell, digitY));
									}
									break;
								}
								case (not 1, 1): // Case 2
								{
									var yCell = yEnd[0];
									if (!xStart.PeerIntersection.Contains(yCell) || xStart.Contains(yCell))
									{
										continue;
									}

									if ((grid.GetCandidates(yCell) >> digitX & 1) != 0)
									{
										conclusions.Add(new(Elimination, yCell, digitX));
									}
									break;
								}
								case (not 1, not 1): // Case 1
								{
									// There is no way to produce eliminations.
									continue;
								}
							}
							if (conclusions.Count == 0)
							{
								// No eliminations can be found.
								continue;
							}

							// A possible S-Wing is found.

							// In fact, only one bi-value cell can be found in general, because they can see both side of chain nodes.
							// If 2 or more cells exist here, it will be a naked pair, making S-Wing patterns invalid.
							foreach (var midCell in possibleBivalueCells)
							{
								var step = new SWingStep(
									[.. conclusions],
									[
										[
											.. from cell in xStart select new CandidateViewNode(ColorIdentifier.Auxiliary1, cell * 9 + digitX),
											.. from cell in xEnd select new CandidateViewNode(ColorIdentifier.Normal, cell * 9 + digitX),
											.. from cell in yStart select new CandidateViewNode(ColorIdentifier.Normal, cell * 9 + digitY),
											.. from cell in yEnd select new CandidateViewNode(ColorIdentifier.Auxiliary1, cell * 9 + digitY),
											..
											from digit in grid.GetCandidates(midCell)
											select new CandidateViewNode(ColorIdentifier.Normal, midCell * 9 + digit),
											new HouseViewNode(ColorIdentifier.Normal, house1),
											new HouseViewNode(ColorIdentifier.Normal, house2),
											new CellViewNode(ColorIdentifier.Normal, midCell)
										]
									],
									context.PredefinedOptions,
									in xNode1,
									in xNode2,
									in yNode1,
									in yNode2,
									digitX,
									digitY,
									midCell
								);
								if (context.OnlyFindOne)
								{
									return step;
								}

								context.Accumulator.Add(step);
							}
						}
					}
				}
			}
		}

		return null;
	}
}
