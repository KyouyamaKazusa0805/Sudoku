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
				for (int size = 3, count = possibleCells.Count; size <= Min(count, 5); size++)
				{
					// Iterates on each combination.
					foreach (var cells in possibleCells & size)
					{
						// Checks whether they can intersect to at least one cell, as the elimination cell.
						if (!cells is not (var elimMap and not []))
						{
							continue;
						}

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

						foreach (int fixedDigit in stackalloc[] { digit1, digit2 })
						{
							foreach (int house in (AllRowsMask | AllColumnsMask) & ~cells.Houses)
							{
								var crosshatchingHouseType = house >= 18 ? HouseType.Row : HouseType.Column;
								var emptyCellsInThisHouse = HouseMaps[house] & CandidatesMap[fixedDigit];
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

								int elimDigit = fixedDigit == digit1 ? digit2 : digit1;
								var conclusions = new List<Conclusion>(elimMap.Count);
								foreach (int cell in elimMap)
								{
									if (CandidatesMap[elimDigit].Contains(cell))
									{
										conclusions.Add(new(Elimination, cell, elimDigit));
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
									candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + fixedDigit));
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
