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
/// <!--
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
/// -->
/// </list>
/// </summary>
[StepSearcher(
	"StepSearcherName_IrregularWingStepSearcher",
	Technique.WWing, Technique.GroupedWWing, Technique.MultiBranchWWing, Technique.MWing, Technique.GroupedMWing
#if false
	,
	Technique.SWing, Technique.GroupedSWing, Technique.LWing, Technique.GroupedLWing, Technique.HWing, Technique.GroupedHWing
#endif
	)]
public sealed partial class IrregularWingStepSearcher : StepSearcher
{
#if false
	/// <summary>
	/// Indicates the iterable house pairs.
	/// </summary>
	private static readonly (BlockIndex Left, BlockIndex Right)[] IterableHousePairOffsets =
		[(0, 1), (0, 2), (1, 2), (3, 4), (3, 5), (4, 5), (6, 7), (6, 8), (7, 8)];


	/// <summary>
	/// Indicates whether the step searcher allows searching for W-Wings.
	/// </summary>
	[SettingItemName(SettingItemNames.AllowWWing)]
	public bool AllowWWing { get; set; }

	/// <summary>
	/// Indicates whether the step searcher allows searching for M-Wings.
	/// </summary>
	[SettingItemName(SettingItemNames.AllowMWing)]
	public bool AllowMWing { get; set; }

	/// <summary>
	/// Indicates whether the step searcher allows searching for S-Wings.
	/// </summary>
	[SettingItemName(SettingItemNames.AllowSWing)]
	public bool AllowSWing { get; set; }

	/// <summary>
	/// Indicates whether the step searcher allows searching for L-Wings.
	/// </summary>
	[SettingItemName(SettingItemNames.AllowLWing)]
	public bool AllowLWing { get; set; }

	/// <summary>
	/// Indicates whether the step searcher allows searching for H-Wings.
	/// </summary>
	[SettingItemName(SettingItemNames.AllowHWing)]
	public bool AllowHWing { get; set; }
#endif


	/// <inheritdoc/>
	protected internal override Step? Collect(ref AnalysisContext context)
	{
		if (/*AllowWWing && */Collect_WWing(ref context) is { } w)
		{
			return w;
		}
		if (/*AllowWWing && */Collect_MultiBranchWWing(ref context) is { } w2)
		{
			return w2;
		}
		if (/*AllowMWing && */Collect_MWing(ref context) is { } m)
		{
			return m;
		}
#if false
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
#endif

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
	/// +39+4+7.6...+2+851..3+7+616+7+3.2...8+7.+6.34..6+4..+7+5+8+31..34.+8.+67...8...23..8..76.....5...8.:435 935 567 175 185 192 193 993 195 495 995
	/// ]]></code>
	/// </example>
	private WWingStep? Collect_WWing(ref AnalysisContext context)
	{
		// The grid with possible W-Wing pattern should contain at least two empty cells (start and end cell).
		if (BivalueCells.Count < 2)
		{
			return null;
		}

		// Iterate on each cells.
		ref readonly var grid = ref context.Grid;
		for (var c1 = 0; c1 < 72; c1++)
		{
			if (!BivalueCells.Contains(c1))
			{
				// The cell isn't a bi-value cell.
				continue;
			}

			// Iterate on each cells which are not peers in 'c1'.
			var digits = grid.GetCandidates(c1).GetAllSets();
			foreach (var c2 in BivalueCells & ~(PeersMap[c1] + c1))
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
								when (c1.AsCellMap() + a).InOneHouse(out _) && (c2.AsCellMap() + b).InOneHouse(out _)
								|| (c1.AsCellMap() + b).InOneHouse(out _) && (c2.AsCellMap() + a).InOneHouse(out _)
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
						var elimMap = CandidatesMap[anotherDigit] & (c1.AsCellMap() + c2).PeerIntersection;
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
							context.Options,
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
	private MultiBranchWWingStep? Collect_MultiBranchWWing(ref AnalysisContext context)
	{
		// Iterates on each digit.
		ref readonly var grid = ref context.Grid;
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
					foreach (ref readonly var cells in possibleCells & size)
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

								var tempCrosshatchingHouses = CellMap.Empty;
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
									context.Options,
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
	/// .9.2.+8.5.7..+4...891+85+9.6....4.59+72...+7.+6+4+3..+8..9+182.7....3..7.163......5.1...4.3.:417 419 157 883 785 891 893 795
	/// ]]></code>
	/// </example>
	private MWingStep? Collect_MWing(ref AnalysisContext context)
	{
		foreach (var supportsGroupedNodes in (false, true))
		{
			if (collectCore(ref context, supportsGroupedNodes) is { } step)
			{
				return step;
			}
		}
		return null;


		static MWingStep? collectCore(ref AnalysisContext context, bool supportsGroupedNodes)
		{
			// A grid must contain at least one bi-value cell.
			if (!BivalueCells)
			{
				return null;
			}

			// Iterates on two houses, combining to the part 'y=(y-x)=x'.
			ref readonly var grid = ref context.Grid;
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
							if (!Grouped.IsGroupedStrongLink(in cells1, h1, out var spannedHousesList1))
							{
								continue;
							}
							if (!Grouped.IsGroupedStrongLink(in cells2, h2, out var spannedHousesList2))
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

										if ((cells1 & ~p) is not [var theOtherCell1])
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

											if ((cells2 & ~q) is not [var theOtherCell2])
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
														foreach (var strongXyCell in
															possibleBivalueCells & HousesMap[strongXyCellHouse] & ~node & ~theOtherNode)
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
																context.Options,
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
}
