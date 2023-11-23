using System.Numerics;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Metadata;
using Sudoku.Analytics.Steps;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Rendering.Nodes;
using static Sudoku.Analytics.CachedFields;
using static Sudoku.Analytics.ConclusionType;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Grouped Two-strong Links</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Grouped Skyscraper</item>
/// <item>Grouped Two-string Kite</item>
/// <item>Grouped Turbot Fish</item>
/// </list>
/// </summary>
[StepSearcher(Technique.GroupedSkyscraper, Technique.GroupedTwoStringKite, Technique.GroupedTurbotFish, OnlyUsesCachedFields = true)]
[StepSearcherRuntimeName("StepSearcherName_GroupedTwoStrongLinksStepSearcher")]
public sealed partial class GroupedTwoStrongLinksStepSearcher : StepSearcher
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
					// Check whether digit appeared in both two houses can be treated as two grouped or normal nodes.
					// i.e. For each house, we just determine whether 2 bit group is not 0 in each 3 bits in a mask.
					var (mask1, mask2) = ((HousesMap[h1] & CandidatesMap[digit]) / h1, (HousesMap[h2] & CandidatesMap[digit]) / h2);
					var mask1Subgroups = new List<Mask>(3);
					var mask2Subgroups = new List<Mask>(3);
					for (var (tempMask, i) = (mask1, 0); i < 3; tempMask >>= 3, i++)
					{
						if ((tempMask & 7) is var target and not 0)
						{
							mask1Subgroups.Add((Mask)(target << i * 3));
						}
					}
					for (var (tempMask, i) = (mask2, 0); i < 3; tempMask >>= 3, i++)
					{
						if ((tempMask & 7) is var target and not 0)
						{
							mask2Subgroups.Add((Mask)(target << i * 3));
						}
					}
					if (mask1Subgroups.Count != 2 || mask2Subgroups.Count != 2)
					{
						continue;
					}

					// Now we should iterate on each combination to determine which 2 cells group intersects in a same house.
					// If so, it will form a weak link.
					var (cellsForHouse1, cellsForHouse2) = ((CellMap[])[CellMap.Empty, CellMap.Empty], (CellMap[])[CellMap.Empty, CellMap.Empty]);
					foreach (var pos in mask1Subgroups[0])
					{
						scoped ref var currentMap = ref cellsForHouse1[0];
						currentMap.Add(HouseCells[h1][pos]);
					}
					foreach (var pos in mask1Subgroups[1])
					{
						scoped ref var currentMap = ref cellsForHouse1[1];
						currentMap.Add(HouseCells[h1][pos]);
					}
					foreach (var pos in mask2Subgroups[0])
					{
						scoped ref var currentMap = ref cellsForHouse2[0];
						currentMap.Add(HouseCells[h2][pos]);
					}
					foreach (var pos in mask2Subgroups[1])
					{
						scoped ref var currentMap = ref cellsForHouse2[1];
						currentMap.Add(HouseCells[h2][pos]);
					}
					foreach (var (cells1, cells2, headCells, tailCells) in (
						(cellsForHouse1[0], cellsForHouse2[0], cellsForHouse1[1], cellsForHouse2[1]),
						(cellsForHouse1[0], cellsForHouse2[1], cellsForHouse1[1], cellsForHouse2[0]),
						(cellsForHouse1[1], cellsForHouse2[0], cellsForHouse1[0], cellsForHouse2[1]),
						(cellsForHouse1[1], cellsForHouse2[1], cellsForHouse1[0], cellsForHouse2[0])
					))
					{
						// Check whether they are in a same house.
						if (!(cells1 | cells2).InOneHouse(out var weakLinkHouse) || !!(cells1 & cells2))
						{
							continue;
						}

						// Determine whether the pattern is a non-grouped one. If so, we should skip for it
						// because it is handled by non-grouped step searcher.
						if (cells1.Count * cells2.Count * headCells.Count * tailCells.Count == 1)
						{
							continue;
						}

						// Check elimination.
						var elimMap = headCells.PeerIntersection & tailCells.PeerIntersection & CandidatesMap[digit];
						if (!elimMap)
						{
							continue;
						}

						// A grouped two-strong link is found.
						var step = new TwoStrongLinksStep(
							[.. from cell in elimMap select new Conclusion(Elimination, cell, digit)],
							[
								[
									.. from cell in headCells select new CandidateViewNode(WellKnownColorIdentifier.Normal, cell * 9 + digit),
									.. from cell in tailCells select new CandidateViewNode(WellKnownColorIdentifier.Normal, cell * 9 + digit),
									.. from cell in cells1 select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary1, cell * 9 + digit),
									.. from cell in cells2 select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary1, cell * 9 + digit),
									new HouseViewNode(WellKnownColorIdentifier.Normal, h1),
									new HouseViewNode(WellKnownColorIdentifier.Normal, h2),
									new HouseViewNode(WellKnownColorIdentifier.Auxiliary1, weakLinkHouse)
								]
							],
							context.PredefinedOptions,
							digit,
							h1,
							h2,
							true
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

		return null;
	}
}
