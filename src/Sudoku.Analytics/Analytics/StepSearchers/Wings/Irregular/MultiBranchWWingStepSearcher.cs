namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Multiple-Branch W-Wing</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Multiple-Branch W-Wing (Multiple-Branch George Woods' Wing)</item>
/// </list>
/// </summary>
[StepSearcher(Technique.MultiBranchWWing)]
[StepSearcherRuntimeName("StepSearcherName_MultiBranchWWingStepSearcher")]
public sealed partial class MultiBranchWWingStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
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
										candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
									}
								}
								foreach (var cell in emptyCellsInThisHouse)
								{
									candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, cell * 9 + xDigit));
								}

								var step = new MultiBranchWWingStep(
									[.. conclusions],
									[[.. candidateOffsets, new HouseViewNode(WellKnownColorIdentifier.Auxiliary1, house)]],
									context.PredefinedOptions,
									in cells,
									in emptyCellsInThisHouse,
									emptyCellsInThisHouse.CoveredLine
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
