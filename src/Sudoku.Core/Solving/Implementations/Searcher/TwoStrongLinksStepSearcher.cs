namespace Sudoku.Solving.Implementations.Searcher;

[StepSearcher]
[StepSearcherOptions(PuzzleNotRelying = true)]
internal sealed unsafe partial class TwoStrongLinksStepSearcher : ITwoStrongLinksStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		for (int digit = 0; digit < 9; digit++)
		{
			for (int h1 = 0; h1 < 26; h1++)
			{
				for (int h2 = h1 + 1; h2 < 27; h2++)
				{
					// Get masks.
					short mask1 = (HousesMap[h1] & CandidatesMap[digit]) / h1;
					short mask2 = (HousesMap[h2] & CandidatesMap[digit]) / h2;
					if (PopCount((uint)mask1) != 2 || PopCount((uint)mask2) != 2)
					{
						continue;
					}

					// Get all cells.
					var cells1 = CellMap.Empty;
					var cells2 = CellMap.Empty;
					var cellsList1 = new List<int>(PopCount((uint)mask1));
					var cellsList2 = new List<int>(PopCount((uint)mask2));
					foreach (int pos1 in mask1)
					{
						int cell1 = HouseCells[h1][pos1];
						cellsList1.Add(cell1);
						cells1.Add(cell1);
					}
					foreach (int pos2 in mask2)
					{
						int cell2 = HouseCells[h2][pos2];
						cellsList2.Add(cell2);
						cells2.Add(cell2);
					}

					if (cells1 & cells2)
					{
						continue;
					}

					// Check two cells share a same house.
					int sameHouse, headIndex, tailIndex, c1Index, c2Index;
					for (int i = 0; i < 2; i++)
					{
						int cell1 = cellsList1[i];
						for (int j = 0; j < 2; j++)
						{
							int cell2 = cellsList2[j];
							if ((CellMap.Empty + cell1 + cell2).AllSetsAreInOneHouse(out sameHouse))
							{
								(c1Index, c2Index) = (i, j);
								(headIndex, tailIndex) = (i == 0 ? 1 : 0, j == 0 ? 1 : 0);
								goto Checking;
							}
						}
					}

					// Not same house.
					continue;

				Checking:
					// Two strong link found.
					// Record all eliminations.
					int head = cellsList1[headIndex], tail = cellsList2[tailIndex];
					if ((PeersMap[head] & PeersMap[tail] & CandidatesMap[digit]) is not (var elimMap and not []))
					{
						continue;
					}

					var step = new TwoStrongLinksStep(
						from cell in elimMap select new Conclusion(Elimination, cell, digit),
						ImmutableArray.Create(
							View.Empty
								| new CandidateViewNode[]
								{
									new(DisplayColorKind.Normal, cellsList1[c1Index] * 9 + digit),
									new(DisplayColorKind.Normal, cellsList2[c2Index] * 9 + digit),
									new(DisplayColorKind.Normal, head * 9 + digit),
									new(DisplayColorKind.Normal, tail * 9 + digit)
								}
								| new HouseViewNode(DisplayColorKind.Auxiliary1, sameHouse)
						),
						digit,
						h1,
						h2
					);

					if (onlyFindOne)
					{
						return step;
					}

					accumulator.Add(step);
				}
			}
		}

		return null;
	}
}
