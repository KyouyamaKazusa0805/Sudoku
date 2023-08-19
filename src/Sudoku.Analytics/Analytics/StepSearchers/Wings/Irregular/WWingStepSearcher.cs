namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>W-Wing</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>W-Wing (George Woods' Wing)</item>
/// </list>
/// </summary>
[StepSearcher(Technique.WWing, Technique.GroupedWWing)]
public sealed partial class WWingStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
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
						// The house to search for conjugate pairs shouldn't
						// be the same as those two cells' houses.
						continue;
					}

					// Iterate on each digit to search for the conjugate pair.
					foreach (var digit in digits)
					{
						var bridge = CandidatesMap[digit] & HousesMap[house];
						var isPassed = bridge switch
						{
							[var a, var b] => (CellsMap[c1] + a, CellsMap[c2] + b, CellsMap[c1] + b, CellsMap[c2] + a) switch
							{
								({ InOneHouse: true }, { InOneHouse: true }, _, _) => true,
								(_, _, { InOneHouse: true }, { InOneHouse: true }) => true,
								_ => false
							},
							{ Count: > 2 and <= 6, BlockMask: var blocks } => PopCount((uint)blocks) switch
							{
								1 => ((PeersMap[c1] | PeersMap[c2]) & bridge) == bridge,
								2 => TrailingZeroCount(blocks) switch
								{
									var block1 => blocks.GetNextSet(block1) switch
									{
										var block2 => (HousesMap[block1] & bridge, HousesMap[block2] & bridge) switch
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
										}
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
						var elimMap = CandidatesMap[anotherDigit] & (CellsMap[c1] + c2).PeerIntersection;
						if (!elimMap)
						{
							// No possible eliminations found.
							continue;
						}

						// Now W-Wing found. Store it into the accumulator.
						Step step;
						switch (bridge)
						{
							case [var a, var b]:
							{
								step = new WWingStep(
									[.. from cell in elimMap select new Conclusion(Elimination, cell, anotherDigit)],
									[
										[
											new CandidateViewNode(WellKnownColorIdentifier.Normal, c1 * 9 + anotherDigit),
											new CandidateViewNode(WellKnownColorIdentifier.Normal, c2 * 9 + anotherDigit),
											new CandidateViewNode(WellKnownColorIdentifier.Auxiliary1, c1 * 9 + digit),
											new CandidateViewNode(WellKnownColorIdentifier.Auxiliary1, c2 * 9 + digit),
											new CandidateViewNode(WellKnownColorIdentifier.Auxiliary1, a * 9 + digit),
											new CandidateViewNode(WellKnownColorIdentifier.Auxiliary1, b * 9 + digit),
											new HouseViewNode(WellKnownColorIdentifier.Auxiliary1, house)
										]
									],
									c1,
									c2,
									new(a, b, digit)
								);

								break;
							}
							default:
							{
								var candidateOffsets = new List<CandidateViewNode>(8)
								{
									new(WellKnownColorIdentifier.Normal, c1 * 9 + anotherDigit),
									new(WellKnownColorIdentifier.Normal, c2 * 9 + anotherDigit),
									new(WellKnownColorIdentifier.Auxiliary1, c1 * 9 + digit),
									new(WellKnownColorIdentifier.Auxiliary1, c2 * 9 + digit)
								};

								foreach (var cell in bridge)
								{
									candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, cell * 9 + digit));
								}

								step = new GroupedWWingStep(
									[.. from cell in elimMap select new Conclusion(Elimination, cell, anotherDigit)],
									[[.. candidateOffsets, new HouseViewNode(WellKnownColorIdentifier.Auxiliary1, house)]],
									c1,
									c2,
									bridge
								);

								break;
							}
						}

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
}
