using System;
using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Models;
using Sudoku.Solving.Manual.Extensions;
using Sudoku.Techniques;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	partial class UrStepSearcher
	{
		/// <summary>
		/// Check UR-XY-Wing, UR-XYZ-Wing, UR-WXYZ-Wing and AR-XY-Wing, AR-XYZ-Wing and AR-WXYZ-Wing.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="corner1">The corner cell 1.</param>
		/// <param name="corner2">The corner cell 2.</param>
		/// <param name="otherCellsMap">
		/// The map of other cells during the current UR searching.
		/// </param>
		/// <param name="size">The size of the wing to search.</param>
		/// <param name="index">The index.</param>
		partial void CheckWing(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap,
			int size, int index)
		{
			// Subtype 1:
			//     ↓ corner1
			//   (ab )  abxy  yz  xz
			//   (ab )  abxy  *
			//     ↑ corner2
			// Note that 'abxy' cells should be in the same region.
			//
			// Subtype 2:
			//     ↓ corner1
			//   (ab )  abx   xz
			//    aby  (ab )  *   yz
			//           ↑ corner2
			if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
			{
				return;
			}

			if (new Cells { corner1, corner2 }.AllSetsAreInOneRegion(out int region) && region < 9)
			{
				#region Subtype 1
				// Subtype 1.
				int[] offsets = otherCellsMap.ToArray();
				int otherCell1 = offsets[0], otherCell2 = offsets[1];
				short mask1 = grid.GetCandidates(otherCell1);
				short mask2 = grid.GetCandidates(otherCell2);
				short mask = (short)(mask1 | mask2);

				if (PopCount((uint)mask) != 2 + size || (mask & comparer) != comparer
					|| mask1 == comparer || mask2 == comparer)
				{
					return;
				}

				var map = (PeerMaps[otherCell1] | PeerMaps[otherCell2]) & BivalueMap;
				if (map.Count < size)
				{
					return;
				}

				var testMap = new Cells { otherCell1, otherCell2 }.PeerIntersection;
				short extraDigitsMask = (short)(mask ^ comparer);
				int[] cells = map.ToArray();
				for (int i1 = 0, length = cells.Length; i1 < length - size + 1; i1++)
				{
					int c1 = cells[i1];
					short m1 = grid.GetCandidates(c1);
					if ((m1 & ~extraDigitsMask) == 0)
					{
						continue;
					}

					for (int i2 = i1 + 1; i2 < length - size + 2; i2++)
					{
						int c2 = cells[i2];
						short m2 = grid.GetCandidates(c2);
						if ((m2 & ~extraDigitsMask) == 0)
						{
							continue;
						}

						if (size == 2)
						{
							// Check XY-Wing.
							short m = (short)((short)(m1 | m2) ^ extraDigitsMask);
							if ((PopCount((uint)m), PopCount((uint)(m1 & m2))) != (1, 1))
							{
								continue;
							}

							// Now check whether all cells found should see their corresponding
							// cells in UR structure ('otherCells1' or 'otherCells2').
							bool flag = true;
							foreach (int cell in stackalloc[] { c1, c2 })
							{
								int extraDigit = TrailingZeroCount(grid.GetCandidates(cell) & ~m);
								if (!(testMap & CandMaps[extraDigit]).Contains(cell))
								{
									flag = false;
									break;
								}
							}
							if (!flag)
							{
								continue;
							}

							// Now check eliminations.
							var conclusions = new List<Conclusion>();
							int elimDigit = TrailingZeroCount(m);
							var elimMap = new Cells { c1, c2 }.PeerIntersection & CandMaps[elimDigit];
							if (elimMap.IsEmpty)
							{
								continue;
							}

							foreach (int cell in elimMap)
							{
								conclusions.Add(new(ConclusionType.Elimination, cell, elimDigit));
							}

							var candidateOffsets = new List<DrawingInfo>();
							foreach (int cell in urCells)
							{
								if (grid.GetStatus(cell) == CellStatus.Empty)
								{
									foreach (int digit in grid.GetCandidates(cell))
									{
										candidateOffsets.Add(
											new(
												digit == elimDigit
												? otherCellsMap.Contains(cell) ? 2 : 0
												: (extraDigitsMask >> digit & 1) != 0 ? 1 : 0,
												cell * 9 + digit));
									}
								}
							}
							foreach (int digit in grid.GetCandidates(c1))
							{
								candidateOffsets.Add(new(digit == elimDigit ? 2 : 1, c1 * 9 + digit));
							}
							foreach (int digit in grid.GetCandidates(c2))
							{
								candidateOffsets.Add(new(digit == elimDigit ? 2 : 1, c2 * 9 + digit));
							}
							if (IsIncompleteUr(candidateOffsets))
							{
								return;
							}

							accumulator.Add(
								new UrWithWingStepInfo(
									conclusions,
									new View[]
									{
										new()
										{
											Cells = arMode ? GetHighlightCells(urCells) : null,
											Candidates = candidateOffsets
										}
									},
									arMode ? Technique.ArXyWing : Technique.UrXyWing,
									d1,
									d2,
									urCells,
									arMode,
									new[] { c1, c2 },
									extraDigitsMask.GetAllSets().ToArray(),
									otherCellsMap,
									index));
						}
						else // size > 2
						{
							for (int i3 = i2 + 1; i3 < length - size + 3; i3++)
							{
								int c3 = cells[i3];
								short m3 = grid.GetCandidates(c3);
								if ((m3 & ~extraDigitsMask) == 0)
								{
									continue;
								}

								if (size == 3)
								{
									// Check XYZ-Wing.
									short m = (short)(((short)(m1 | m2) | m3) ^ extraDigitsMask);
									if ((PopCount((uint)m), PopCount((uint)(m1 & m2 & m3))) != (1, 1))
									{
										continue;
									}

									// Now check whether all cells found should see their corresponding
									// cells in UR structure ('otherCells1' or 'otherCells2').
									bool flag = true;
									foreach (int cell in stackalloc[] { c1, c2, c3 })
									{
										int extraDigit = TrailingZeroCount(grid.GetCandidates(cell) & ~m);
										if (!(testMap & CandMaps[extraDigit]).Contains(cell))
										{
											flag = false;
											break;
										}
									}
									if (!flag)
									{
										continue;
									}

									// Now check eliminations.
									var conclusions = new List<Conclusion>();
									int elimDigit = TrailingZeroCount(m);
									var elimMap = new Cells { c1, c2, c3 }.PeerIntersection & CandMaps[elimDigit];
									if (elimMap.IsEmpty)
									{
										continue;
									}

									foreach (int cell in elimMap)
									{
										conclusions.Add(new(ConclusionType.Elimination, cell, elimDigit));
									}

									var candidateOffsets = new List<DrawingInfo>();
									foreach (int cell in urCells)
									{
										if (grid.GetStatus(cell) == CellStatus.Empty)
										{
											foreach (int digit in grid.GetCandidates(cell))
											{
												candidateOffsets.Add(
													new((extraDigitsMask >> digit & 1) != 0 ? 1 : 0, cell * 9 + digit));
											}
										}
									}
									foreach (int digit in grid.GetCandidates(c1))
									{
										candidateOffsets.Add(new(digit == elimDigit ? 2 : 1, c1 * 9 + digit));
									}
									foreach (int digit in grid.GetCandidates(c2))
									{
										candidateOffsets.Add(new(digit == elimDigit ? 2 : 1, c2 * 9 + digit));
									}
									foreach (int digit in grid.GetCandidates(c3))
									{
										candidateOffsets.Add(new(digit == elimDigit ? 2 : 1, c3 * 9 + digit));
									}
									if (IsIncompleteUr(candidateOffsets))
									{
										return;
									}

									accumulator.Add(
										new UrWithWingStepInfo(
											conclusions,
											new View[]
											{
												new()
												{
													Cells = arMode ? GetHighlightCells(urCells) : null,
													Candidates = candidateOffsets
												}
											},
											arMode ? Technique.ArXyzWing : Technique.UrXyzWing,
											d1,
											d2,
											urCells,
											arMode,
											new[] { c1, c2, c3 },
											extraDigitsMask.GetAllSets().ToArray(),
											otherCellsMap,
											index));
								}
								else // size == 4
								{
									for (int i4 = i3 + 1; i4 < length; i4++)
									{
										int c4 = cells[i4];
										short m4 = grid.GetCandidates(c4);
										if ((m4 & ~extraDigitsMask) == 0)
										{
											continue;
										}

										// Check WXYZ-Wing.
										short m = (short)((short)((short)((short)(m1 | m2) | m3) | m4) ^ extraDigitsMask);
										if ((PopCount((uint)m), PopCount((uint)(m1 & m2 & m3 & m4))) != (1, 1))
										{
											continue;
										}

										// Now check whether all cells found should see their corresponding
										// cells in UR structure ('otherCells1' or 'otherCells2').
										bool flag = true;
										foreach (int cell in stackalloc[] { c1, c2, c3, c4 })
										{
											int extraDigit = TrailingZeroCount(grid.GetCandidates(cell) & ~m);
											if (!(testMap & CandMaps[extraDigit]).Contains(cell))
											{
												flag = false;
												break;
											}
										}
										if (!flag)
										{
											continue;
										}

										// Now check eliminations.
										var conclusions = new List<Conclusion>();
										int elimDigit = TrailingZeroCount(m);
										var elimMap =
											new Cells { c1, c2, c3, c4 }.PeerIntersection & CandMaps[elimDigit];
										if (elimMap.IsEmpty)
										{
											continue;
										}

										foreach (int cell in elimMap)
										{
											conclusions.Add(new(ConclusionType.Elimination, cell, elimDigit));
										}

										var candidateOffsets = new List<DrawingInfo>();
										foreach (int cell in urCells)
										{
											if (grid.GetStatus(cell) == CellStatus.Empty)
											{
												foreach (int digit in grid.GetCandidates(cell))
												{
													candidateOffsets.Add(
														new(
															(extraDigitsMask >> digit & 1) != 0 ? 1 : 0,
															cell * 9 + digit));
												}
											}
										}
										foreach (int digit in grid.GetCandidates(c1))
										{
											candidateOffsets.Add(new(digit == elimDigit ? 2 : 1, c1 * 9 + digit));
										}
										foreach (int digit in grid.GetCandidates(c2))
										{
											candidateOffsets.Add(new(digit == elimDigit ? 2 : 1, c2 * 9 + digit));
										}
										foreach (int digit in grid.GetCandidates(c3))
										{
											candidateOffsets.Add(new(digit == elimDigit ? 2 : 1, c3 * 9 + digit));
										}
										foreach (int digit in grid.GetCandidates(c4))
										{
											candidateOffsets.Add(new(digit == elimDigit ? 2 : 1, c4 * 9 + digit));
										}
										if (IsIncompleteUr(candidateOffsets))
										{
											return;
										}

										accumulator.Add(
											new UrWithWingStepInfo(
												conclusions,
												new View[]
												{
													new()
													{
														Cells = arMode ? GetHighlightCells(urCells) : null,
														Candidates = candidateOffsets
													}
												},
												arMode ? Technique.ArWxyzWing : Technique.UrWxyzWing,
												d1,
												d2,
												urCells,
												arMode,
												new[] { c1, c2, c3, c4 },
												extraDigitsMask.GetAllSets().ToArray(),
												otherCellsMap,
												index));
									}
								}
							}
						}
					}
				}
				#endregion
			}
			else
			{
				#region Subtype 2
				// TODO: Finish processing Subtype 2.
				#endregion
			}
		}

		/// <summary>
		/// Check UR+SdC.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="corner1">The corner cell 1.</param>
		/// <param name="corner2">The corner cell 2.</param>
		/// <param name="otherCellsMap">
		/// The map of other cells during the current UR searching.
		/// </param>
		/// <param name="index">The index.</param>
		partial void CheckSdc(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer,
			int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index)
		{
			//           |   xyz
			//  ab+ ab+  | abxyz abxyz
			//           |   xyz
			// ----------+------------
			// (ab)(ab)  |
			//  ↑ corner1, corner2
			bool notSatisfiedType3 = false;
			short mergedMaskInOtherCells = 0;
			foreach (int cell in otherCellsMap)
			{
				short currentMask = grid.GetCandidates(cell);
				mergedMaskInOtherCells |= currentMask;
				if ((currentMask & comparer) == 0
					|| currentMask == comparer || arMode && grid.GetStatus(cell) != CellStatus.Empty)
				{
					notSatisfiedType3 = true;
					break;
				}
			}

			if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer || notSatisfiedType3
				|| (mergedMaskInOtherCells & comparer) != comparer)
			{
				return;
			}

			// Check whether the corners spanned two blocks. If so, UR+SdC can't be found.
			short blockMaskInOtherCells = otherCellsMap.BlockMask;
			if (blockMaskInOtherCells == 0 || (blockMaskInOtherCells & blockMaskInOtherCells - 1) != 0)
			{
				return;
			}

			short otherDigitsMask = (short)(mergedMaskInOtherCells & ~comparer);
			byte line = (byte)otherCellsMap.CoveredLine;
			byte block = (byte)TrailingZeroCount(otherCellsMap.CoveredRegions & ~(1 << line));
			var (a, _, _, d) = IntersectionMaps[(line, block)];
			var list = new List<Cells>(4);
			foreach (bool cannibalMode in stackalloc[] { false, true })
			{
				foreach (byte otherBlock in d)
				{
					var emptyCellsInInterMap = RegionMaps[otherBlock] & RegionMaps[line] & EmptyMap;
					if (emptyCellsInInterMap.Count < 2)
					{
						// The intersection needs at least two empty cells.
						continue;
					}

					Cells b = RegionMaps[otherBlock] - RegionMaps[line], c = a & b;

					list.Clear();
					int[] offsets = emptyCellsInInterMap.ToArray();
					switch (emptyCellsInInterMap.Count)
					{
						case 2:
						{
							list.Add(new() { offsets[0], offsets[1] });

							break;
						}
						case 3:
						{
							int i = offsets[0], j = offsets[1], k = offsets[2];
							list.Add(new() { i, j });
							list.Add(new() { j, k });
							list.Add(new() { i, k });
							list.Add(new() { i, j, k });

							break;
						}
					}

					// Iterate on each intersection combination.
					foreach (var currentInterMap in list)
					{
						short selectedInterMask = 0;
						foreach (int cell in currentInterMap)
						{
							selectedInterMask |= grid.GetCandidates(cell);
						}
						if (PopCount((uint)selectedInterMask) <= currentInterMap.Count + 1)
						{
							// The intersection combination is an ALS or a normal subset,
							// which is invalid in SdCs.
							continue;
						}

						var blockMap = (b | c - currentInterMap) & EmptyMap;
						var lineMap = (a & EmptyMap) - otherCellsMap;

						// Iterate on the number of the cells that should be selected in block.
						for (int i = 1; i <= blockMap.Count - 1; i++)
						{
							// Iterate on each combination in block.
							foreach (int[] selectedCellsInBlock in blockMap.ToArray().GetSubsets(i))
							{
								bool flag = false;
								foreach (int digit in otherDigitsMask)
								{
									foreach (int cell in selectedCellsInBlock)
									{
										if (grid.Exists(cell, digit) is true)
										{
											flag = true;
											break;
										}
									}
								}
								if (flag)
								{
									continue;
								}

								var currentBlockMap = new Cells(selectedCellsInBlock);
								Cells elimMapBlock = Cells.Empty, elimMapLine = Cells.Empty;

								// Get the links of the block.
								short blockMask = 0;
								foreach (int cell in selectedCellsInBlock)
								{
									blockMask |= grid.GetCandidates(cell);
								}

								// Get the elimination map in the block.
								foreach (int digit in blockMask)
								{
									elimMapBlock |= CandMaps[digit];
								}
								elimMapBlock &= blockMap - currentBlockMap;

								foreach (int digit in otherDigitsMask)
								{
									elimMapLine |= CandMaps[digit];
								}
								elimMapLine &= lineMap - currentInterMap;

								checkGeneralizedSdc(
									accumulator, grid, arMode, cannibalMode, d1, d2, urCells,
									line, otherBlock, otherDigitsMask, blockMask, selectedInterMask,
									otherDigitsMask, elimMapLine, elimMapBlock, otherCellsMap, currentBlockMap,
									currentInterMap, i, 0, index);
							}
						}
					}
				}
			}

			static void checkGeneralizedSdc(
				IList<UrStepInfo> accumulator, in SudokuGrid grid, bool arMode, bool cannibalMode, int digit1,
				int digit2, int[] urCells, int line, int block, short lineMask, short blockMask,
				short selectedInterMask, short otherDigitsMask, in Cells elimMapLine, in Cells elimMapBlock,
				in Cells currentLineMap, in Cells currentBlockMap, in Cells currentInterMap, int i, int j,
				int index)
			{
				short maskOnlyInInter = (short)(selectedInterMask & ~(blockMask | lineMask));
				short maskIsolated = (short)(
					cannibalMode ? (lineMask & blockMask & selectedInterMask) : maskOnlyInInter
				);
				if (
					!cannibalMode && (
						(blockMask & lineMask) != 0
						|| maskIsolated != 0 && (maskIsolated == 0 || (maskIsolated & maskIsolated - 1) != 0)
					) || cannibalMode && (maskIsolated == 0 || (maskIsolated & maskIsolated - 1) != 0)
				)
				{
					return;
				}

				var elimMapIsolated = Cells.Empty;
				int digitIsolated = TrailingZeroCount(maskIsolated);
				if (digitIsolated != Constants.InvalidFirstSet)
				{
					elimMapIsolated =
						(cannibalMode ? currentBlockMap | currentLineMap : currentInterMap)
						% CandMaps[digitIsolated] & EmptyMap;
				}

				if (currentInterMap.Count + i + j + 1 ==
					PopCount((uint)blockMask) + PopCount((uint)lineMask) + PopCount((uint)maskOnlyInInter)
					&& (!elimMapBlock.IsEmpty || !elimMapLine.IsEmpty || !elimMapIsolated.IsEmpty))
				{
					// Check eliminations.
					var conclusions = new List<Conclusion>();
					foreach (int cell in elimMapBlock)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							if ((blockMask >> digit & 1) != 0)
							{
								conclusions.Add(new(ConclusionType.Elimination, cell, digit));
							}
						}
					}
					foreach (int cell in elimMapLine)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							if ((lineMask >> digit & 1) != 0)
							{
								conclusions.Add(new(ConclusionType.Elimination, cell, digit));
							}
						}
					}
					foreach (int cell in elimMapIsolated)
					{
						conclusions.Add(new(ConclusionType.Elimination, cell, digitIsolated));
					}
					if (conclusions.Count == 0)
					{
						return;
					}

					// Record highlight candidates and cells.
					var candidateOffsets = new List<DrawingInfo>();
					foreach (int cell in urCells)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(
								new((otherDigitsMask >> digit & 1) != 0 ? 1 : 0, cell * 9 + digit));
						}
					}
					foreach (int cell in currentBlockMap)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(
								new(!cannibalMode && digit == digitIsolated ? 3 : 2, cell * 9 + digit));
						}
					}
					foreach (int cell in currentInterMap)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(
								new(
									digitIsolated == digit ? 3 : (otherDigitsMask >> digit & 1) != 0 ? 1 : 2,
									cell * 9 + digit));
						}
					}

					accumulator.Add(
						new UrWithSdcStepInfo(
							conclusions,
							new View[]
							{
								new()
								{
									Cells = arMode ? GetHighlightCells(urCells) : null,
									Candidates = candidateOffsets,
									Regions = new DrawingInfo[] { new(0, block), new(2, line) }
								}
							},
							digit1,
							digit2,
							urCells,
							arMode,
							index,
							block,
							line,
							blockMask,
							lineMask,
							selectedInterMask,
							cannibalMode,
							maskIsolated,
							currentBlockMap,
							currentLineMap,
							currentInterMap));
				}
			}
		}

		/// <summary>
		/// Check UR+Unknown covering.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="comparer">The comparer.</param>
		/// <param name="d1">The digit 1.</param>
		/// <param name="d2">The digit 2.</param>
		/// <param name="index">The index.</param>
		partial void CheckUnknownCoveringUnique(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, short comparer,
			int d1, int d2, int index)
		{
			//      ↓urCellInSameBlock
			// ab  abc      abc  ←anotherCell
			//
			//     abcx-----abcy ←resultCell
			//           c
			//      ↑targetCell
			// Where the digit 'a' and 'b' in the down-left cell 'abcx' can be removed.

			var cells = new Cells(urCells);

			// Check all cells are ampty.
			bool containsValueCells = false;
			foreach (int cell in cells)
			{
				if (grid.GetStatus(cell) != CellStatus.Empty)
				{
					containsValueCells = true;
					break;
				}
			}
			if (containsValueCells)
			{
				return;
			}

			// Iterate on each cell.
			foreach (int targetCell in cells)
			{
				int block = targetCell.ToRegion(RegionLabel.Block);
				var bivalueCellsToCheck = (PeerMaps[targetCell] & RegionMaps[block] & BivalueMap) - cells;
				if (bivalueCellsToCheck.IsEmpty)
				{
					continue;
				}

				// Check all bivalue cells.
				foreach (int bivalueCellToCheck in bivalueCellsToCheck)
				{
					if (new Cells { bivalueCellToCheck, targetCell }.CoveredLine != Constants.InvalidFirstSet)
					{
						// 'targetCell' and 'bivalueCellToCheck' can't lie on a same line.
						continue;
					}

					if (grid.GetCandidates(bivalueCellToCheck) != comparer)
					{
						// 'bivalueCell' must contain both 'd1' and 'd2'.
						continue;
					}

					int urCellInSameBlock = new Cells(RegionMaps[block] & cells) { ~targetCell }[0];
					int coveredLine = new Cells { bivalueCellToCheck, urCellInSameBlock }.CoveredLine;
					if (coveredLine == Constants.InvalidFirstSet)
					{
						// The bi-value cell 'bivalueCellToCheck' should be lie on a same region
						// as 'urCellInSameBlock'.
						continue;
					}

					int anotherCell = (new Cells(cells) { ~urCellInSameBlock } & RegionMaps[coveredLine])[0];
					foreach (int extraDigit in grid.GetCandidates(targetCell) & ~comparer)
					{
						short abcMask = (short)(comparer | (short)(1 << extraDigit));

						if (grid.GetCandidates(anotherCell) != abcMask)
						{
							continue;
						}

						// Check the conjugate pair of the extra digit.
						int resultCell = new Cells(cells) { ~urCellInSameBlock, ~anotherCell, ~targetCell }[0];
						var map = new Cells { targetCell, resultCell };
						int line = map.CoveredLine;
						if (!IsConjugatePair(extraDigit, map, line))
						{
							continue;
						}

						#region Subtype 1
						if (grid.GetCandidates(urCellInSameBlock) != abcMask)
						{
							goto SubType2;
						}

						// Here, is the basic sub-type having passed the checking.
						// Gather conclusions.
						var conclusions = new List<Conclusion>();
						foreach (int digit in grid.GetCandidates(targetCell))
						{
							if (digit == d1 || digit == d2)
							{
								conclusions.Add(new(ConclusionType.Elimination, targetCell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							goto SubType2;
						}

						// Gather views.
						var candidateOffsets = new List<DrawingInfo> { new(1, targetCell * 9 + extraDigit) };
						if (grid.Exists(resultCell, d1) is true)
						{
							candidateOffsets.Add(new(0, resultCell * 9 + d1));
						}
						if (grid.Exists(resultCell, d2) is true)
						{
							candidateOffsets.Add(new(0, resultCell * 9 + d2));
						}
						if (grid.Exists(resultCell, extraDigit) is true)
						{
							candidateOffsets.Add(new(1, resultCell * 9 + extraDigit));
						}

						foreach (int digit in grid.GetCandidates(urCellInSameBlock) & abcMask)
						{
							candidateOffsets.Add(new(0, urCellInSameBlock * 9 + digit));
						}
						foreach (int digit in grid.GetCandidates(anotherCell))
						{
							candidateOffsets.Add(new(0, anotherCell * 9 + digit));
						}
						foreach (int digit in grid.GetCandidates(bivalueCellToCheck))
						{
							candidateOffsets.Add(new(2, bivalueCellToCheck * 9 + digit));
						}

						// Add into the list.
						accumulator.Add(
							new UrWithUnknownCoveringStepInfo(
								conclusions,
								new View[]
								{
									new()
									{
										Cells = new DrawingInfo[] { new(0, targetCell) },
										Candidates = candidateOffsets,
										Regions = new DrawingInfo[] { new(0, block), new(1, line) }
									},
									new()
									{
										Candidates = new DrawingInfo[]
										{
											new(1, resultCell * 9 + extraDigit),
											new(1, targetCell * 9 + extraDigit)
										},
										StepFilling = new (int, char)[]
										{
											(bivalueCellToCheck, 'y'),
											(targetCell, 'x'),
											(urCellInSameBlock, (char)(extraDigit + '1')),
											(anotherCell, 'x'),
											(resultCell, (char)(extraDigit + '1'))
										}
									}
								},
								d1,
								d2,
								targetCell,
								extraDigit,
								urCells,
								index));
					#endregion

#pragma warning disable IDE0055
						#region Subtype 2
#pragma warning restore IDE0055
					SubType2:
						// The extra digit should form a conjugate pair in that line.
						var anotherMap = new Cells { urCellInSameBlock, anotherCell };
						int anotherLine = anotherMap.CoveredLine;
						if (!IsConjugatePair(extraDigit, anotherMap, anotherLine))
						{
							continue;
						}

						// Gather conclusions.
						var conclusionsAnotherSubType = new List<Conclusion>();
						foreach (int digit in grid.GetCandidates(targetCell))
						{
							if (digit == d1 || digit == d2)
							{
								conclusionsAnotherSubType.Add(
									new(ConclusionType.Elimination, targetCell, digit));
							}
						}
						if (conclusionsAnotherSubType.Count == 0)
						{
							continue;
						}

						// Gather views.
						var candidateOffsetsAnotherSubtype = new List<DrawingInfo>
						{
							new(1, targetCell * 9 + extraDigit)
						};
						if (grid.Exists(resultCell, d1) is true)
						{
							candidateOffsetsAnotherSubtype.Add(new(0, resultCell * 9 + d1));
						}
						if (grid.Exists(resultCell, d2) is true)
						{
							candidateOffsetsAnotherSubtype.Add(new(0, resultCell * 9 + d2));
						}
						if (grid.Exists(resultCell, extraDigit) is true)
						{
							candidateOffsetsAnotherSubtype.Add(new(1, resultCell * 9 + extraDigit));
						}

						var candidateOffsetsAnotherSubtypeLighter = new List<DrawingInfo>
						{
							new(1, resultCell * 9 + extraDigit),
							new(1, targetCell * 9 + extraDigit)
						};
						foreach (int digit in grid.GetCandidates(urCellInSameBlock) & abcMask)
						{
							if (digit == extraDigit)
							{
								candidateOffsetsAnotherSubtype.Add(new(1, urCellInSameBlock * 9 + digit));
								candidateOffsetsAnotherSubtypeLighter.Add(new(1, urCellInSameBlock * 9 + digit));
							}
							else
							{
								candidateOffsetsAnotherSubtype.Add(new(0, urCellInSameBlock * 9 + digit));
							}
						}
						foreach (int digit in grid.GetCandidates(anotherCell))
						{
							if (digit == extraDigit)
							{
								candidateOffsetsAnotherSubtype.Add(new(1, anotherCell * 9 + digit));
								candidateOffsetsAnotherSubtypeLighter.Add(new(1, anotherCell * 9 + digit));
							}
							else
							{
								candidateOffsetsAnotherSubtype.Add(new(0, anotherCell * 9 + digit));
							}
						}
						foreach (int digit in grid.GetCandidates(bivalueCellToCheck))
						{
							candidateOffsetsAnotherSubtype.Add(new(2, bivalueCellToCheck * 9 + digit));
						}

						// Add into the list.
						accumulator.Add(
							new UrWithUnknownCoveringStepInfo(
								conclusionsAnotherSubType,
								new View[]
								{
									new()
									{
										Cells = new DrawingInfo[] { new(0, targetCell) },
										Candidates = candidateOffsetsAnotherSubtype,
										Regions = new DrawingInfo[]
										{
											new(0, block),
											new(1, line),
											new(1, anotherLine)
										}
									},
									new()
									{
										Candidates = candidateOffsetsAnotherSubtypeLighter,
										StepFilling = new (int, char)[]
										{
											(bivalueCellToCheck, 'y'),
											(targetCell, 'x'),
											(urCellInSameBlock, (char)(extraDigit + '1')),
											(anotherCell, 'x'),
											(resultCell, (char)(extraDigit + '1'))
										}
									}
								},
								d1,
								d2,
								targetCell,
								extraDigit,
								urCells,
								index));
						#endregion
					}
				}
			}
		}

		/// <summary>
		/// Check UR+Guardian.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="comparer">The mask comparer.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="index">The index.</param>
		partial void CheckGuardianUnique(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, short comparer,
			int d1, int d2, int index)
		{
			var cells = new Cells(urCells);

			if ((grid.GetCandidates(urCells[0]) & comparer) != comparer
				|| (grid.GetCandidates(urCells[1]) & comparer) != comparer
				|| (grid.GetCandidates(urCells[2]) & comparer) != comparer
				|| (grid.GetCandidates(urCells[3]) & comparer) != comparer)
			{
				// Guardian type shouldn't be incomplete.
				return;
			}

			// Iterate on two regions used.
			foreach (int[] regionCombination in cells.Regions.GetAllSets().GetSubsets(2))
			{
				var regionCells = RegionMaps[regionCombination[0]] | RegionMaps[regionCombination[1]];
				if ((regionCells & cells) != cells)
				{
					// The regions must contain all 4 UR cells.
					continue;
				}

				var guardian1 = regionCells - cells & CandMaps[d1];
				var guardian2 = regionCells - cells & CandMaps[d2];
				if (!(guardian1.IsEmpty ^ guardian2.IsEmpty))
				{
					// Only one digit can contain guardians.
					continue;
				}

				int guardianDigit = -1;
				Cells? targetElimMap = null, targetGuardianMap = null;
				if (!guardian1.IsEmpty && (guardian1.PeerIntersection & CandMaps[d1]) is { IsEmpty: false } a)
				{
					targetElimMap = a;
					guardianDigit = d1;
					targetGuardianMap = guardian1;
				}
				else if (!guardian2.IsEmpty && (guardian2.PeerIntersection & CandMaps[d2]) is { IsEmpty: false } b)
				{
					targetElimMap = b;
					guardianDigit = d2;
					targetGuardianMap = guardian2;
				}

				if (targetElimMap is not { } elimMap || targetGuardianMap is not { } guardianMap
					|| guardianDigit == -1)
				{
					continue;
				}

				var conclusions = new List<Conclusion>();
				foreach (int cell in elimMap)
				{
					conclusions.Add(new(ConclusionType.Elimination, cell, guardianDigit));
				}

				var candidateOffsets = new List<DrawingInfo>();
				foreach (int cell in urCells)
				{
					candidateOffsets.Add(new(0, cell * 9 + d1));
					candidateOffsets.Add(new(0, cell * 9 + d2));
				}
				foreach (int cell in guardianMap)
				{
					candidateOffsets.Add(new(1, cell * 9 + guardianDigit));
				}

				accumulator.Add(
					new UrWithGuardianStepInfo(
						conclusions,
						new View[]
						{
							new()
							{
								Candidates = candidateOffsets,
								Regions = new DrawingInfo[]
								{
									new(0, regionCombination[0]),
									new(0, regionCombination[1])
								}
							}
						},
						d1,
						d2,
						urCells,
						guardianDigit,
						guardianMap,
						index));
			}
		}

		/// <summary>
		/// Check AR+Hidden single.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="urCells">All UR cells.</param>
		/// <param name="d1">The digit 1 used in UR.</param>
		/// <param name="d2">The digit 2 used in UR.</param>
		/// <param name="corner1">The corner cell 1.</param>
		/// <param name="corner2">The corner cell 2.</param>
		/// <param name="otherCellsMap">
		/// The map of other cells during the current UR searching.
		/// </param>
		/// <param name="index">The index.</param>
		partial void CheckHiddenSingleAvoidable(
			IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, int d1, int d2,
			int corner1, int corner2, in Cells otherCellsMap, int index)
		{
			// ↓corner1
			// a   | aby  -  -
			// abx | a    -  b
			//     | -    -  -
			//       ↑corner2(cell 'a')
			// There's only one cell can be filled with the digit 'b' besides the cell 'aby'.

			if (grid.GetStatus(corner1) != CellStatus.Modifiable
				|| grid.GetStatus(corner2) != CellStatus.Modifiable
				|| grid[corner1] != grid[corner2]
				|| grid[corner1] != d1 && grid[corner1] != d2)
			{
				return;
			}

			// Get the base digit ('a') and the other digit ('b').
			// Here 'b' is the digit that we should check the possible hidden single.
			int baseDigit = grid[corner1], otherDigit = baseDigit == d1 ? d2 : d1;
			var cellsThatTwoOtherCellsBothCanSee = otherCellsMap.PeerIntersection & CandMaps[otherDigit];

			// Iterate on two cases (because we holds two other cells,
			// and both those two cells may contain possible elimination).
			for (int i = 0; i < 2; i++)
			{
				var (baseCell, anotherCell) = i == 0
					? (otherCellsMap[0], otherCellsMap[1])
					: (otherCellsMap[1], otherCellsMap[0]);

				// Iterate on each region type.
				for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
				{
					int region = baseCell.ToRegion(label);

					// If the region doesn't overlap with the specified region, just skip it.
					if ((cellsThatTwoOtherCellsBothCanSee & RegionMaps[region]).IsEmpty)
					{
						continue;
					}

					var otherCellsToCheck = RegionMaps[region] & CandMaps[otherDigit] & PeerMaps[anotherCell];
					int sameRegions = new Cells(otherCellsToCheck) { anotherCell }.CoveredRegions;
					foreach (int sameRegion in sameRegions)
					{
						// Check whether all possible positions of the digit 'b' in this region only
						// lies in the given cells above ('cellsThatTwoOtherCellsBothCanSee').
						if (
							(new Cells(RegionMaps[sameRegion]) { ~anotherCell } & CandMaps[otherDigit])
							!= otherCellsToCheck
						)
						{
							continue;
						}

						// Possible hidden single found.
						// If the elimination doesn't exist, just skip it.
						if (grid.Exists(baseCell, otherDigit) is not true)
						{
							continue;
						}

						var cellOffsets = new List<DrawingInfo>();
						foreach (int cell in urCells)
						{
							cellOffsets.Add(new(0, cell));
						}

						var candidateOffsets = new List<DrawingInfo> { new(0, anotherCell * 9 + otherDigit) };
						foreach (int cell in otherCellsToCheck)
						{
							candidateOffsets.Add(new(1, cell * 9 + otherDigit));
						}

						accumulator.Add(
							new ArWithHiddenSingleStepInfo(
								new Conclusion[] { new(ConclusionType.Elimination, baseCell, otherDigit) },
								new View[]
								{
									new()
									{
										Cells = cellOffsets,
										Candidates = candidateOffsets,
										Regions = new DrawingInfo[] { new(0, sameRegion) }
									}
								},
								d1,
								d2,
								urCells,
								baseCell,
								anotherCell,
								sameRegion,
								index));
					}
				}
			}
		}
	}
}
