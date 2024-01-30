namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with an <b>H-Wing (Hybrid Wing)</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>H-Wing</item>
/// <item>Grouped H-Wing</item>
/// </list>
/// </summary>
[StepSearcher(Technique.HWing, Technique.GroupedHWing)]
[StepSearcherRuntimeName("StepSearcherName_HWingStepSearcher")]
public sealed partial class HWingStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="g/developer-notes"/>
	/// <para>
	/// A valid pattern of H-Wing is <c><![CDATA[(x=y)-(y=z)-z=z]]></c>, asymmetric.
	/// Please note that the head and tail of the chain is not of a same digit, meaning it will contain eliminations
	/// if those two cells share a same house.
	/// </para>
	/// <para>
	/// We can treat <c><![CDATA[(x=y)-(y=z)]]></c> as an ALS of digits X, Y and Z, in order to reduce redundant searching -
	/// this pattern will be reduce to <c><![CDATA[(ALS:x=z)-z=z]]></c>.
	/// </para>
	/// </remarks>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		if (BivalueCells.Count < 2)
		{
			// There is no valid H-Wings because a valid H-Wing will use 2 bi-value cells.
			return null;
		}

		scoped ref readonly var grid = ref context.Grid;

		// Search for all possible ALSes appeared in the grid, and arrange them by grouping them by houses.
		var alsLinks = new Dictionary<House, List<HWingAlmostLockedSetLinkInfo>>(27);
		var strongLinks = new IrregularWingStrongLinkEntry(243);
		for (var house = 0; house < 27; house++)
		{
			// Check for strong links first.
			for (var digit = 0; digit < 9; digit++)
			{
				if ((HousesMap[house] & CandidatesMap[digit]) is var possibleCells
					&& GroupedNode.IsGroupedStrongLink(in possibleCells, digit, house, out var spannedHouses)
					&& TrailingZeroCount(spannedHouses) is var firstHouse
					&& spannedHouses.GetNextSet(firstHouse) is var secondHouse
					&& (possibleCells & HousesMap[firstHouse], possibleCells & HousesMap[secondHouse]) is var (p, q)
					&& new StrongLinkInfo(house, in p, in q, spannedHouses) is var info
					&& !strongLinks.TryAdd((house, digit), [info]))
				{
					strongLinks[(house, digit)].Add(info);
				}
			}

			var bivalueCells = BivalueCells & HousesMap[house];
			if (bivalueCells.Count < 2)
			{
				// There is no way to combine cell-based ALSes.
				continue;
			}

			// Iterate on each pair of chosen cells, check whether they contain a common digit.
			// If they hold totally same digits (both 2 digits are same), they form a naked pair.
			foreach (ref readonly var pair in bivalueCells.GetSubsets(2))
			{
				var (p, q) = (pair[0], pair[1]);
				var commonCandidate = (Mask)(grid.GetCandidates(p) & grid.GetCandidates(q));
				if (commonCandidate == 0 || !IsPow2((uint)commonCandidate))
				{
					// They aren't a valid ALS.
					continue;
				}

				var commonDigit = Log2((uint)commonCandidate);
				var strongLink = new HWingAlmostLockedSetLinkInfo(
					commonDigit,
					(Mask)((Mask)(grid.GetCandidates(p) | grid.GetCandidates(q)) & (Mask)~(1 << commonDigit)),
					in pair
				);
				if (!alsLinks.TryAdd(house, [strongLink]))
				{
					alsLinks[house].Add(strongLink);
				}
			}
		}
		if (alsLinks.Count == 0)
		{
			// No available cells can be used.
			return null;
		}

		// Iterate on the dictionary, to check for existence of extra strong links.
		foreach (var supportGroupedNode in (false, true))
		{
			if (CollectCore(ref context, in grid, alsLinks, strongLinks, supportGroupedNode) is { } step)
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
	/// <param name="alsLinks">All possible ALS-related links.</param>
	/// <param name="strongLinks">Indicates the strong links.</param>
	/// <param name="supportsGroupedNode">Indicates whether the method calculate in grouped nodes.</param>
	/// <returns>The found step.</returns>
	private HWingStep? CollectCore(
		scoped ref AnalysisContext context,
		scoped ref readonly Grid grid,
		Dictionary<House, List<HWingAlmostLockedSetLinkInfo>> alsLinks,
		IrregularWingStrongLinkEntry strongLinks,
		bool supportsGroupedNode
	)
	{
		foreach (var house in alsLinks.Keys)
		{
			foreach (var (digitY, otherDigits, cells) in alsLinks[house])
			{
				var (c1, c2) = (cells[0], cells[1]);
				var d1 = TrailingZeroCount(otherDigits);
				var d2 = otherDigits.GetNextSet(d1);
				foreach (var (digitX, digitZ) in ((d1, d2), (d2, d1)))
				{
					// Check the cells, determining which one cell is related to the digit X and which one is to the digit Z.
					var xyCell = (grid.GetCandidates(c1) >> digitX & 1) != 0 ? c1 : c2;
					var yzCell = xyCell == c1 ? c2 : c1;

					// Iterate on strong links, determing whether a (grouped) node can form a weak link with 'xyCell'.
					for (var xyCellHouse = 0; xyCellHouse < 27; xyCellHouse++)
					{
						if (xyCellHouse == house)
						{
							// The strong link shouldn't be same as 'house'.
							continue;
						}

						if (!strongLinks.TryGetValue((xyCellHouse, digitX), out var validStrongLinksInXyCellHouse))
						{
							continue;
						}

						foreach (var (_, node1, node2, _) in validStrongLinksInXyCellHouse)
						{
							if (node1.Contains(xyCell) || node2.Contains(xyCell)
								|| node1.Contains(yzCell) || node2.Contains(yzCell))
							{
								continue;
							}

							foreach (var (connectedNode, startNode) in ((node1, node2), (node2, node1)))
							{
								// Check whether the 'connectedNode' forms a weak link with 'xyCell'.
								if (!connectedNode.PeerIntersection.Contains(xyCell))
								{
									continue;
								}

								if (startNode.Count != 1)
								{
									// If grouped, no eliminations will exist.
									continue;
								}

								if (supportsGroupedNode ^ connectedNode.Count != 1)
								{
									continue;
								}

								foreach (var startNodeCoveredHouse in startNode.CoveredHouses)
								{
									// Check whether the node 'startNode' and 'yzCell' in a same house.
									// If not, we cannot conclude anything.
									if (!HousesMap[startNodeCoveredHouse].Contains(yzCell))
									{
										continue;
									}

									// A possible H-Wing is found. Now check for eliminations.
									// The eliminations will be the digit Z in the 'startNode'.
									var elimMap = CandidatesMap[digitZ] & startNode;
									if (!elimMap)
									{
										// No eliminations found.
										continue;
									}

									var step = new HWingStep(
										[.. from cell in elimMap select new Conclusion(Elimination, cell, digitZ)],
										[
											[
												..
												from digit in grid.GetCandidates(xyCell)
												select new CandidateViewNode(ColorIdentifier.Normal, xyCell * 9 + digit),
												..
												from digit in grid.GetCandidates(yzCell)
												let identifier = digit == digitZ ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal
												select new CandidateViewNode(identifier, yzCell * 9 + digit),
												..
												from cell in connectedNode
												select new CandidateViewNode(ColorIdentifier.Normal, cell * 9 + digitX),
												..
												from cell in startNode
												select new CandidateViewNode(ColorIdentifier.Auxiliary1, cell * 9 + digitX),
												new CellViewNode(ColorIdentifier.Normal, xyCell),
												new CellViewNode(ColorIdentifier.Normal, yzCell),
												new HouseViewNode(ColorIdentifier.Normal, xyCellHouse)
											]
										],
										context.PredefinedOptions,
										xyCell,
										yzCell,
										digitX,
										digitY,
										digitZ,
										node1 | node2
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
		}

		return null;
	}
}
