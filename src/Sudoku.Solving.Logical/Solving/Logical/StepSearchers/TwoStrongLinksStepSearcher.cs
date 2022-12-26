namespace Sudoku.Solving.Logical.StepSearchers;

[StepSearcher]
[StepSearcherMetadata(PuzzleNotRelying = true)]
internal sealed unsafe partial class TwoStrongLinksStepSearcher : ITwoStrongLinksStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(scoped in LogicalAnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		for (var digit = 0; digit < 9; digit++)
		{
			for (var h1 = 0; h1 < 26; h1++)
			{
				for (var h2 = h1 + 1; h2 < 27; h2++)
				{
					// Get masks.
					var mask1 = (HousesMap[h1] & CandidatesMap[digit]) / h1;
					var mask2 = (HousesMap[h2] & CandidatesMap[digit]) / h2;
					if (PopCount((uint)mask1) != 2 || PopCount((uint)mask2) != 2)
					{
						continue;
					}

					// Get all cells.
					var cells1 = CellMap.Empty;
					var cells2 = CellMap.Empty;
					var cellsList1 = new List<int>(PopCount((uint)mask1));
					var cellsList2 = new List<int>(PopCount((uint)mask2));
					foreach (var pos1 in mask1)
					{
						var cell1 = HouseCells[h1][pos1];
						cellsList1.Add(cell1);
						cells1.Add(cell1);
					}
					foreach (var pos2 in mask2)
					{
						var cell2 = HouseCells[h2][pos2];
						cellsList2.Add(cell2);
						cells2.Add(cell2);
					}

					if (cells1 && cells2)
					{
						continue;
					}

					// Check two cells share a same house.
					int sameHouse, headIndex, tailIndex, c1Index, c2Index;
					for (var i = 0; i < 2; i++)
					{
						var cell1 = cellsList1[i];
						for (var j = 0; j < 2; j++)
						{
							var cell2 = cellsList2[j];
							if ((CellsMap[cell1] + cell2).AllSetsAreInOneHouse(out sameHouse))
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

					if (context.OnlyFindOne)
					{
						return step;
					}

					context.Accumulator.Add(step);
				}
			}
		}

		return null;
	}
}
