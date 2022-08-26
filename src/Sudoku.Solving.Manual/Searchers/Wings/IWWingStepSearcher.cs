namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>W-Wing</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>W-Wing (George Woods' Wing)</item>
/// </list>
/// </summary>
public interface IWWingStepSearcher : IIrregularWingStepSearcher
{
}

[StepSearcher]
internal sealed unsafe partial class WWingStepSearcher : IWWingStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		// The grid with possible W-Wing structure should contain
		// at least two empty cells (start and end cell).
		if (BivalueCells.Count < 2)
		{
			return null;
		}

		// Iterate on each cells.
		for (int c1 = 0; c1 < 72; c1++)
		{
			if (!BivalueCells.Contains(c1))
			{
				// The cell isn't a bi-value cell.
				continue;
			}

			// Iterate on each cells which are not peers in 'c1'.
			scoped var digits = grid.GetCandidates(c1).GetAllSets();
			foreach (int c2 in BivalueCells - (PeersMap[c1] + c1))
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
				if ((EmptyCells & intersection) is [])
				{
					// The structure doesn't contain any possible eliminations.
					continue;
				}

				// Iterate on each house.
				for (int house = 0; house < 27; house++)
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
					foreach (int digit in digits)
					{
						var bridge = CandidatesMap[digit] & HousesMap[house];
						bool isPassed = bridge switch
						{
							[var a, var b] => (
								Cells.Empty + c1 + a, Cells.Empty + c2 + b,
								Cells.Empty + c1 + b, Cells.Empty + c2 + a
							) switch
							{
								({ InOneHouse: true }, { InOneHouse: true }, _, _) => true,
								(_, _, { InOneHouse: true }, { InOneHouse: true }) => true,
								_ => false
							},
							{ Count: > 2 and <= 6, BlockMask: var blox } => PopCount((uint)blox) switch
							{
								1 => ((PeersMap[c1] | PeersMap[c2]) & bridge) == bridge,
								2 => TrailingZeroCount(blox) switch
								{
									var block1 => blox.GetNextSet(block1) switch
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
						int anotherDigit = TrailingZeroCount(grid.GetCandidates(c1) & ~(1 << digit));
						var elimMap = CandidatesMap[anotherDigit] & +(Cells.Empty + c1 + c2);
						if (elimMap is [])
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
									from cell in elimMap select new Conclusion(Elimination, cell, anotherDigit),
									ImmutableArray.Create(
										View.Empty
											| new CandidateViewNode[]
											{
												new(DisplayColorKind.Normal, c1 * 9 + anotherDigit),
												new(DisplayColorKind.Normal, c2 * 9 + anotherDigit),
												new(DisplayColorKind.Auxiliary1, c1 * 9 + digit),
												new(DisplayColorKind.Auxiliary1, c2 * 9 + digit),
												new(DisplayColorKind.Auxiliary1, a * 9 + digit),
												new(DisplayColorKind.Auxiliary1, b * 9 + digit)
											}
											| new HouseViewNode(DisplayColorKind.Auxiliary1, house)),
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
									new(DisplayColorKind.Normal, c1 * 9 + anotherDigit),
									new(DisplayColorKind.Normal, c2 * 9 + anotherDigit),
									new(DisplayColorKind.Auxiliary1, c1 * 9 + digit),
									new(DisplayColorKind.Auxiliary1, c2 * 9 + digit)
								};

								foreach (int cell in bridge)
								{
									candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
								}

								step = new GroupedWWingStep(
									from cell in elimMap select new Conclusion(Elimination, cell, anotherDigit),
									ImmutableArray.Create(
										View.Empty
											| candidateOffsets
											| new HouseViewNode(DisplayColorKind.Auxiliary1, house)),
									c1,
									c2,
									bridge
								);

								break;
							}
						}

						if (onlyFindOne)
						{
							return step;
						}

						accumulator.Add(step);
					}
				}
			}
		}

		return null;
	}
}
