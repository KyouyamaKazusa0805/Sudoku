namespace Sudoku.Solving.Manual.Searchers.SingleDigitPatterns;

/// <summary>
/// Provides with a <b>Two-strong Links</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Skyscraper</item>
/// <item>Two-string Kite</item>
/// <item>Turbot Fish</item>
/// </list>
/// </summary>
[StepSearcher(PuzzleNotRelying = true)]
public sealed unsafe class TwoStrongLinksStepSearcher : ITwoStrongLinksStepSearcher
{
	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(5, DisplayingLevel.B);

	/// <inheritdoc/>
	public delegate*<in Grid, bool> Predicate
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => null;
	}


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		for (int digit = 0; digit < 9; digit++)
		{
			for (int r1 = 0; r1 < 26; r1++)
			{
				for (int r2 = r1 + 1; r2 < 27; r2++)
				{
					// Get masks.
					short mask1 = (RegionMaps[r1] & CandMaps[digit]) / r1;
					short mask2 = (RegionMaps[r2] & CandMaps[digit]) / r2;
					if (PopCount((uint)mask1) != 2 || PopCount((uint)mask2) != 2)
					{
						continue;
					}

					// Get all cells.
					Cells map1 = Cells.Empty, map2 = Cells.Empty;
					List<int> cells1 = new(), cells2 = new();
					foreach (int pos1 in mask1)
					{
						int cell1 = RegionCells[r1][pos1];
						cells1.Add(cell1);
						map1.AddAnyway(cell1);
					}
					foreach (int pos2 in mask2)
					{
						int cell2 = RegionCells[r2][pos2];
						cells2.Add(cell2);
						map2.AddAnyway(cell2);
					}

					if (!(map1 & map2).IsEmpty)
					{
						continue;
					}

					// Check two cells share a same region.
					int sameRegion, headIndex, tailIndex, c1Index, c2Index;
					for (int i = 0; i < 2; i++)
					{
						int cell1 = cells1[i];
						for (int j = 0; j < 2; j++)
						{
							int cell2 = cells2[j];
							if (new Cells { cell1, cell2 }.AllSetsAreInOneRegion(out sameRegion))
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
					if ((PeerMaps[head] & PeerMaps[tail] & CandMaps[digit]) is not { IsEmpty: false } gridMap)
					{
						continue;
					}

					var step = new TwoStrongLinksStep(
						gridMap.ToImmutableConclusions(digit),
						ImmutableArray.Create(new PresentationData
						{
							Candidates = new[]
							{
								(cells1[c1Index] * 9 + digit, (ColorIdentifier)0),
								(cells2[c2Index] * 9 + digit, (ColorIdentifier)0),
								(head * 9 + digit, (ColorIdentifier)0),
								(tail * 9 + digit, (ColorIdentifier)0)
							},
							Regions = new[] { (sameRegion, (ColorIdentifier)1) }
						}),
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
