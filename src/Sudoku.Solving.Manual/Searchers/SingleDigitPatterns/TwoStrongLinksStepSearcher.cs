namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Two-strong Links</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Skyscraper</item>
/// <item>Two-string Kite</item>
/// <item>Turbot Fish</item>
/// </list>
/// </summary>
[StepSearcher]
[StepSearcherOptions(PuzzleNotRelying = true)]
public sealed unsafe partial class TwoStrongLinksStepSearcher : ITwoStrongLinksStepSearcher
{
	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		for (int digit = 0; digit < 9; digit++)
		{
			for (int r1 = 0; r1 < 26; r1++)
			{
				for (int r2 = r1 + 1; r2 < 27; r2++)
				{
					// Get masks.
					short mask1 = (HouseMaps[r1] & CandidatesMap[digit]) / r1;
					short mask2 = (HouseMaps[r2] & CandidatesMap[digit]) / r2;
					if (PopCount((uint)mask1) != 2 || PopCount((uint)mask2) != 2)
					{
						continue;
					}

					// Get all cells.
					var map1 = Cells.Empty;
					var map2 = Cells.Empty;
					var cells1 = new List<int>();
					var cells2 = new List<int>();
					foreach (int pos1 in mask1)
					{
						int cell1 = HouseCells[r1][pos1];
						cells1.Add(cell1);
						map1.Add(cell1);
					}
					foreach (int pos2 in mask2)
					{
						int cell2 = HouseCells[r2][pos2];
						cells2.Add(cell2);
						map2.Add(cell2);
					}

					if ((map1 & map2) is not [])
					{
						continue;
					}

					// Check two cells share a same house.
					int sameHouse, headIndex, tailIndex, c1Index, c2Index;
					for (int i = 0; i < 2; i++)
					{
						int cell1 = cells1[i];
						for (int j = 0; j < 2; j++)
						{
							int cell2 = cells2[j];
							if ((Cells.Empty + cell1 + cell2).AllSetsAreInOneHouse(out sameHouse))
							{
								c1Index = i;
								c2Index = j;
								headIndex = i == 0 ? 1 : 0;
								tailIndex = j == 0 ? 1 : 0;
								goto Checking;
							}
						}
					}

					// Not same block.
					continue;

				Checking:
					// Two strong link found.
					// Record all eliminations.
					int head = cells1[headIndex], tail = cells2[tailIndex];
					if ((PeerMaps[head] & PeerMaps[tail] & CandidatesMap[digit]) is not { Count: not 0 } gridMap)
					{
						continue;
					}

					var step = new TwoStrongLinksStep(
						ImmutableArray.Create(
							Conclusion.ToConclusions(gridMap, digit, ConclusionType.Elimination)
						),
						ImmutableArray.Create(
							View.Empty
								| new CandidateViewNode[]
								{
									new(DisplayColorKind.Normal, cells1[c1Index] * 9 + digit),
									new(DisplayColorKind.Normal, cells2[c2Index] * 9 + digit),
									new(DisplayColorKind.Normal, head * 9 + digit),
									new(DisplayColorKind.Normal, tail * 9 + digit)
								}
								| new HouseViewNode(DisplayColorKind.Auxiliary1, sameHouse)
						),
						digit,
						r1,
						r2
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
