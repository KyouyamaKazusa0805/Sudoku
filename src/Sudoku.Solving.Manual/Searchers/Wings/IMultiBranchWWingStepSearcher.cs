namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Multiple-Branch W-Wing</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Multiple-Branch W-Wing (Multiple-Branch George Woods' Wing)</item>
/// </list>
/// </summary>
public interface IMultiBranchWWingStepSearcher : IIrregularWingStepSearcher
{
}

[StepSearcher]
internal sealed partial class MultiBranchWWingStepSearcher : IMultiBranchWWingStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		// Iterates on each digit.
		for (int digit1 = 0; digit1 < 8; digit1++)
		{
			for (int digit2 = digit1 + 1; digit2 < 9; digit2++)
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
				for (int size = 3, count = Min(possibleCells.Count, 5); size <= count; size++)
				{
					// Iterates on each combination.
					foreach (var cells in possibleCells & size)
					{
						// Checks whether they can intersect to at least one cell, as the elimination cell.
						if (!cells is not (var elimMap and not []))
						{
							continue;
						}

#if false
						// Checks whether every pair of cells in the cell combination is in a same region.
						bool anyTwoCellsInSameHouse = false;
						foreach (var e in cells & 2)
						{
							if (e.CoveredHouses != 0)
							{
								anyTwoCellsInSameHouse = true;
								break;
							}
						}
						if (anyTwoCellsInSameHouse)
						{
							continue;
						}
#endif

						// Iterates on each digit, to fix the digit as the X digit.
						// W-Wing structure requires a pair of W digits as the first and the last place,
						// and requires the X digits are in the body of the structure (i.e. non-terminal nodes).
						foreach (int xDigit in stackalloc[] { digit1, digit2 })
						{
							// Gets the target house that can place root cells.
							foreach (int house in (AllRowsMask | AllColumnsMask) & ~cells.Houses)
							{
								var crosshatchingHouseType = house >= 18 ? HouseType.Row : HouseType.Column;
								var emptyCellsInThisHouse = HouseMaps[house] & CandidatesMap[xDigit];
								if (emptyCellsInThisHouse.Count < size)
								{
									continue;
								}

								var tempCrosshatchingHouses = Cells.Empty;
								foreach (int cell in cells)
								{
									tempCrosshatchingHouses |= HouseMaps[cell.ToHouseIndex(crosshatchingHouseType)];
								}
								emptyCellsInThisHouse &= tempCrosshatchingHouses;
								if (emptyCellsInThisHouse.Count != size)
								{
									continue;
								}

								int wDigit = xDigit == digit1 ? digit2 : digit1;
								var conclusions = new List<Conclusion>(elimMap.Count);
								foreach (int cell in elimMap)
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
								foreach (int cell in cells)
								{
									foreach (int digit in grid.GetCandidates(cell))
									{
										candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
									}
								}
								foreach (int cell in emptyCellsInThisHouse)
								{
									candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + xDigit));
								}

								var step = new MultiBranchWWingStep(
									conclusions.ToImmutableArray(),
									ImmutableArray.Create(
										View.Empty
											| candidateOffsets
											| new HouseViewNode(DisplayColorKind.Auxiliary1, house)
									),
									cells,
									emptyCellsInThisHouse,
									emptyCellsInThisHouse.CoveredLine
								);
								if (onlyFindOne)
								{
									return step;
								}

								accumulator.Add(step);
							}
						}
					}
				}
			}
		}

		return null;
	}
}
