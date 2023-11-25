using System.Numerics;
using System.Runtime.CompilerServices;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Metadata;
using Sudoku.Analytics.Steps;
using Sudoku.Analytics.StepSearcherModules;
using Sudoku.Concepts;
using Sudoku.Concepts.ObjectModel;
using Sudoku.Rendering;
using Sudoku.Rendering.Nodes;
using static System.Numerics.BitOperations;
using static Sudoku.Analytics.CachedFields;
using static Sudoku.Analytics.ConclusionType;

namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Death Blossom</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Death Blossom</item>
/// </list>
/// </summary>
[StepSearcher(Technique.DeathBlossom)]
[StepSearcherRuntimeName("StepSearcherName_DeathBlossomStepSearcher")]
public sealed partial class DeathBlossomStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates whether the step searcher searches for extended types.
	/// </summary>
	[RuntimeIdentifier(RuntimeIdentifier.SearchExtendedDeathBlossomTypes)]
	public bool SearchExtendedTypes { get; set; }


	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		scoped var alses = AlmostLockedSetsModule.CollectAlmostLockedSets(in grid);

		var ckIndex = new int[729];
		Array.Fill(ckIndex, -1);

		// Iterate on each cell to collect cell-blossom type.
		var cachedCands = grid.ToCandidateMaskArray();
		foreach (var i in EmptyCells)
		{
			if (PopCount((uint)cachedCands[i]) < 2)
			{
				continue;
			}

			var bCands = (Mask)(cachedCands[i] & ~(1 << Solution.GetDigit(i)));
			for (var j = 0; j < PopCount((uint)bCands); j++)
			{
				var vCand = bCands.SetAt(j);
				var psbCells = CellMap.Empty;
				var ckCands = grid.ToCandidateMaskArray();
				for (var k1 = 0; k1 < alses.Length; k1++)
				{
					if ((alses[k1].DigitsMask >> vCand & 1) != 0 && alses[k1].EliminationMap[vCand].Contains(i))
					{
						var cands = (Mask)(alses[k1].DigitsMask & ~(1 << vCand));
						for (var k2 = 0; k2 < PopCount((uint)cands); k2++)
						{
							scoped ref readonly var t = ref alses[k1].EliminationMap[cands.SetAt(k2)];
							for (var a = 0; a < t.Count; a++)
							{
								if ((ckCands[t[a]] >> cands.SetAt(k2) & 1) == 0)
								{
									continue;
								}

								ckCands[t[a]] &= (Mask)~(1 << cands.SetAt(k2));
								ckIndex[t[a] * 9 + cands.SetAt(k2)] = k1;
								psbCells.Add(t[a]);

								if (ckCands[t[a]] != 0)
								{
									continue;
								}

								var delCands = (Mask)0;
								var branches = new BlossomBranch();
								for (var b = 0; b < PopCount((uint)grid.GetCandidates(t[a])); b++)
								{
									var branch = alses[ckIndex[t[a] * 9 + grid.GetCandidates(t[a]).SetAt(b)]];
									branches.Add(grid.GetCandidates(t[a]).SetAt(b), branch);

									if (b == 0)
									{
										delCands = branch.DigitsMask;
									}
									else
									{
										delCands &= branch.DigitsMask;
									}
								}

								delCands &= (Mask)~grid.GetCandidates(t[a]);

								var temp = CellMap.Empty;
								for (var b = 0; b < PopCount((uint)delCands); b++)
								{
									var digit = delCands.SetAt(b);
									foreach (var branch in branches.Values)
									{
										temp |= branch.Cells & CandidatesMap[digit];
									}
								}

								var zz = (Mask)0;
								var conclusions = new List<Conclusion>();
								for (var b = 0; b < PopCount((uint)delCands); b++)
								{
									var delMap = temp % CandidatesMap[delCands.SetAt(b)];
									if (!delMap)
									{
										continue;
									}

									var ttt = delMap.ToArray();
									zz |= (Mask)(1 << delCands.SetAt(b));
									for (var c = 0; c < ttt.Length; c++)
									{
										conclusions.Add(new(Elimination, ttt[c], delCands.SetAt(b)));

										if (SearchExtendedTypes)
										{
											cachedCands[ttt[c]] &= (Mask)~delCands.SetAt(b);
										}
									}
								}

								var cellOffsets = new List<CellViewNode> { new(WellKnownColorIdentifier.Normal, t[a]) };
								var candidateOffsets = new List<CandidateViewNode>();
								var detailViews = new View[branches.Count];
								foreach (ref var view in detailViews.AsSpan())
								{
									view = [new CellViewNode(WellKnownColorIdentifier.Normal, t[a])];
								}

								var indexOfAls = 0;
								foreach (var digit in grid.GetCandidates(t[a]))
								{
									var node = new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, t[a] * 9 + digit);
									candidateOffsets.Add(node);

									detailViews[indexOfAls++].Add(node);
								}

								indexOfAls = 0;
								foreach (var (branchDigit, (_, alsCells)) in branches)
								{
									foreach (var alsCell in alsCells)
									{
										var alsColor = AlmostLockedSetsModule.GetColor(indexOfAls);
										foreach (var digit in grid.GetCandidates(alsCell))
										{
											var node = new CandidateViewNode(
												branchDigit == digit
													? WellKnownColorIdentifier.Auxiliary2
													: (delCands >> digit & 1) != 0
														? WellKnownColorIdentifier.Auxiliary1
														: alsColor,
												alsCell * 9 + digit
											);
											candidateOffsets.Add(node);

											detailViews[indexOfAls].Add(node);
										}

										var cellNode = new CellViewNode(alsColor, alsCell);
										cellOffsets.Add(cellNode);

										detailViews[indexOfAls].Add(cellNode);
									}

									indexOfAls++;
								}

								var step = new DeathBlossomStep(
									[.. conclusions],
									[[.. cellOffsets, .. candidateOffsets], .. detailViews],
									context.PredefinedOptions,
									t[a],
									branches,
									zz
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

		if (!SearchExtendedTypes)
		{
			return null;
		}

		// Search for extended types.


		return null;
	}
}
