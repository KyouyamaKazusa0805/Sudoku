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
using static Sudoku.SolutionWideReadOnlyFields;

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

		var indexUsed2All = new int[10];
		var usedAls = new CellMap[10, 9];
		var usedIndex = new int[729];
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
				int sz;
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

									zz |= (Mask)(1 << delCands.SetAt(b));
									for (var c = 0; c < delMap.Count; c++)
									{
										conclusions.Add(new(Elimination, delMap[c], delCands.SetAt(b)));

										if (SearchExtendedTypes)
										{
											cachedCands[delMap[c]] &= (Mask)~delCands.SetAt(b);
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

				if (!SearchExtendedTypes || !psbCells)
				{
					return null;
				}

				var ttt = PeersMap[i] & CandidatesMap[vCand];
				for (var a = 0; a < ttt.Count; a++)
				{
					ckCands[ttt[a]] &= (Mask)~(1 << vCand);
				}

				var fCell = new int[9];
				var xCand = new int[9];
				var xAls = new int[9];
				foreach (var un in psbCells.Houses)
				{
					if ((HousesMap[un] & psbCells) is not (var fMap and not []))
					{
						continue;
					}

					var preDelCnt = -1;
					for (var a = 0; a < fMap.Count; a++)
					{
						if (ckCands[fMap[a]] == 0)
						{
							continue;
						}

						fCell[++preDelCnt] = fMap[a];
					}

					fMap = (HousesMap[un] & EmptyCells) - fMap;
					var tCnt = preDelCnt;
					if (fMap)
					{
						for (var a = 0; a < fMap.Count; a++)
						{
							if (PopCount((uint)ckCands[fMap[a]]) < 2)
							{
								continue;
							}

							fCell[++tCnt] = fMap[a];
						}
					}

					for (sz = 2; sz <= 8; sz++)
					{
						for (var a = 0; a <= preDelCnt; a++)
						{
							xCand[0] = ckCands[fCell[a]];
							xAls[0] = fCell[a];
							for (var b = a + 1; b <= tCnt; b++)
							{
								xCand[1] = xCand[0] | (int)ckCands[fCell[b]];
								xAls[1] = fCell[b];
								if (PopCount((uint)xCand[1]) < 2)
								{
									goto AlmostAlmostLockedSetDeletion;
								}

								if (sz < 3)
								{
									continue;
								}

								for (var c = b + 1; c <= tCnt; c++)
								{
									xCand[2] = xCand[1] | (int)ckCands[fCell[c]];
									xAls[2] = fCell[c];
									if (PopCount((uint)xCand[2]) < 3)
									{
										goto AlmostAlmostLockedSetDeletion;
									}

									if (sz < 4)
									{
										continue;
									}

									for (var d = c + 1; d <= tCnt; d++)
									{
										xCand[3] = xCand[2] | (int)ckCands[fCell[d]];
										xAls[3] = fCell[d];
										if (PopCount((uint)xCand[3]) < 4)
										{
											goto AlmostAlmostLockedSetDeletion;
										}

										if (sz < 5)
										{
											continue;
										}

										for (var e = d + 1; e <= tCnt; e++)
										{
											xCand[4] = xCand[3] | (int)ckCands[fCell[e]];
											xAls[4] = fCell[e];
											if (PopCount((uint)xCand[4]) < 5)
											{
												goto AlmostAlmostLockedSetDeletion;
											}

											if (sz < 6)
											{
												continue;
											}

											for (var f = e + 1; f <= tCnt; f++)
											{
												xCand[5] = xCand[4] | (int)ckCands[fCell[f]];
												xAls[5] = fCell[f];
												if (PopCount((uint)xCand[5]) < 6)
												{
													goto AlmostAlmostLockedSetDeletion;
												}

												if (sz < 7)
												{
													continue;
												}

												for (var g = f + 1; g <= tCnt; g++)
												{
													xCand[6] = xCand[5] | (int)ckCands[fCell[g]];
													xAls[6] = fCell[g];
													if (PopCount((uint)xCand[6]) < 7)
													{
														goto AlmostAlmostLockedSetDeletion;
													}

													if (sz < 8)
													{
														continue;
													}

													for (var h = g + 1; h <= tCnt; h++)
													{
														xCand[7] = xCand[6] | (int)ckCands[fCell[h]];
														xAls[7] = fCell[h];
														if (PopCount((uint)xCand[7]) < 8)
														{
															goto AlmostAlmostLockedSetDeletion;
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}

				continue;

			AlmostAlmostLockedSetDeletion:
				{
					var zz = (Mask)0;
					var numOfUse = 0;
					Array.Clear(usedAls);
					Array.Clear(usedIndex);
					var setCands = (Mask)0;
					var allAls = CellMap.Empty;
					var tCand = (Mask)0;
					var clrCands = grid.ToCandidateMaskArray();

					for (var ll = 0; ll < sz; ll++)
					{
						setCands |= grid.GetCandidates(xAls[ll]);
						tCand = (Mask)(grid.GetCandidates(xAls[ll]) & ~(xCand[sz - 1] | 1 << vCand));
						if (tCand == 0)
						{
							continue;
						}

						for (var ab = 0; ab < PopCount((uint)tCand); ab++)
						{
							if (usedIndex[ckIndex[xAls[ll] * 9 + tCand.SetAt(ab)]] == 0)
							{
								usedIndex[ckIndex[xAls[ll] * 9 + tCand.SetAt(ab)]] = ++numOfUse;
								indexUsed2All[usedIndex[ckIndex[xAls[ll] * 9 + tCand.SetAt(ab)]]] = ckIndex[xAls[ll] * 9 + tCand.SetAt(ab)];
							}

							if (numOfUse > 10)
							{
								throw new InvalidOperationException("There's a special case that more than 10 branches found.");
							}

							usedAls[usedIndex[ckIndex[xAls[ll] * 9 + tCand.SetAt(ab)]], tCand.SetAt(ab)].Add(xAls[ll]);
							if (zz == 0)
							{
								zz = (Mask)(
									alses[indexUsed2All[usedIndex[ckIndex[xAls[ll] * 9 + tCand.SetAt(ab)]]]].DigitsMask
										& ~(1 << tCand.SetAt(ab))
								);
							}
							else
							{
								zz &= (Mask)(
									alses[indexUsed2All[usedIndex[ckIndex[xAls[ll] * 9 + tCand.SetAt(ab)]]]].DigitsMask
										& ~(1 << tCand.SetAt(ab))
								);
							}
						}
					}

					var complexType = (setCands >> vCand & 1) != 0 && (xCand[sz - 1] >> vCand & 1) == 0 ? 1 : 2;
					if (complexType == 1)
					{
						zz &= (Mask)(xCand[sz - 1] | 1 << vCand);
					}

					var cellOffsets = new List<CellViewNode>();
					var candidateOffsets = new List<CandidateViewNode>();
					var alsIndex = 0;
					var detailViews = new List<View>(9);
					var branches = new BlossomBranch();
					var nTimesAlsDigitsMask = (Mask)0;
					var nTimesAlsCells = CellMap.Empty;
					for (var usedAlsIndex = 1; usedAlsIndex <= numOfUse; usedAlsIndex++)
					{
						var rcc = (Mask)0;
						var view = new View();
						var branchDigit = -1;
						for (var currentDigit = 0; currentDigit < 9; currentDigit++)
						{
							if (!usedAls[usedAlsIndex, currentDigit])
							{
								continue;
							}

							nTimesAlsDigitsMask |= (Mask)(1 << currentDigit);
							branchDigit = currentDigit;
							rcc |= (Mask)(1 << currentDigit);
							foreach (var cell in usedAls[usedAlsIndex, currentDigit])
							{
								if (grid.Exists(cell, currentDigit) is true)
								{
									var candidateNode = new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, cell * 9 + currentDigit);
									view.Add(candidateNode);
									candidateOffsets.Add(candidateNode);
								}

								var node = new CellViewNode(WellKnownColorIdentifier.Normal, cell);
								view.Add(node);
								cellOffsets.Add(node);
								clrCands[cell] &= (Mask)~tCand;

								nTimesAlsCells.Add(cell);
							}
						}

						allAls |= alses[indexUsed2All[usedAlsIndex]].Cells;
						var targetAls = alses[indexUsed2All[usedAlsIndex]];
						foreach (var cell in targetAls.Cells)
						{
							var cellNode = new CellViewNode(AlmostLockedSetsModule.GetColor(alsIndex), cell);
							view.Add(cellNode);
							cellOffsets.Add(cellNode);

							foreach (var digit in grid.GetCandidates(cell))
							{
								var colorIdentifier = (rcc >> digit & 1) != 0
									? WellKnownColorIdentifier.Auxiliary2
									: (zz >> digit & 1) != 0
										? WellKnownColorIdentifier.Auxiliary1
										: AlmostLockedSetsModule.GetColor(alsIndex);
								var candidateNode = new CandidateViewNode(colorIdentifier, cell * 9 + digit);
								view.Add(candidateNode);
								candidateOffsets.Add(candidateNode);
							}
						}

						branches.TryAdd(branchDigit, targetAls);
						detailViews.Add(view);
						alsIndex++;
					}

					var temp = (CellMap)xAls[..sz];
					//var rank0 = false;
					var conclusions = new List<Conclusion>();
					for (var ll = 0; ll < PopCount((uint)zz); ll++)
					{
						var delMap = allAls;
						if (complexType == 1)
						{
							delMap |= temp;
						}

						delMap &= CandidatesMap[zz.SetAt(ll)];
						//if (((allAls | temp) & CandidatesMap[zz.SetAt(ll)]).InOneHouse(out _))
						//{
						//	rank0 = true;
						//}

						delMap = delMap.PeerIntersection & CandidatesMap[zz.SetAt(ll)];
						if (!delMap)
						{
							continue;
						}

						for (var ii = 0; ii < delMap.Count; ii++)
						{
							conclusions.Add(new(Elimination, delMap[ii], zz.SetAt(ll)));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var step = new NTimesAlmostLockedSetDeathBlossomStep(
						[.. conclusions],
						[[.. cellOffsets, .. candidateOffsets], .. detailViews],
						context.PredefinedOptions,
						nTimesAlsDigitsMask,
						in nTimesAlsCells,
						branches,
						PopCount((uint)(grid[in nTimesAlsCells] & ~nTimesAlsDigitsMask)) + 1
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
