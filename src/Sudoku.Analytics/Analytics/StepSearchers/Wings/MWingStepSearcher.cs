namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with an <b>M-Wing (Medusa Wing)</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>M-Wing</item>
/// <item>Grouped M-Wing</item>
/// </list>
/// </summary>
[StepSearcher(Technique.MWing, Technique.GroupedMWing)]
[StepSearcherRuntimeName("StepSearcherName_MWingStepSearcher")]
public sealed partial class MWingStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="g/developer-notes"/>
	/// A valid pattern of M-Wing is <c><![CDATA[(x=y)-y=(y-x)=x]]></c>, asymmetric.
	/// </remarks>
	/// <example>
	/// TODO: Add loop logic. Example:
	/// <code><![CDATA[
	/// .9.2.+8.5.7..+4...891+85+9.6....4.59+72...+7.+6+4+3..+8..9+182.7....3..7.163......5.1...4.3.:
	///   417 419 157 883 785 891 893 795
	/// ]]></code>
	/// </example>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		foreach (var supportsGroupedNodes in (false, true))
		{
			if (CollectCore(ref context, supportsGroupedNodes) is { } step)
			{
				return step;
			}
		}
		return null;
	}

	/// <summary>
	/// The internal method to check for M-Wings and grouped M-Wings.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="supportsGroupedNodes">Indicates whether the pattern supports for grouped nodes.</param>
	/// <returns>The found step.</returns>
	private MWingStep? CollectCore(scoped ref AnalysisContext context, bool supportsGroupedNodes)
	{
		// A grid must contain at least one bi-value cell.
		if (!BivalueCells)
		{
			return null;
		}

		// Iterates on two houses, combining to the part 'y=(y-x)=x'.
		scoped ref readonly var grid = ref context.Grid;
		for (var h1 = 0; h1 < 27; h1++)
		{
			var digitsMask1 = grid[HousesMap[h1] & EmptyCells];
			for (var h2 = h1; h2 < 27; h2++)
			{
				// Check for strong links.
				var digitsMask2 = grid[HousesMap[h2] & EmptyCells];
				foreach (var d1 in digitsMask1)
				{
					foreach (var d2 in (Mask)(digitsMask2 & (Mask)~(1 << d1)))
					{
						// Check for validity of (grouped) strong links. The link must be one of the cases in:
						//
						//   1) If non-grouped, two cell maps must contain 2 cells.
						//   2) If grouped, two cell maps must be the case either:
						//     a. The house type is block - the number of spanned rows or columns must be 2.
						//     b. The house type is row or column - the number of spanned blocks must be 2.
						//
						// Otherwise, invalid.
						var (cells1, cells2) = (HousesMap[h1] & CandidatesMap[d1], HousesMap[h2] & CandidatesMap[d2]);
						if (!GroupedNode.IsGroupedStrongLink(in cells1, d1, h1, out var spannedHouses1))
						{
							continue;
						}
						if (!GroupedNode.IsGroupedStrongLink(in cells2, d2, h2, out var spannedHouses2))
						{
							continue;
						}

						// Check for cases, and determine whether 2 (grouped) nodes use 1 cell.
						foreach (var spannedHouse1 in spannedHouses1)
						{
							var p = cells1 & HousesMap[spannedHouse1];
							if (!supportsGroupedNodes && p.Count != 1)
							{
								// Grouped nodes will not be supported in non-grouped searching mode.
								continue;
							}

							if (cells1 - p is not [var theOtherCell1])
							{
								// We cannot make both two nodes grouped.
								continue;
							}

							foreach (var spannedHouse2 in spannedHouses2)
							{
								var q = cells2 & HousesMap[spannedHouse2];
								if (!supportsGroupedNodes && q.Count != 1)
								{
									// Grouped nodes will not be supported in non-grouped searching mode.
									continue;
								}

								if (cells2 - q is not [var theOtherCell2])
								{
									// We cannot make both nodes grouped.
									continue;
								}

								if (supportsGroupedNodes && p.Count * q.Count == 1)
								{
									// In grouped mode, we don't handle for non-grouped steps.
									continue;
								}

								if (theOtherCell1 != theOtherCell2)
								{
									// The XYa cell must be same.
									continue;
								}

								var weakXyCell = theOtherCell1;

								// Find for the real XY cell (strong XY cell) that only contains candidates X and Y.
								var possibleBivalueCells = CandidatesMap[d1] & CandidatesMap[d2] & BivalueCells;
								foreach (var (node, theOtherNode) in ((p, q), (q, p)))
								{
									foreach (var (elimDigit, theOtherDigit) in ((d1, d2), (d2, d1)))
									{
										foreach (var strongXyCellHouse in theOtherNode.CoveredHouses)
										{
											foreach (var strongXyCell in (possibleBivalueCells & HousesMap[strongXyCellHouse]) - node - theOtherNode)
											{
												if (strongXyCell == weakXyCell)
												{
													// Invalid.
													continue;
												}

												if ((HousesMap[h1] & CandidatesMap[elimDigit]) - weakXyCell != node
													|| (HousesMap[h2] & CandidatesMap[theOtherDigit]) - weakXyCell != theOtherNode)
												{
													// Rescue check: We should guarantee the case that both two are real strong links,
													// which means, the target houses for those strong links produced should
													// only contains the cells that hold the nodes and 'weakXyCell'.
													continue;
												}

												var elimMap = (node + strongXyCell).PeerIntersection & CandidatesMap[elimDigit];
												if (!elimMap)
												{
													// No conclusions will be found.
													continue;
												}

												var step = new MWingStep(
													[.. from cell in elimMap select new Conclusion(Elimination, cell, elimDigit)],
													[
														[
															..
															from cell in node
															let cand = cell * 9 + elimDigit
															select new CandidateViewNode(ColorIdentifier.Auxiliary1, cand),
															..
															from cell in theOtherNode
															let cand = cell * 9 + theOtherDigit
															select new CandidateViewNode(ColorIdentifier.Normal, cand),
															new CellViewNode(ColorIdentifier.Normal, strongXyCell),
															new CellViewNode(ColorIdentifier.Auxiliary1, weakXyCell),
															new CandidateViewNode(ColorIdentifier.Auxiliary1, strongXyCell * 9 + elimDigit),
															new CandidateViewNode(ColorIdentifier.Normal, strongXyCell * 9 + theOtherDigit),
															new CandidateViewNode(ColorIdentifier.Normal, weakXyCell * 9 + d1),
															new CandidateViewNode(ColorIdentifier.Normal, weakXyCell * 9 + d2),
															new HouseViewNode(ColorIdentifier.Normal, h1),
															.. h1 == h2 ? [] : (ViewNode[])[new HouseViewNode(ColorIdentifier.Normal, h2)],
														]
													],
													context.PredefinedOptions,
													in node,
													in theOtherNode,
													strongXyCell,
													weakXyCell,
													(Mask)(1 << d1 | 1 << d2)
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
				}
			}
		}

		return null;
	}
}
