namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with an <b>Irregular Wing</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// W-Wing (George Woods' Wing) family:
/// <list type="bullet">
/// <item>W-Wing</item>
/// <item>Grouped W-Wing</item>
/// <item>Multiple Branch W-Wing</item>
/// </list>
/// </item>
/// <item>
/// M-Wing (Medusa Wing) family:
/// <list type="bullet">
/// <item>M-Wing</item>
/// <item>Grouped M-Wing</item>
/// </list>
/// </item>
/// <item>
/// S-Wing (Split Wing) family:
/// <list type="bullet">
/// <item>S-Wing</item>
/// <item>Grouped S-Wing</item>
/// </list>
/// </item>
/// <item>
/// L-Wing (Local Wing) family:
/// <list type="bullet">
/// <item>L-Wing</item>
/// <item>Grouped L-Wing</item>
/// </list>
/// </item>
/// <item>
/// H-Wing (Hybrid Wing) family:
/// <list type="bullet">
/// <item>H-Wing</item>
/// <item>Grouped H-Wing</item>
/// </list>
/// </item>
/// </list>
/// </summary>
[StepSearcher(
	Technique.WWing, Technique.GroupedWWing, Technique.MultiBranchWWing, Technique.MWing, Technique.GroupedMWing,
	Technique.SWing, Technique.GroupedSWing, Technique.LWing, Technique.GroupedLWing, Technique.HWing, Technique.GroupedHWing)]
[StepSearcherRuntimeName("StepSearcherName_IrregularWingStepSearcher")]
public sealed partial class IrregularWingStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates the iterable house pairs.
	/// </summary>
	private static readonly (BlockIndex Left, BlockIndex Right)[] IterableHousePairOffsets =
		[(0, 1), (0, 2), (1, 2), (3, 4), (3, 5), (4, 5), (6, 7), (6, 8), (7, 8)];


	/// <summary>
	/// Indicates whether the step searcher allows searching for W-Wings.
	/// </summary>
	[RuntimeIdentifier(RuntimeIdentifier.AllowWWing)]
	public bool AllowWWing { get; set; }

	/// <summary>
	/// Indicates whether the step searcher allows searching for M-Wings.
	/// </summary>
	[RuntimeIdentifier(RuntimeIdentifier.AllowMWing)]
	public bool AllowMWing { get; set; }

	/// <summary>
	/// Indicates whether the step searcher allows searching for S-Wings.
	/// </summary>
	[RuntimeIdentifier(RuntimeIdentifier.AllowSWing)]
	public bool AllowSWing { get; set; }

	/// <summary>
	/// Indicates whether the step searcher allows searching for L-Wings.
	/// </summary>
	[RuntimeIdentifier(RuntimeIdentifier.AllowLWing)]
	public bool AllowLWing { get; set; }

	/// <summary>
	/// Indicates whether the step searcher allows searching for H-Wings.
	/// </summary>
	[RuntimeIdentifier(RuntimeIdentifier.AllowHWing)]
	public bool AllowHWing { get; set; }


	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		if (AllowWWing && Collect_WWing(ref context) is { } w)
		{
			return w;
		}
		if (AllowWWing && Collect_MultiBranchWWing(ref context) is { } w2)
		{
			return w2;
		}
		if (AllowMWing && Collect_MWing(ref context) is { } m)
		{
			return m;
		}
		if (AllowSWing && Collect_SWing(ref context) is { } s)
		{
			return s;
		}
		if (AllowLWing && Collect_LWing(ref context) is { } l)
		{
			return l;
		}
		if (AllowHWing && Collect_HWing(ref context) is { } h)
		{
			return h;
		}

		return null;
	}

	/// <inheritdoc cref="Collect(ref AnalysisContext)"/>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="g/developer-notes"/>
	/// A valid pattern of W-Wing is <c><![CDATA[(x=y)-y=y-(y=x)]]></c>, symmetric.
	/// </remarks>
	/// <example>
	/// TODO: Add loop logic. Example:
	/// <code><![CDATA[
	/// +39+4+7.6...+2+851..3+7+616+7+3.2...8+7.+6.34..6+4..+7+5+8+31..34.+8.+67...8...23..8..76.....5...8.:
	///   435 935 567 175 185 192 193 993 195 495 995
	/// ]]></code>
	/// </example>
	private WWingStep? Collect_WWing(scoped ref AnalysisContext context)
	{
		// The grid with possible W-Wing pattern should contain at least two empty cells (start and end cell).
		if (BivalueCells.Count < 2)
		{
			return null;
		}

		// Iterate on each cells.
		scoped ref readonly var grid = ref context.Grid;
		for (var c1 = 0; c1 < 72; c1++)
		{
			if (!BivalueCells.Contains(c1))
			{
				// The cell isn't a bi-value cell.
				continue;
			}

			// Iterate on each cells which are not peers in 'c1'.
			scoped var digits = grid.GetCandidates(c1).GetAllSets();
			foreach (var c2 in BivalueCells - (PeersMap[c1] + c1))
			{
				if (c2 < c1)
				{
					// To avoid duplicate structures found.
					continue;
				}

				if (grid.GetCandidates(c1) != grid.GetCandidates(c2))
				{
					// Two cells may contain different kinds of digits.
					continue;
				}

				var intersection = PeersMap[c1] & PeersMap[c2];
				if (!(EmptyCells & intersection))
				{
					// The pattern doesn't contain any possible eliminations.
					continue;
				}

				// Iterate on each house.
				for (var house = 0; house < 27; house++)
				{
					if (house == c1.ToHouseIndex(HouseType.Block)
						|| house == c1.ToHouseIndex(HouseType.Row)
						|| house == c1.ToHouseIndex(HouseType.Column)
						|| house == c2.ToHouseIndex(HouseType.Block)
						|| house == c2.ToHouseIndex(HouseType.Row)
						|| house == c2.ToHouseIndex(HouseType.Column))
					{
						// The house to search for conjugate pairs shouldn't be the same as those two cells' houses.
						continue;
					}

					// Iterate on each digit to search for the conjugate pair.
					foreach (var digit in digits)
					{
						var bridge = CandidatesMap[digit] & HousesMap[house];
						var isPassed = bridge switch
						{
#pragma warning disable format
							[var a, var b]
								when ((CellMap)c1 + a).InOneHouse(out _) && ((CellMap)c2 + b).InOneHouse(out _)
								|| ((CellMap)c1 + b).InOneHouse(out _) && ((CellMap)c2 + a).InOneHouse(out _)
								=> true,
#pragma warning restore format
							{ Count: > 2 and <= 6, BlockMask: var blocks } => PopCount((uint)blocks) switch
							{
								1 => ((PeersMap[c1] | PeersMap[c2]) & bridge) == bridge,
								2 when TrailingZeroCount(blocks) is var block1 && blocks.GetNextSet(block1) is var block2
									=> (HousesMap[block1] & bridge, HousesMap[block2] & bridge) switch
									{
										var (bridgeInBlock1, bridgeInBlock2) => (
											(PeersMap[c1] & bridgeInBlock1) == bridgeInBlock1,
											(PeersMap[c2] & bridgeInBlock2) == bridgeInBlock2,
											(PeersMap[c1] & bridgeInBlock2) == bridgeInBlock2,
											(PeersMap[c2] & bridgeInBlock1) == bridgeInBlock1
										) switch
										{
											(true, true, _, _) => true,
											(_, _, true, true) => true,
											_ => false
										}
									},
								_ => false
							},
							_ => false
						};
						if (!isPassed)
						{
							continue;
						}

						// Check for eliminations.
						var anotherDigit = TrailingZeroCount(grid.GetCandidates(c1) & ~(1 << digit));
						var elimMap = CandidatesMap[anotherDigit] & ((CellMap)c1 + c2).PeerIntersection;
						if (!elimMap)
						{
							// No possible eliminations found.
							continue;
						}

						// Now W-Wing found. Store it into the accumulator.
						var step = new WWingStep(
							[.. from cell in elimMap select new Conclusion(Elimination, cell, anotherDigit)],
							[
								[
									new CandidateViewNode(ColorIdentifier.Auxiliary1, c1 * 9 + anotherDigit),
									new CandidateViewNode(ColorIdentifier.Auxiliary1, c2 * 9 + anotherDigit),
									new CandidateViewNode(ColorIdentifier.Normal, c1 * 9 + digit),
									new CandidateViewNode(ColorIdentifier.Normal, c2 * 9 + digit),
									.. from cell in bridge select new CandidateViewNode(ColorIdentifier.Normal, cell * 9 + digit),
									new HouseViewNode(ColorIdentifier.Normal, house)
								]
							],
							context.PredefinedOptions,
							c1,
							c2,
							in bridge
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

		return null;
	}

	/// <inheritdoc cref="Collect(ref AnalysisContext)"/>
	private MultiBranchWWingStep? Collect_MultiBranchWWing(scoped ref AnalysisContext context)
	{
		// Iterates on each digit.
		scoped ref readonly var grid = ref context.Grid;
		for (var digit1 = 0; digit1 < 8; digit1++)
		{
			for (var digit2 = digit1 + 1; digit2 < 9; digit2++)
			{
				// Checks for bi-value cells that only holds two different digits.
				var possibleCells = BivalueCells & CandidatesMap[digit1] & CandidatesMap[digit2];
				if (possibleCells.Count < 3)
				{
					continue;
				}

				// Iterates on sizes.
				// Please note that cases whose cardinality (i.e. size of the subset) is greater than 5
				// is extremely rare to appear. Therefore we can just skip on such cases,
				// although the limit will miss some rare cases.
				for (var size = 3; size <= Math.Min(possibleCells.Count, 5); size++)
				{
					// Iterates on each combination.
					foreach (ref readonly var cells in possibleCells.GetSubsets(size))
					{
						// Checks whether they can intersect to at least one cell, as the elimination cell.
						if (cells.PeerIntersection is not (var elimMap and not []))
						{
							continue;
						}

						// Iterates on each digit, to fix the digit as the X digit.
						// W-Wing pattern requires a pair of W digits as the first and the last place,
						// and requires the X digits are in the body of the pattern (i.e. non-terminal nodes).
						foreach (var xDigit in (digit1, digit2))
						{
							// Gets the target house that can place root cells.
							foreach (var house in (HouseMaskOperations.AllRowsMask | HouseMaskOperations.AllColumnsMask) & ~cells.Houses)
							{
								var crosshatchingHouseType = house >= 18 ? HouseType.Row : HouseType.Column;
								var emptyCellsInThisHouse = HousesMap[house] & CandidatesMap[xDigit];
								if (emptyCellsInThisHouse.Count < size)
								{
									continue;
								}

								var tempCrosshatchingHouses = (CellMap)[];
								foreach (var cell in cells)
								{
									tempCrosshatchingHouses |= HousesMap[cell.ToHouseIndex(crosshatchingHouseType)];
								}
								emptyCellsInThisHouse &= tempCrosshatchingHouses;
								if (emptyCellsInThisHouse.Count != size)
								{
									continue;
								}

								if ((HousesMap[house] & CandidatesMap[xDigit]) != emptyCellsInThisHouse)
								{
									// This house contains other unused empty cells
									// that can also be filled with digit X.
									continue;
								}

								var wDigit = xDigit == digit1 ? digit2 : digit1;
								var conclusions = new List<Conclusion>(elimMap.Count);
								foreach (var cell in elimMap)
								{
									if (CandidatesMap[wDigit].Contains(cell))
									{
										conclusions.Add(new(Elimination, cell, wDigit));
									}
								}
								if (conclusions.Count == 0)
								{
									continue;
								}

								var candidateOffsets = new List<CandidateViewNode>();
								foreach (var cell in cells)
								{
									foreach (var digit in grid.GetCandidates(cell))
									{
										candidateOffsets.Add(new(ColorIdentifier.Normal, cell * 9 + digit));
									}
								}
								foreach (var cell in emptyCellsInThisHouse)
								{
									candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, cell * 9 + xDigit));
								}

								var step = new MultiBranchWWingStep(
									[.. conclusions],
									[[.. candidateOffsets, new HouseViewNode(ColorIdentifier.Auxiliary1, house)]],
									context.PredefinedOptions,
									in cells,
									in emptyCellsInThisHouse,
									emptyCellsInThisHouse.SharedLine
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

	/// <inheritdoc cref="Collect(ref AnalysisContext)"/>
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
	private MWingStep? Collect_MWing(scoped ref AnalysisContext context)
	{
		foreach (var supportsGroupedNodes in (false, true))
		{
			if (collectCore(ref context, supportsGroupedNodes) is { } step)
			{
				return step;
			}
		}
		return null;


		static MWingStep? collectCore(scoped ref AnalysisContext context, bool supportsGroupedNodes)
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
							// Check for validity of (grouped) strong links.
							var (cells1, cells2) = (HousesMap[h1] & CandidatesMap[d1], HousesMap[h2] & CandidatesMap[d2]);
							if (!Grouped.IsGroupedStrongLink(in cells1, d1, h1, out var spannedHousesList1))
							{
								continue;
							}
							if (!Grouped.IsGroupedStrongLink(in cells2, d2, h2, out var spannedHousesList2))
							{
								continue;
							}

							foreach (var spannedHouses1 in spannedHousesList1)
							{
								foreach (var spannedHouses2 in spannedHousesList2)
								{
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
													foreach (var strongXyCellHouse in theOtherNode.SharedHouses)
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
				}
			}

			return null;
		}
	}

	/// <inheritdoc cref="Collect(ref AnalysisContext)"/>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="g/developer-notes"/>
	/// A valid pattern of S-Wing is <c><![CDATA[x=x-(x=y)-y=y]]></c>, symmetric.
	/// Please note that the head and tail of the chain is not of a same digit, meaning it will contain eliminations
	/// if those two cells share a same house.
	/// </remarks>
	private SWingStep? Collect_SWing(scoped ref AnalysisContext context)
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
			if (collectCore(ref context, in grid, supportsGroupedNode, iterableDigitsMask, strongLinks) is { } step)
			{
				return step;
			}
		}

		return null;


		static SWingStep? collectCore(
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

	/// <inheritdoc cref="Collect(ref AnalysisContext)"/>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="g/developer-notes"/>
	/// A valid pattern of L-Wing is <c><![CDATA[x=(x-y)=(y-z)=z]]></c>, asymmetric.
	/// Please note that the head and tail of the chain is not of a same digit, meaning it will contain eliminations
	/// if those two cells share a same house.
	/// </remarks>
	private LWingStep? Collect_LWing(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;

		// Collect strong links.
		var strongLinks = new IrregularWingStrongLinkEntry(243);
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
							&& (cells & HousesMap[firstHouse], cells & HousesMap[secondHouse]) is var (node1, node2)
							&& new StrongLinkInfo(house, in node1, in node2, spannedHouses) is var link
							&& !strongLinks.TryAdd((house, digit), [link]))
						{
							strongLinks[(house, digit)].Add(link);
						}
					}
				}
			}
		}

		// Iterate on each house and digit.
		foreach (var supportsGroupedNode in (false, true))
		{
			if (collectCore(ref context, in grid, strongLinks, supportsGroupedNode) is { } step)
			{
				return step;
			}
		}

		return null;


		static LWingStep? collectCore(
			scoped ref AnalysisContext context,
			scoped ref readonly Grid grid,
			IrregularWingStrongLinkEntry strongLinks,
			bool supportsGroupedNode
		)
		{
			foreach (var houseType in HouseTypes)
			{
				var offset = (byte)houseType * 9;
				foreach (var (o1, o2) in IterableHousePairOffsets)
				{
					var (house1, house2) = (offset + o1, offset + o2);
					for (var x = 0; x < 8; x++)
					{
						for (var z = x + 1; z < 9; z++)
						{
							foreach (var (digitX, digitZ) in ((x, z), (z, x)))
							{
								if (!strongLinks.TryGetValue((house1, digitX), out var links1)
									|| !strongLinks.TryGetValue((house2, digitZ), out var links2)
									|| (links1, links2) is not ([var (_, p1, q1, _)], [var (_, p2, q2, _)]))
								{
									continue;
								}

								foreach (var (elimSide1, midCellSide1) in ((p1, q1), (q1, p1)))
								{
									foreach (var (elimSide2, midCellSide2) in ((p2, q2), (q2, p2)))
									{
										if (elimSide1.Count != 1 && elimSide2.Count != 1)
										{
											// No eliminations.
											continue;
										}

										if (supportsGroupedNode ^ elimSide1.Count * elimSide2.Count != 1)
										{
											continue;
										}

										// Check whether the eliminated side is in a same house.
										if ((elimSide1 | elimSide2).SharedHouses == 0)
										{
											continue;
										}

										if (midCellSide1.Count != 1 || midCellSide2.Count != 1)
										{
											// Cannot form a valid L-Wing.
											continue;
										}

										// Check whether the mid cells side is in a same house.
										var midCells = midCellSide1 | midCellSide2;
										if (midCells.SharedHouses == 0)
										{
											continue;
										}

										if (grid[in midCells, false, GridMaskMergingMethod.And] is var mask && (mask == 0 || !IsPow2(mask)))
										{
											// No digit Y can be found.
											continue;
										}

										var digitY = Log2((uint)mask);
										if (digitY == digitX || digitY == digitZ)
										{
											// Digit Y cannot be same as any one of X and Z.
											continue;
										}

										// Check whether the digit Y forms a strong link with two digits.
										foreach (var coveredHouse in midCells.SharedHouses)
										{
											if ((HousesMap[coveredHouse] & CandidatesMap[digitY]) != midCells)
											{
												// The strong link may be formed.
												continue;
											}

											// A valid L-Wing is formed. Now check for eliminations.
											var conclusions = new List<Conclusion>(2);
											switch (elimSide1.Count, elimSide2.Count)
											{
												case (1, 1):
												{
													var digitXCell = elimSide1[0];
													var digitZCell = elimSide2[0];
													if ((grid.GetCandidates(digitXCell) >> digitZ & 1) != 0)
													{
														conclusions.Add(new(Elimination, digitXCell, digitZ));
													}
													if ((grid.GetCandidates(digitZCell) >> digitX & 1) != 0)
													{
														conclusions.Add(new(Elimination, digitZCell, digitX));
													}
													break;
												}
												case (1, not 1):
												{
													var digitXCell = elimSide1[0];
													if ((grid.GetCandidates(digitXCell) >> digitZ & 1) != 0)
													{
														conclusions.Add(new(Elimination, digitXCell, digitZ));
													}
													break;
												}
												case (not 1, 1):
												{
													var digitZCell = elimSide2[0];
													if ((grid.GetCandidates(digitZCell) >> digitX & 1) != 0)
													{
														conclusions.Add(new(Elimination, digitZCell, digitX));
													}
													break;
												}
											}
											if (conclusions.Count == 0)
											{
												// No conclusions found.
												continue;
											}

											var comparer = (Mask)((Mask)((Mask)(1 << digitX) | (Mask)(1 << digitY)) | (Mask)(1 << digitZ));
											var step = new LWingStep(
												[.. conclusions],
												[
													[
														..
														from cell in elimSide1
														select new CandidateViewNode(ColorIdentifier.Auxiliary1, cell * 9 + digitX),
														..
														from cell in elimSide2
														select new CandidateViewNode(ColorIdentifier.Auxiliary1, cell * 9 + digitZ),
														..
														from digit in (Mask)(grid.GetCandidates(midCellSide1[0]) & comparer)
														select new CandidateViewNode(ColorIdentifier.Normal, midCellSide1[0] * 9 + digit),
														..
														from digit in (Mask)(grid.GetCandidates(midCellSide2[0]) & comparer)
														select new CandidateViewNode(ColorIdentifier.Normal, midCellSide2[0] * 9 + digit),
														.. from cell in midCells select new CellViewNode(ColorIdentifier.Auxiliary1, cell),
														new HouseViewNode(ColorIdentifier.Normal, house1),
														new HouseViewNode(ColorIdentifier.Normal, house2),
														new HouseViewNode(ColorIdentifier.Normal, coveredHouse)
													]
												],
												context.PredefinedOptions,
												in elimSide1,
												in elimSide2,
												digitX,
												digitY,
												digitZ,
												midCells[0],
												midCells[1]
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

			return null;
		}
	}

	/// <inheritdoc cref="Collect(ref AnalysisContext)"/>
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
	private HWingStep? Collect_HWing(scoped ref AnalysisContext context)
	{
		if (BivalueCells.Count < 2)
		{
			// There is no valid H-Wings because a valid H-Wing will use 2 bi-value cells.
			return null;
		}

		scoped ref readonly var grid = ref context.Grid;

		// Search for all possible ALSes appeared in the grid, and arrange them by grouping them by houses.
		var alsLinks = new Dictionary<House, List<AlmostLockedSetLinkInfo>>(27);
		var strongLinks = new IrregularWingStrongLinkEntry(243);
		for (var house = 0; house < 27; house++)
		{
			// Check for strong links first.
			for (var digit = 0; digit < 9; digit++)
			{
				if ((HousesMap[house] & CandidatesMap[digit]) is var possibleCells
					&& Grouped.IsGroupedStrongLink(in possibleCells, digit, house, out var spannedHousesList))
				{
					foreach (var spannedHouses in spannedHousesList)
					{
						if (TrailingZeroCount(spannedHouses) is var firstHouse
							&& spannedHouses.GetNextSet(firstHouse) is var secondHouse
							&& (possibleCells & HousesMap[firstHouse], possibleCells & HousesMap[secondHouse]) is var (p, q)
							&& new StrongLinkInfo(house, in p, in q, spannedHouses) is var info
							&& !strongLinks.TryAdd((house, digit), [info]))
						{
							strongLinks[(house, digit)].Add(info);
						}
					}
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
				var strongLink = new AlmostLockedSetLinkInfo(
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
			if (collectCore(ref context, in grid, alsLinks, strongLinks, supportGroupedNode) is { } step)
			{
				return step;
			}
		}

		return null;


		static HWingStep? collectCore(
			scoped ref AnalysisContext context,
			scoped ref readonly Grid grid,
			Dictionary<House, List<AlmostLockedSetLinkInfo>> alsLinks,
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

									foreach (var startNodeCoveredHouse in startNode.SharedHouses)
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


	/// <summary>
	/// Represents data describing for an H-Wing pattern.
	/// </summary>
	/// <param name="CommonDigit">The common digit for those two digits.</param>
	/// <param name="OtherDigitsMask">Indicates the other digits used.</param>
	/// <param name="Cells">Indicates the two cells.</param>
	private sealed record AlmostLockedSetLinkInfo(Digit CommonDigit, Mask OtherDigitsMask, scoped ref readonly CellMap Cells);
}
