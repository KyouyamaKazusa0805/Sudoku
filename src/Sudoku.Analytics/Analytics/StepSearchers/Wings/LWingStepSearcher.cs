namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with an <b>L-Wing (Local Wing)</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>L-Wing</item>
/// <item>Grouped L-Wing</item>
/// </list>
/// </summary>
[StepSearcher(Technique.LWing, Technique.GroupedLWing)]
[StepSearcherRuntimeName("StepSearcherName_LWingStepSearcher")]
public sealed partial class LWingStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates the iterable house pairs.
	/// </summary>
	private static readonly (BlockIndex Left, BlockIndex Right)[] IterableHousePairOffsets =
		[(0, 1), (0, 2), (1, 2), (3, 4), (3, 5), (4, 5), (6, 7), (6, 8), (7, 8)];


	/// <inheritdoc/>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="g/developer-notes"/>
	/// A valid pattern of L-Wing is <c><![CDATA[x=(x-y)=(y-z)=z]]></c>, symmetric.
	/// Please note that the head and tail of the chain is not of a same digit, meaning it will contain eliminations
	/// if those two cells share a same house.
	/// </remarks>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;

		// Collect strong links.
		var strongLinks = new IrregularWingStrongLinkEntry(243);
		for (var house = 0; house < 27; house++)
		{
			for (var digit = 0; digit < 9; digit++)
			{
				if ((HousesMap[house] & CandidatesMap[digit]) is var cells
					&& GroupedNode.IsGroupedStrongLink(in cells, digit, house, out var spannedHouses)
					&& TrailingZeroCount(spannedHouses) is var firstHouse
					&& spannedHouses.GetNextSet(firstHouse) is var secondHouse
					&& (cells & HousesMap[firstHouse], cells & HousesMap[secondHouse]) is var (node1, node2)
					&& new StrongLinkInfo(house, in node1, in node2, spannedHouses) is var link
					&& !strongLinks.TryAdd((house, digit), [link]))
				{
					strongLinks[(house, digit)].Add(link);
				}
			}
		}

		// Iterate on each house and digit.
		foreach (var supportsGroupedNode in (false, true))
		{
			if (CollectCore(ref context, in grid, strongLinks, supportsGroupedNode) is { } step)
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
	/// <param name="strongLinks">Indicates the strong links.</param>
	/// <param name="supportsGroupedNode">Indicates whether the method calculate in grouped nodes.</param>
	/// <returns>The found step.</returns>
	private LWingStep? CollectCore(
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
									if ((elimSide1 | elimSide2).CoveredHouses == 0)
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
									if (midCells.CoveredHouses == 0)
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
									foreach (var coveredHouse in midCells.CoveredHouses)
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
