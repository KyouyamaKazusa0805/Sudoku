using Sudoku.Collections;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;

namespace Sudoku.Test;

partial class AicSearcher
{
	/// <summary>
	/// Gather the strong and weak inferences on sole candidate nodes.
	/// </summary>
	/// <param name="grid">The grid.</param>
	private void GatherStrongAndWeak_Sole(in Grid grid)
	{
		// Sole candidate -> Sole candidate.
		foreach (int candidate in grid)
		{
			byte cell = (byte)(candidate / 9), digit = (byte)(candidate % 9);
			var node = new SoleCandidateNode((byte)(candidate / 9), (byte)(candidate % 9));
			getStrong(grid);
			getWeak(grid);


			void getStrong(in Grid grid)
			{
				HashSet<int>? list = null;

				// Get bi-location regions.
				if (NodeTypes.Flags(SearcherNodeTypes.SoleDigit))
				{
					foreach (var label in Regions)
					{
						int region = cell.ToRegionIndex(label);
						var posCells = (RegionMaps[region] & grid.CandidatesMap[digit]) - cell;
						if (posCells is [var posCell])
						{
							var nextNode = new SoleCandidateNode((byte)posCell, digit);
							AddNode(nextNode, ref list);
						}
					}
				}

				// Get bi-value cell.
				if (NodeTypes.Flags(SearcherNodeTypes.SoleCell))
				{
					short candidateMask = grid.GetCandidates(cell);
					if (PopCount((uint)candidateMask) == 2)
					{
						byte theOtherDigit = (byte)Log2((uint)(candidateMask & ~(1 << digit)));
						var nextNode = new SoleCandidateNode(cell, theOtherDigit);
						AddNode(nextNode, ref list);
					}
				}

				AssignOrUpdateHashSet(list, node, _strongInferences);
			}

			void getWeak(in Grid grid)
			{
				HashSet<int>? list = null;

				if (NodeTypes.Flags(SearcherNodeTypes.SoleDigit))
				{
					foreach (byte anotherCell in (PeerMaps[cell] & grid.CandidatesMap[digit]) - cell)
					{
						var nextNode = new SoleCandidateNode(anotherCell, digit);
						AddNode(nextNode, ref list);
					}
				}

				if (NodeTypes.Flags(SearcherNodeTypes.SoleCell))
				{
					foreach (byte anotherDigit in grid.GetCandidates(cell) & ~(1 << digit))
					{
						var nextNode = new SoleCandidateNode(cell, anotherDigit);
						AddNode(nextNode, ref list);
					}
				}

				AssignOrUpdateHashSet(list, node, _weakInferences);
			}
		}
	}

	/// <summary>
	/// Gather the strong and weak inferences on locked candidates nodes.
	/// </summary>
	/// <param name="grid">The grid.</param>
	private void GatherStrongAndWeak_LockedCandidates(in Grid grid)
	{
		// Sole candidate -> Locked candidates.
		foreach (int candidate in grid)
		{
			byte cell = (byte)(candidate / 9), digit = (byte)(candidate % 9);
			var node = new SoleCandidateNode(cell, digit);
			getStrong(grid);
			getWeak(grid);


			void getStrong(in Grid grid)
			{
				HashSet<int>? list = null;

				if (NodeTypes.Flags(SearcherNodeTypes.LockedCandidates))
				{
					foreach (var label in Regions)
					{
						int region = cell.ToRegionIndex(label);
						var otherCells = RegionMaps[region] & grid.CandidatesMap[digit] - cell;
						if (
							otherCells is not
							{
								Count: > 1 and <= 3,
								CoveredLine: not Constants.InvalidFirstSet,
								CoveredRegions: var coveredRegions
							}
						)
						{
							// Optimization:
							// 1) If the number of all other cells in the current region
							// is greater than 3, the region doesn't hold a valid strong inference
							// from the current candidate to a locked candidates,
							// because a locked candidates node at most use 3 cells.
							// 2) If all other cells don't lie in a same row or column, those cells
							// can still not form a locked candidates node.
							continue;
						}

						if (TrailingZeroCount(coveredRegions) >= 9)
						{
							// The cells must be in a same block.
							continue;
						}

						var nextNode = new LockedCandidatesNode(digit, otherCells);
						AddNode(nextNode, ref list);
					}
				}

				AssignOrUpdateHashSet(list, node, _strongInferences);
			}

			void getWeak(in Grid grid)
			{
				HashSet<int>? list = null;
				using var possibleList = new ValueList<Cells>(4);
				var triplets = (stackalloc Cells[3]);

				if (NodeTypes.Flags(SearcherNodeTypes.LockedCandidates))
				{
					foreach (var label in Regions)
					{
						int region = cell.ToRegionIndex(label);
						var otherCells = RegionMaps[region] & grid.CandidatesMap[digit] - cell;
						if (otherCells.Count <= 1)
						{
							continue;
						}

						// Okay. Now we get a set of cells.
						// Now we should gather all possible covered rows and columns.
						if (label == Region.Block)
						{
							int block = cell.ToRegionIndex(Region.Block);
							foreach (int subRegions in otherCells.RowMask << 9 | otherCells.ColumnMask << 18)
							{
								var intersectionMap = RegionMaps[block] & RegionMaps[subRegions];
								var subOtherCells = grid.CandidatesMap[digit] & intersectionMap - cell;
								if (subOtherCells.Count is not (var count and not (0 or 1)))
								{
									continue;
								}

								possibleList.Clear();
								if (count == 2)
								{
									possibleList.Add(subOtherCells);
								}
								else// if (count == 3)
								{
									var combinations = subOtherCells & 2;
									possibleList.Add(combinations[0]);
									possibleList.Add(combinations[1]);
									possibleList.Add(combinations[2]);
									possibleList.Add(subOtherCells);
								}

								foreach (var cellsCombination in possibleList)
								{
									var nextNode = new LockedCandidatesNode(digit, cellsCombination);
									AddNode(nextNode, ref list);
								}
							}
						}
						else
						{
							triplets.Clear();
							triplets[0] = RegionMaps[region][0..3];
							triplets[1] = RegionMaps[region][3..6];
							triplets[2] = RegionMaps[region][6..9];

							foreach (ref readonly var triplet in triplets)
							{
								var subOtherCells = triplet & otherCells;
								if (subOtherCells.Count is not (var count and not (0 or 1)))
								{
									continue;
								}

								possibleList.Clear();
								if (count == 2)
								{
									possibleList.Add(subOtherCells);
								}
								else// if (count == 3)
								{
									var combinations = subOtherCells & 2;
									possibleList.Add(combinations[0]);
									possibleList.Add(combinations[1]);
									possibleList.Add(combinations[2]);
									possibleList.Add(subOtherCells);
								}

								foreach (var cellsCombination in possibleList)
								{
									var nextNode = new LockedCandidatesNode(digit, cellsCombination);
									AddNode(nextNode, ref list);
								}
							}
						}
					}
				}

				AssignOrUpdateHashSet(list, node, _weakInferences);
			}
		}

		// Locked candidates -> Sole candidate.
		// Locked candidates -> Locked candidates.
		foreach (var (lineMap, blockMap, intersectionMap, _) in IntersectionMaps.Values)
		{
			foreach (ref readonly var cellCombination in stackalloc[]
			{
				(intersectionMap & 2)[0],
				(intersectionMap & 2)[1],
				(intersectionMap & 2)[2],
				(intersectionMap & 3)[0]
			})
			{
				foreach (byte digit in grid.GetDigitsUnion(cellCombination))
				{
					if ((cellCombination & grid.CandidatesMap[digit]) != cellCombination)
					{
						continue;
					}

					var node = new LockedCandidatesNode(digit, cellCombination);
					getStrong(grid, cellCombination);
					getWeak(grid, cellCombination);


					void getStrong(in Grid grid, in Cells cells)
					{
						HashSet<int>? list = null;

						if (NodeTypes.Flags(SearcherNodeTypes.LockedCandidates))
						{
							foreach (int region in cells.CoveredRegions)
							{
								var node = (RegionMaps[region] & grid.CandidatesMap[digit] - cells) switch
								{
									// e.g. aaa==a
									[var onlyCell] when NodeTypes.Flags(SearcherNodeTypes.SoleDigit) =>
										new SoleCandidateNode((byte)onlyCell, digit),

									// e.g. aaa==aaa
									{
										CoveredLine: not Constants.InvalidFirstSet,
										CoveredRegions: var coveredRegions
									} otherCells => region.ToRegion() switch
									{
										Region.Block => new LockedCandidatesNode(digit, otherCells),
										_ => TrailingZeroCount(coveredRegions) switch
										{
											< 9 => new LockedCandidatesNode(digit, otherCells),
											_ => default(Node?)
										}
									},

									// Other cases that the following cases cannot satisfy.
									_ => null
								};
								if (node is not null)
								{
									AddNode(node, ref list);
								}
							}
						}

						AssignOrUpdateHashSet(list, node, _strongInferences);
					}

					void getWeak(in Grid grid, in Cells cells)
					{
						HashSet<int>? list = null;
						var triplets = (stackalloc Cells[3]);

						if (NodeTypes.Flags(SearcherNodeTypes.LockedCandidates))
						{
							foreach (int region in cells.CoveredRegions)
							{
								var otherCells = grid.CandidatesMap[digit] & RegionMaps[region] - cells;
								if (region.ToRegion() == Region.Block)
								{
									foreach (int subRegion in otherCells.RowMask << 9 | otherCells.ColumnMask << 18)
									{
										var intersectionMap = RegionMaps[region] & RegionMaps[subRegion];
										var subOtherCells = otherCells & intersectionMap;
										switch (subOtherCells)
										{
											case [var cell] when NodeTypes.Flags(SearcherNodeTypes.SoleDigit):
											{
												var nextNode = new SoleCandidateNode((byte)cell, digit);
												AddNode(nextNode, ref list);

												break;
											}
											case { Count: var count }:
											{
												for (int i = 0; i < count; i++)
												{
													foreach (var cellsCombination in subOtherCells & i)
													{
														if (cellsCombination is [var onlyCell])
														{
															if (NodeTypes.Flags(SearcherNodeTypes.SoleDigit))
															{
																AddNode(
																	new SoleCandidateNode(
																		(byte)onlyCell,
																		digit),
																	ref list);
															}
														}
														else
														{
															AddNode(
																new LockedCandidatesNode(
																	digit,
																	cellsCombination),
																ref list);
														}
													}
												}

												break;
											}
										}
									}
								}
								else
								{
									triplets.Clear();
									triplets[0] = RegionMaps[region][0..3];
									triplets[1] = RegionMaps[region][3..6];
									triplets[2] = RegionMaps[region][6..9];

									foreach (ref readonly var triplet in triplets)
									{
										var subOtherCells = triplet & otherCells;
										if (subOtherCells is not { Count: var count and not 0 })
										{
											continue;
										}

										for (int i = 0; i < count; i++)
										{
											foreach (var cellsCombination in subOtherCells & i)
											{
												if (cellsCombination is [var onlyCell])
												{
													if (NodeTypes.Flags(SearcherNodeTypes.SoleDigit))
													{
														AddNode(
															new SoleCandidateNode((byte)onlyCell, digit),
															ref list);
													}
												}
												else
												{
													AddNode(
														new LockedCandidatesNode(digit, cellsCombination),
														ref list);
												}
											}
										}
									}
								}
							}
						}

						AssignOrUpdateHashSet(list, node, _weakInferences);
					}
				}
			}
		}
	}

	/// <summary>
	/// Gather the strong and weak inferences on almost locked sets nodes.
	/// </summary>
	/// <param name="grid">The grid.</param>
	private void GatherStrongAndWeak_AlmostLockedSet(in Grid grid)
	{
		var alses = AlmostLockedSet.Gather(grid);
		foreach (ref readonly var als in alses.EnumerateRef())
		{
			_ = als is { DigitsMask: var digitsMask, Map: var alsMap };

			// Sole candidate -> Almost locked sets.
			// Locked candidates -> Almost locked sets.
			// Almost locked sets -> Almost locked sets.
			// Almost locked sets -> Sole candidate.
			// Almost locked sets -> Locked candidates.
			foreach (byte digit in digitsMask)
			{
				var digitCellsUsed = grid.CandidatesMap[digit] & alsMap;
				var cellsNotUsed = alsMap - digitCellsUsed;
				var node = new AlmostLockedSetNode(digit, digitCellsUsed, cellsNotUsed);
				getStrong(grid);
				getWeak(grid);


				void getStrong(in Grid grid)
				{
					HashSet<int>? list = null;
					HashSet<int>? list2 = null;

					if (NodeTypes.Flags(SearcherNodeTypes.LockedSet))
					{
						//foreach (int region in digitCellsUsed.CoveredRegions)
						//{
						//	var otherCells = RegionMaps[region] & grid.CandidatesMap[digit] - digitCellsUsed;
						//	if (otherCells.Count > 3)
						//	{
						//		continue;
						//	}

						//	if (
						//		region.ToRegion() is var label && (
						//			label == Region.Block
						//			&& otherCells.CoveredLine == Constants.InvalidFirstSet
						//			|| label is Region.Row or Region.Column
						//			&& TrailingZeroCount(otherCells.CoveredRegions) >= 9
						//		)
						//	)
						//	{
						//		continue;
						//	}

						//	if (otherCells is [var onlyCell])
						//	{
						//		if (NodeTypes.Flags(SearcherNodeTypes.SoleDigit))
						//		{
						//			var nextNode = new SoleCandidateNode((byte)onlyCell, digit);
						//			AddNode(nextNode, ref list);
						//			AddNode(node, ref list2);
						//			AssignOrUpdateHashSet(list2, nextNode, _strongInferences);
						//		}
						//	}
						//	else
						//	{
						//		if (NodeTypes.Flags(SearcherNodeTypes.LockedCandidates))
						//		{
						//			var nextNode = new LockedCandidatesNode(digit, otherCells);
						//			AddNode(nextNode, ref list);
						//			AddNode(node, ref list2);
						//			AssignOrUpdateHashSet(list2, nextNode, _strongInferences);
						//		}
						//	}
						//}

						foreach (byte theOtherDigit in (short)(digitsMask & ~(1 << digit)))
						{
							var theOtherCells = grid.CandidatesMap[theOtherDigit] & alsMap;
							var nextNode = new AlmostLockedSetNode(theOtherDigit, theOtherCells, alsMap - theOtherCells);
							AddNode(nextNode, ref list);
						}
					}

					AssignOrUpdateHashSet(list, node, _strongInferences);
				}

				void getWeak(in Grid grid)
				{
					HashSet<int>? list = null;
					var triplets = (stackalloc Cells[3]);

					if (NodeTypes.Flags(SearcherNodeTypes.LockedSet))
					{
						foreach (int region in digitCellsUsed.CoveredRegions)
						{
							var otherCells = grid.CandidatesMap[digit] & RegionMaps[region] - digitCellsUsed;
							if (region.ToRegion() == Region.Block)
							{
								foreach (int subRegion in otherCells.RowMask << 9 | otherCells.ColumnMask << 18)
								{
									var intersectionMap = RegionMaps[region] & RegionMaps[subRegion];
									var subOtherCells = otherCells & intersectionMap;
									switch (subOtherCells)
									{
										case [var cell] when NodeTypes.Flags(SearcherNodeTypes.SoleDigit):
										{
											var nextNode = new SoleCandidateNode((byte)cell, digit);
											AddNode(nextNode, ref list);

											break;
										}
										case { Count: var count }:
										{
											for (int i = 0; i < count; i++)
											{
												foreach (var cellsCombination in subOtherCells & i)
												{
													if (cellsCombination is [var onlyCell])
													{
														if (NodeTypes.Flags(SearcherNodeTypes.SoleDigit))
														{
															var nextNode = new SoleCandidateNode((byte)onlyCell, digit);
															AddNode(nextNode, ref list);
														}
													}
													else
													{
														if (NodeTypes.Flags(SearcherNodeTypes.LockedCandidates))
														{
															var nextNode = new LockedCandidatesNode(digit, cellsCombination);
															AddNode(nextNode, ref list);
														}
													}
												}
											}

											break;
										}
									}
								}
							}
							else
							{
								triplets.Clear();
								triplets[0] = RegionMaps[region][0..3];
								triplets[1] = RegionMaps[region][3..6];
								triplets[2] = RegionMaps[region][6..9];

								foreach (ref readonly var triplet in triplets)
								{
									var subOtherCells = triplet & otherCells;
									if (subOtherCells is not { Count: var count and not 0 })
									{
										continue;
									}

									for (int i = 0; i < count; i++)
									{
										foreach (var cellsCombination in subOtherCells & i)
										{
											if (cellsCombination is [var onlyCell])
											{
												if (NodeTypes.Flags(SearcherNodeTypes.SoleDigit))
												{
													var nextNode = new SoleCandidateNode((byte)onlyCell, digit);
													AddNode(nextNode, ref list);
												}
											}
											else
											{
												if (NodeTypes.Flags(SearcherNodeTypes.LockedCandidates))
												{
													var nextNode = new LockedCandidatesNode(digit, cellsCombination);
													AddNode(nextNode, ref list);
												}
											}
										}
									}
								}
							}
						}
					}

					AssignOrUpdateHashSet(list, node, _weakInferences);
				}
			}
		}
	}
}
