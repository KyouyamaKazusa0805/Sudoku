using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Extensions;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Solving.Manual.Exocets.Eliminations;
using Sudoku.Windows;
using static Sudoku.Constants.Processings;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Encapsulates a <b>senior exocet</b> (SE) technique searcher.
	/// </summary>
	[Obsolete("Please use '" + nameof(SeStepSearcher) + "' instead.", true)]
	[DisableDisplaying]
	public sealed class SeniorExocetStepSearcher : ExocetStepSearcher
	{
		/// <inheritdoc/>
		public SeniorExocetStepSearcher(bool checkAdvanced) : base(checkAdvanced)
		{
		}


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(96, nameof(TechniqueCode.Se))
		{
			IsEnabled = false,
			DisabledReason = DisabledReason.HasBugs
		};


		/// <inheritdoc/>
		[SkipLocalsInit]
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			unsafe
			{
				int* compatibleCells = stackalloc int[4], cover = stackalloc int[8];
				foreach (var exocet in Patterns)
				{
					var (baseMap, targetMap, _) = exocet;
					var (b1, b2, tq1, tq2, tr1, tr2, s, mq1, mq2, mr1, mr2) = exocet;
					if (grid.GetCandidates(b1).PopCount() < 2 || grid.GetCandidates(b2).PopCount() < 2)
					{
						continue;
					}

					bool isRow = baseMap.CoveredLine < 18;
					var tempCrosslineMap = s | targetMap;
					short baseCandidatesMask = (short)(grid.GetCandidates(b1) | grid.GetCandidates(b2));

					int i = 0;
					int r = RegionLabel.Row.ToRegion(b1) - 9, c = RegionLabel.Column.ToRegion(b1) - 18;
					foreach (int pos in SudokuGrid.MaxCandidatesMask & ~(1 << (isRow ? r : c)))
					{
						cover[i++] = isRow ? pos + 9 : pos + 18;
					}

					i = 0;
					Cells temp;
					foreach (int digit in baseCandidatesMask)
					{
						if (i++ == 0)
						{
							*&temp = DigitMaps[digit];
						}
						else
						{
							*&temp |= DigitMaps[digit];
						}
					}
					*&temp &= tempCrosslineMap;

					var tempTarget = new List<int>();
					for (i = 0; i < 8; i++)
					{
						var check = temp & RegionMaps[cover[i]];
						if (check.Count != 1)
						{
							continue;
						}

						tempTarget.Add(check[0]);
					}
					if (tempTarget.Count == 0)
					{
						continue;
					}
					int bOrT = isRow ? b1 / 9 / 3 : b1 % 9 / 3; // Base or target (B or T).
					foreach (int[] comb in tempTarget.GetSubsets(2))
					{
						[MethodImpl(MethodImplOptions.AggressiveInlining)] static int a(int v) => v / 9 / 3;
						[MethodImpl(MethodImplOptions.AggressiveInlining)] static int b(int v) => v % 9 / 3;

						int v1 = comb[0], v2 = comb[1];
						if (isRow ? a(v1) == bOrT && a(v2) == bOrT : b(v1) == bOrT && b(v2) == bOrT)
						{
							continue;
						}

						int row1 = RegionLabel.Row.ToRegion(v1), column1 = RegionLabel.Column.ToRegion(v1);
						int row2 = RegionLabel.Row.ToRegion(v2), column2 = RegionLabel.Column.ToRegion(v2);
						if (isRow ? column1 == column2 : row1 == row2)
						{
							continue;
						}

						short elimDigits = (short)((
							grid.GetCandidates(v1) | grid.GetCandidates(v2)
						) & ~baseCandidatesMask);
						if (
							!CheckCrossline(
								baseMap, tempCrosslineMap, baseCandidatesMask,
								v1, v2, isRow, out int[]? extraRegionsMask))
						{
							continue;
						}

						// Get all target eliminations.
						var targetElims = new Target();
						short cands = (short)(elimDigits & grid.GetCandidates(v1));
						if (cands != 0)
						{
							foreach (int digit in cands)
							{
								targetElims.Add(new(ConclusionType.Elimination, v1, digit));
							}
						}
						cands = (short)(elimDigits & grid.GetCandidates(v2));
						if (cands != 0)
						{
							foreach (int digit in cands)
							{
								targetElims.Add(new(ConclusionType.Elimination, v2, digit));
							}
						}

						short tbCands = 0;
						for (int j = 0; j < 2; j++)
						{
							if (grid.GetCandidates(comb[j]).PopCount() == 1)
							{
								tbCands |= grid.GetCandidates(comb[j]);
							}
						}

						// Get all true base eliminations.
						var trueBaseElims = new TrueBase();
						if (tbCands != 0
							&& (grid.GetStatus(v1) != CellStatus.Empty || grid.GetStatus(v2) != CellStatus.Empty))
						{
							for (int j = 0; j < 2; j++)
							{
								if (grid.GetStatus(comb[j]) != CellStatus.Empty)
								{
									continue;
								}

								if ((cands = (short)(grid.GetCandidates(comb[j]) & tbCands)) == 0)
								{
									continue;
								}

								foreach (int digit in cands)
								{
									trueBaseElims.Add(new(ConclusionType.Elimination, comb[j], digit));
								}
							}
						}

						if (tbCands != 0)
						{
							foreach (int digit in tbCands)
							{
								var elimMap = (baseMap & CandMaps[digit]).PeerIntersection & CandMaps[digit];
								if (elimMap.IsEmpty)
								{
									continue;
								}

								foreach (int cell in elimMap)
								{
									trueBaseElims.Add(new(ConclusionType.Elimination, cell, digit));
								}
							}
						}

						// Get mirror and compatibility test eliminations.
						var mirrorElims = new Mirror();
						var compatibilityElims = new CompatibilityTest();
						Cells mir;
						int target = -1;
						var cellOffsets = new List<DrawingInfo> { new(0, b1), new(0, b2) };
						foreach (int cell in tempCrosslineMap)
						{
							cellOffsets.Add(new(cell == v1 || cell == v2 ? 1 : 2, cell));
						}
						var candidateOffsets = new List<DrawingInfo>();
						if (CheckAdvanced)
						{
							for (int k = 0; k < 2; k++)
							{
								if (comb[k] == tq1 && !baseCandidatesMask.Overlaps(grid.GetCandidates(tr2)))
								{
									target = comb[k];
									mir = mq1;
								}
								if (comb[k] == tq2 && !baseCandidatesMask.Overlaps(grid.GetCandidates(tr1)))
								{
									target = comb[k];
									mir = mq2;
								}
								if (comb[k] == tr1 && !baseCandidatesMask.Overlaps(grid.GetCandidates(tq2)))
								{
									target = comb[k];
									mir = mr1;
								}
								if (comb[k] == tr2 && !baseCandidatesMask.Overlaps(grid.GetCandidates(tq1)))
								{
									target = comb[k];
									mir = mr2;
								}
							}

							if (target != -1)
							{
								var (tempTargetElims, tempMirrorElims) =
									CheckMirror(
										grid, target, comb[target == v1 ? 1 : 0], 0,
										baseCandidatesMask, *&mir, 0, -1, cellOffsets, candidateOffsets);

								targetElims = (Target)(targetElims | tempTargetElims);
								mirrorElims = (Mirror)(mirrorElims | tempMirrorElims);
							}

							short incompatible =
								CompatibilityTest(
									baseCandidatesMask, DigitMaps, tempCrosslineMap, baseMap, v1, v2);
							if (incompatible != 0)
							{
								compatibleCells[0] = b1;
								compatibleCells[1] = b2;
								compatibleCells[2] = v1;
								compatibleCells[3] = v2;

								for (int k = 0; k < 4; k++)
								{
									cands = (short)(incompatible & grid.GetCandidates(compatibleCells[k]));
									if (cands != 0)
									{
										foreach (int digit in cands)
										{
											compatibilityElims.Add(
												new(ConclusionType.Elimination, compatibleCells[k], digit));
										}
									}
								}
							}

							CompatibilityTest2(grid, compatibilityElims, baseMap, baseCandidatesMask, v1, v2);
						}

						if (mirrorElims.Count == 0 && compatibilityElims.Count == 0
							&& (
							CheckAdvanced && targetElims.Count == 0 && trueBaseElims.Count == 0
							|| !CheckAdvanced))
						{
							continue;
						}

						int endoTargetCell = comb[s.Contains(v1) ? 0 : 1];
						short m1 = grid.GetCandidates(b1), m2 = grid.GetCandidates(b2), m = (short)(m1 | m2);
						foreach (int digit in m1)
						{
							candidateOffsets.Add(new(0, b1 * 9 + digit));
						}
						foreach (int digit in m2)
						{
							candidateOffsets.Add(new(0, b2 * 9 + digit));
						}

						// Gather extra region cells (mutant exocets).
						var extraMap = Cells.Empty;
						for (int digit = 0; digit < 9; digit++)
						{
							foreach (int region in extraRegionsMask[digit])
							{
								foreach (int cell in RegionCells[region])
								{
									if (/*!tempCrosslineMap[cell] && */b1 != cell && b2 != cell
										&& !extraMap.Contains(cell))
									{
										extraMap.AddAnyway(cell);
										cellOffsets.Add(new(4, cell));
									}
									if (grid.Exists(cell, digit) is true)
									{
										candidateOffsets.Add(new(2, cell * 9 + digit));
									}
								}
							}
						}

						accumulator.Add(
							new SeStepInfo(
								new List<Conclusion>(), // Special eliminations will use this empty list.
								new View[] { new() { Cells = cellOffsets, Candidates = candidateOffsets } },
								exocet,
								m.GetAllSets().ToArray(),
								endoTargetCell,
								extraRegionsMask,
								targetElims,
								trueBaseElims,
								mirrorElims,
								compatibilityElims));
					}
				}
			}
		}

		/// <summary>
		/// Check the cross-line cells.
		/// </summary>
		/// <param name="baseMap">(<see langword="in"/> parameter) The base cells map.</param>
		/// <param name="tempCrossline">(<see langword="in"/> parameter) The cross-line map.</param>
		/// <param name="baseCandidatesMask">The base candidate mask.</param>
		/// <param name="t1">The target cell 1.</param>
		/// <param name="t2">The target cell 2.</param>
		/// <param name="isRow">Indicates whether the specified computation is for rows.</param>
		/// <param name="extraRegionsMask">
		/// (<see langword="out"/> parameter) The extra region to add
		/// (used for franken/mutant exocets). If normal, the value will be an array with 9 elements
		/// representing 9 different digits.
		/// </param>
		/// <returns>The <see cref="bool"/> result.</returns>
		private bool CheckCrossline(
			in Cells baseMap, in Cells tempCrossline, short baseCandidatesMask,
			int t1, int t2, bool isRow, [NotNullWhen(true)] out int[]? extraRegionsMask)
		{
			var xx = new Cells { t1, t2 };
			var tempMask = (stackalloc int[9]);
			foreach (int digit in baseCandidatesMask)
			{
				bool flag = true;
				var temp = (tempCrossline & DigitMaps[digit]) - xx;
				if ((isRow ? temp.RowMask : temp.ColumnMask).PopCount() > 2)
				{
					flag = false;
				}

				if (flag)
				{
					continue;
				}

				if (DeepCrosslineCheck(digit, (baseMap & DigitMaps[digit]).PeerIntersection, temp, ref tempMask))
				{
					continue;
				}

				if (!flag)
				{
					extraRegionsMask = null;
					return false;
				}
			}

			extraRegionsMask = tempMask.ToArray();
			return true;
		}

		/// <summary>
		/// Deeply check of cross-line cells (franken/mutant exocets can be searched here).
		/// </summary>
		/// <param name="digit">The digit.</param>
		/// <param name="baseElimMap">(<see langword="in"/> parameter) The base elimination map.</param>
		/// <param name="tempCrossline">(<see langword="in"/> parameter) The cross-line map.</param>
		/// <param name="extraRegionsMask">(<see langword="ref"/> parameter) The extra regions.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		[SkipLocalsInit]
		private bool DeepCrosslineCheck(
			int digit, in Cells baseElimMap, in Cells tempCrossline, ref Span<int> extraRegionsMask)
		{
			unsafe
			{
				int region, p;
				foreach (int[] combination in tempCrossline.ToArray().GetSubsets(3))
				{
					var (a, b, c) = (combination[0], combination[1], combination[2]);
					int r1 = RegionLabel.Row.ToRegion(a), c1 = RegionLabel.Column.ToRegion(a);
					int r2 = RegionLabel.Row.ToRegion(b), c2 = RegionLabel.Column.ToRegion(b);
					int r3 = RegionLabel.Row.ToRegion(c), c3 = RegionLabel.Column.ToRegion(c);
					if (r1 == r2 || r1 == r3 || r2 == r3 || c1 == c2 || c1 == c3 || c2 == c3)
					{
						continue;
					}

					bool flag = false;
					var check = DigitMaps[digit] - (PeerMaps[a] | PeerMaps[b] | PeerMaps[c] | baseElimMap);
					for (region = 9, p = 0; p < 27; region = (region + 1) % 27, p++)
					{
						if (!RegionMaps[region].Overlaps(check))
						{
							flag = true;
							break;
						}
					}

					if (!flag)
					{
						return false;
					}
				}

				extraRegionsMask[digit] |= 1 << *&region;
				return true;
			}
		}

		/// <summary>
		/// Compatibility test.
		/// </summary>
		/// <param name="baseCandidatesMask">The base candidates mask.</param>
		/// <param name="digitMaps">The digit distributions.</param>
		/// <param name="tempCrossline">(<see langword="in"/> parameter) The cross-line map.</param>
		/// <param name="baseCellsMap">(<see langword="in"/> parameter) The base cells map.</param>
		/// <param name="t1">The target cell 1.</param>
		/// <param name="t2">The target cell 2.</param>
		/// <returns>The mask of all incompatible values.</returns>
		private short CompatibilityTest(
			short baseCandidatesMask, Cells[] digitMaps, in Cells tempCrossline,
			in Cells baseCellsMap, int t1, int t2)
		{
			short result = 0;
			foreach (int digit in baseCandidatesMask)
			{
				bool flag = false;
				var baseElimsMap = (baseCellsMap & digitMaps[digit]).PeerIntersection;
				foreach (int[] combination in (tempCrossline & digitMaps[digit]).ToArray().GetSubsets(3))
				{
					var (a, b, c) = (combination[0], combination[1], combination[2]);
					int r1 = RegionLabel.Row.ToRegion(a), c1 = RegionLabel.Column.ToRegion(a);
					int r2 = RegionLabel.Row.ToRegion(b), c2 = RegionLabel.Column.ToRegion(b);
					int r3 = RegionLabel.Row.ToRegion(c), c3 = RegionLabel.Column.ToRegion(c);
					if (r1 == r2 || r1 == r3 || r2 == r3 || c1 == c2 || c1 == c3 || c2 == c3)
					{
						continue;
					}

					bool flag2 = false;
					var check = DigitMaps[digit] - (PeerMaps[a] | PeerMaps[b] | PeerMaps[c] | baseElimsMap);
					for (int i = 0; i < 27; i++)
					{
						if (!RegionMaps[i].Overlaps(check))
						{
							flag2 = true;
							break;
						}
					}
					if (flag2)
					{
						continue;
					}

					int n = 0;
					for (int i = 0; i < 3; i++)
					{
						if (combination[i] == t1) n |= 1;
						if (combination[i] == t2) n |= 2;
					}

					if (n.PopCount() == 2)
					{
						continue;
					}

					flag = true;
					break;
				}

				if (!flag)
				{
					result |= (short)(1 << digit);
				}
			}

			return result;
		}

		/// <summary>
		/// The compatibility testing after the method
		/// <see cref="CompatibilityTest(short, Cells[], in Cells, in Cells, int, int)"/>.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="compatibilityElims">The compatibility eliminations.</param>
		/// <param name="baseCellsMap">(<see langword="in"/> parameter) The base cells map.</param>
		/// <param name="baseCandidatesMask">The base candidates mask.</param>
		/// <param name="t1">The target cell 1.</param>
		/// <param name="t2">The target cell 2.</param>
		/// <seealso cref="CompatibilityTest(short, Cells[], in Cells, in Cells, int, int)"/>
		private void CompatibilityTest2(
			in SudokuGrid grid, CompatibilityTest compatibilityElims,
			in Cells baseCellsMap, short baseCandidatesMask, int t1, int t2)
		{
			if ((grid.GetStatus(t1), grid.GetStatus(t2)) is (not CellStatus.Empty, not CellStatus.Empty))
			{
				return;
			}

			foreach (var (currentTarget, elimTarget) in stackalloc[] { (t1, t2), (t2, t1) })
			{
				int r = RegionLabel.Row.ToRegion(currentTarget);
				int c = RegionLabel.Column.ToRegion(currentTarget);
				int b = RegionLabel.Block.ToRegion(currentTarget);
				foreach (int digit in baseCandidatesMask)
				{
					if (!grid.GetCandidates(currentTarget).ContainsBit(digit))
					{
						continue;
					}

					var temp = (PeerMaps[currentTarget] & DigitMaps[digit]) - baseCellsMap.PeerIntersection;

					bool flag = false;
					var elimMap = Cells.Empty;
					if (!temp.Overlaps(RegionMaps[r]))
					{
						flag = true;
						elimMap =
						(
							PeerMaps[currentTarget] & CandMaps[digit] & RegionMaps[r] | baseCellsMap
						).PeerIntersection & CandMaps[digit];
					}
					else if (!temp.Overlaps(RegionMaps[c]))
					{
						flag = true;
						elimMap =
						(
							PeerMaps[currentTarget] & CandMaps[digit] & RegionMaps[c] | baseCellsMap
						).PeerIntersection & CandMaps[digit];
					}
					else if (!temp.Overlaps(RegionMaps[b]))
					{
						flag = true;
						elimMap =
						(
							PeerMaps[currentTarget] & CandMaps[digit] & RegionMaps[b] | baseCellsMap
						).PeerIntersection & CandMaps[digit];
					}
					if (!flag)
					{
						continue;
					}

					if (grid.Exists(elimTarget, digit) is true)
					{
						compatibilityElims.Add(new(ConclusionType.Elimination, elimTarget, digit));
					}

					foreach (int cell in elimMap)
					{
						compatibilityElims.Add(new(ConclusionType.Elimination, cell, digit));
					}
				}
			}
		}
	}
}
