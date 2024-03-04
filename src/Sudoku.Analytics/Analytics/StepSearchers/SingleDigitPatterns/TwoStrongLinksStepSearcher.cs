namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Two-strong Links</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Skyscraper</item>
/// <item>Two-string Kite</item>
/// <item>Turbot Fish</item>
/// </list>
/// </summary>
[StepSearcher(Technique.Skyscraper, Technique.TwoStringKite, Technique.TurbotFish, OnlyUsesCachedFields = true)]
[StepSearcherRuntimeName("StepSearcherName_TwoStrongLinksStepSearcher")]
public sealed partial class TwoStrongLinksStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		for (var digit = 0; digit < 9; digit++)
		{
			for (var h1 = 0; h1 < 26; h1++)
			{
				for (var h2 = h1 + 1; h2 < 27; h2++)
				{
					// Get masks.
					var (mask1, mask2) = ((HousesMap[h1] & CandidatesMap[digit]) / h1, (HousesMap[h2] & CandidatesMap[digit]) / h2);
					if (PopCount((uint)mask1) != 2 || PopCount((uint)mask2) != 2)
					{
						continue;
					}

					// Get all cells.
					var (cells1, cells2) = ((CellMap)[], (CellMap)[]);
					var cellsList1 = new List<Cell>(2);
					var cellsList2 = new List<Cell>(2);
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
					House sameHouse;
					int headIndex, tailIndex, c1Index, c2Index;
					for (var i = 0; i < 2; i++)
					{
						var cell1 = cellsList1[i];
						for (var j = 0; j < 2; j++)
						{
							var cell2 = cellsList2[j];
							if (((CellMap)cell1 + cell2).InOneHouse(out sameHouse))
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
					var (head, tail) = (cellsList1[headIndex], cellsList2[tailIndex]);
					if (((CellMap)head + tail).InOneHouse(out _))
					{
						// A standard X-Wing pattern.
						continue;
					}

					if ((PeersMap[head] & PeersMap[tail] & CandidatesMap[digit]) is not (var elimMap and not []))
					{
						continue;
					}

					var step = new TwoStrongLinksStep(
						[.. from cell in elimMap select new Conclusion(Elimination, cell, digit)],
						[
							[
								new CandidateViewNode(ColorIdentifier.Normal, cellsList1[c1Index] * 9 + digit),
								new CandidateViewNode(ColorIdentifier.Normal, cellsList2[c2Index] * 9 + digit),
								new CandidateViewNode(ColorIdentifier.Normal, head * 9 + digit),
								new CandidateViewNode(ColorIdentifier.Normal, tail * 9 + digit),
								new HouseViewNode(ColorIdentifier.Normal, h1),
								new HouseViewNode(ColorIdentifier.Normal, h2),
								new HouseViewNode(ColorIdentifier.Auxiliary1, sameHouse)
							]
						],
						context.PredefinedOptions,
						digit,
						h1,
						h2,
						false
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
