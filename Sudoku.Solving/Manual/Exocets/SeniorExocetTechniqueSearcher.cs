using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using static System.Algorithms;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.GridMap.InitializeOption;
using static Sudoku.GridProcessings;
using static Sudoku.Data.ConclusionType;
using Sudoku.Data.Collections;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Encapsulates a <b>senior exocet</b> (SE) technique searcher.
	/// </summary>
	[TechniqueDisplay("Senior Exocet")]
	public sealed class SeniorExocetTechniqueSearcher : ExocetTechniqueSearcher
	{
		/// <inheritdoc/>
		public SeniorExocetTechniqueSearcher(bool checkAdvanced) : base(checkAdvanced)
		{
		}


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 96;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			var compatibleCellsPlayground = (Span<int>)stackalloc int[4];
			var cover = (Span<int>)stackalloc int[8];
			var (emptyCellsMap, _, candMap, digitMaps) = grid;
			foreach (var exocet in Exocets)
			{
				var (baseMap, targetMap, _) = exocet;
				var (b1, b2, tq1, tq2, tr1, tr2, s, mq1, mq2, mr1, mr2) = exocet;
				if (grid.GetCandidatesReversal(b1).CountSet() < 2
					|| grid.GetCandidatesReversal(b2).CountSet() < 2)
				{
					continue;
				}

				var baseCellsMap = new GridMap { b1, b2 };
				bool isRow = baseCellsMap.CoveredLine < 18;
				var tempCrosslineMap = new GridMap(s) { tq1, tq2, tr1, tr2 };
				short baseCandidatesMask = (short)(grid.GetCandidatesReversal(b1) | grid.GetCandidatesReversal(b2));

				int i = 0;
				var (r, c, _) = Cell.GetRegion(b1);
				foreach (int pos in ((short)(511 & ~(1 << (isRow ? r : c)))).GetAllSets())
				{
					cover[i++] = isRow ? pos : pos + 9;
				}

				i = 0;
				var temp = default(GridMap);
				foreach (int digit in baseCandidatesMask.GetAllSets())
				{
					if (i++ == 0)
					{
						temp = digitMaps[digit];
					}
					else
					{
						temp |= digitMaps[digit];
					}
				}
				temp &= tempCrosslineMap;

				var tempTarget = new List<int>();
				for (i = 0; i < 8; i++)
				{
					var check = temp & RegionMaps[cover[i] + 9];
					if (check.Count != 1)
					{
						continue;
					}

					tempTarget.Add(check.SetAt(0));
				}
				if (tempTarget.Count == 0)
				{
					continue;
				}

				int borT = isRow ? b1 / 9 / 3 : b1 % 9 / 3; // Base or target (B or T).
				foreach (int[] combination in GetCombinationsOfArray(tempTarget.ToArray(), 2))
				{
					if (isRow
						? combination[0] / 9 / 3 == borT && combination[1] / 9 / 3 == borT
						: combination[0] % 9 / 3 == borT && combination[1] % 9 / 3 == borT)
					{
						continue;
					}

					var (row1, column1, _) = Cell.GetRegion(combination[0]);
					var (row2, column2, _) = Cell.GetRegion(combination[1]);
					if (isRow ? column1 == column2 : row1 == row2)
					{
						continue;
					}

					short elimDigits = (short)((
						grid.GetCandidatesReversal(combination[0])
						| grid.GetCandidatesReversal(combination[1])) & ~baseCandidatesMask);
					if (!CheckCrossline(
						/*baseCellsMap, */tempCrosslineMap, digitMaps, baseCandidatesMask,
						combination[0], combination[1], isRow, out int extraRegionsMask))
					{
						continue;
					}

					var targetElims = new TargetEliminations();
					short cands = (short)(elimDigits & grid.GetCandidatesReversal(combination[0]));
					if (cands != 0)
					{
						foreach (int digit in cands.GetAllSets())
						{
							targetElims.Add(new Conclusion(Elimination, combination[0], digit));
						}
					}
					cands = (short)(elimDigits & grid.GetCandidatesReversal(combination[1]));
					if (cands != 0)
					{
						foreach (int digit in cands.GetAllSets())
						{
							targetElims.Add(new Conclusion(Elimination, combination[1], digit));
						}
					}

					short tbCands = 0;
					for (int j = 0; j < 2; j++)
					{
						if (grid.GetCandidatesReversal(combination[j]).CountSet() == 1)
						{
							tbCands |= grid.GetCandidatesReversal(combination[j]);
						}
					}

					var trueBaseElims = new TrueBaseEliminations();
					if (tbCands != 0
						&& (grid.GetStatus(combination[0]) != Empty || grid.GetStatus(combination[1]) != Empty))
					{
						for (int j = 0; j < 2; j++)
						{
							if (grid.GetStatus(combination[j]) != Empty)
							{
								continue;
							}

							if ((cands = (short)(grid.GetCandidatesReversal(combination[j]) & tbCands)) == 0)
							{
								continue;
							}

							foreach (int digit in cands.GetAllSets())
							{
								trueBaseElims.Add(new Conclusion(Elimination, combination[j], digit));
							}
						}
					}

					if (tbCands != 0)
					{
						foreach (int digit in tbCands.GetAllSets())
						{
							var elimMap =
								new GridMap((baseCellsMap & candMap[digit]).Offsets, ProcessPeersWithoutItself)
								& candMap[digit];
							if (elimMap.IsEmpty)
							{
								continue;
							}

							foreach (int cell in elimMap.Offsets)
							{
								trueBaseElims.Add(new Conclusion(Elimination, cell, digit));
							}
						}
					}

					var mirrorElims = new MirrorEliminations();
					var compatibilityElims = new CompatibilityTestEliminations();
					int target = -1;
					var mir = default(GridMap);

					var cellOffsets = new List<(int, int)> { (0, b1), (0, b2) };
					foreach (int cell in tempCrosslineMap.Offsets)
					{
						cellOffsets.Add((cell == combination[0] || cell == combination[1] ? 1 : 2, cell));
					}
					var candidateOffsets = new List<(int, int)>();
					if (_checkAdvanced)
					{
						for (int k = 0; k < 2; k++)
						{
							if (combination[k] == tq1 && (baseCandidatesMask & grid.GetCandidatesReversal(tr2)) == 0)
							{
								target = combination[k];
								mir = mq1;
							}
							if (combination[k] == tq2 && (baseCandidatesMask & grid.GetCandidatesReversal(tr1)) == 0)
							{
								target = combination[k];
								mir = mq2;
							}
							if (combination[k] == tr1 && (baseCandidatesMask & grid.GetCandidatesReversal(tq2)) == 0)
							{
								target = combination[k];
								mir = mr1;
							}
							if (combination[k] == tr2 && (baseCandidatesMask & grid.GetCandidatesReversal(tq1)) == 0)
							{
								target = combination[k];
								mir = mr2;
							}
						}

						if (target != -1)
						{
							var (tempTargetElims, tempMirrorElims) = CheckMirror(
								grid, target, combination[target == combination[0] ? 1 : 0], 0,
								baseCandidatesMask, mir, digitMaps, 0, -1, cellOffsets, candidateOffsets);
							targetElims = TargetEliminations.MergeAll(targetElims, tempTargetElims);
							mirrorElims = MirrorEliminations.MergeAll(mirrorElims, tempMirrorElims);
						}

						short incompatible = CompatibilityTest(
							baseCandidatesMask, digitMaps, tempCrosslineMap, baseCellsMap,
							combination[0], combination[1]);
						if (incompatible != 0)
						{
							compatibleCellsPlayground[0] = b1;
							compatibleCellsPlayground[1] = b2;
							compatibleCellsPlayground[2] = combination[0];
							compatibleCellsPlayground[3] = combination[1];

							for (int k = 0; k < 4; k++)
							{
								cands = (short)(incompatible & grid.GetCandidatesReversal(compatibleCellsPlayground[k]));
								if (cands == 0)
								{
									continue;
								}

								foreach (int digit in cands.GetAllSets())
								{
									compatibilityElims.Add(
										new Conclusion(Elimination, compatibleCellsPlayground[k], digit));
								}
							}
						}

						CompatibilityTest2(
							grid, ref compatibilityElims, baseCellsMap, digitMaps,
							candMap, baseCandidatesMask, combination[0], combination[1]);
					}

					if (_checkAdvanced
						? mirrorElims.Count == 0 && compatibilityElims.Count == 0
						&& targetElims.Count == 0 && trueBaseElims.Count == 0
						: mirrorElims.Count == 0 && compatibilityElims.Count == 0)
					{
						continue;
					}

					int endoTargetCell = combination[s[combination[0]] ? 0 : 1];
					short m1 = grid.GetCandidatesReversal(b1);
					short m2 = grid.GetCandidatesReversal(b2);
					short m = (short)(m1 | m2);
					foreach (int digit in m1.GetAllSets())
					{
						candidateOffsets.Add((0, b1 * 9 + digit));
					}
					foreach (int digit in m2.GetAllSets())
					{
						candidateOffsets.Add((0, b2 * 9 + digit));
					}

					// Record extra region cells (mutant exocets).
					foreach (int region in extraRegionsMask.GetAllSets())
					{
						foreach (int cell in RegionCells[region])
						{
							if (tempCrosslineMap[cell] || b1 == cell || b2 == cell)
							{
								continue;
							}

							cellOffsets.Add((2, cell));
						}
					}

					accumulator.Add(
						new SeniorExocetTechniqueInfo(
							conclusions: new List<Conclusion>(),
							views: new[]
							{
								new View(
									cellOffsets,
									candidateOffsets,
									regionOffsets: null,
									links: null)
							},
							exocet,
							digits: m.GetAllSets(),
							endoTargetCell,
							targetEliminations: targetElims,
							trueBaseEliminations: trueBaseElims,
							mirrorEliminations: mirrorElims,
							compatibilityEliminations: compatibilityElims));
				}
			}
		}

		///// <summary>
		///// Check the cross-line cells.
		///// </summary>
		///// <param name="baseCellsMap">The base cells map.</param>
		/// <summary>
		/// Check the cross-line cells.
		/// </summary>
		/// <param name="tempCrossline">The cross-line map.</param>
		/// <param name="digitMaps">The digit distributions.</param>
		/// <param name="baseCandidatesMask">The base candidate mask.</param>
		/// <param name="t1">The target cell 1.</param>
		/// <param name="t2">The target cell 2.</param>
		/// <param name="isRow">Indicates whether the specified computation is for rows.</param>
		/// <param name="extraRegionsMask">
		/// (<see langword="out"/> parameter) The extra region to add
		/// (used for franken/mutant exocets). If normal, the value will be 0 (not null if
		/// and only if need extra regions).
		/// </param>
		/// <returns>The <see cref="bool"/> result.</returns>
		private bool CheckCrossline(
			/*GridMap baseCellsMap, */GridMap tempCrossline, GridMap[] digitMaps,
			short baseCandidatesMask, int t1, int t2, bool isRow, out int extraRegionsMask)
		{
			var xx = new GridMap { t1, t2 };
			int tempMask = 0;
			foreach (int digit in baseCandidatesMask.GetAllSets())
			{
				bool flag = true;
				var temp = (tempCrossline & digitMaps[digit]) - xx;
				if ((isRow ? temp.RowMask : temp.ColumnMask).CountSet() > 2)
				{
					flag = false;
				}

				if (flag)
				{
					continue;
				}

				//if (DeepCrosslineCheck(
				//	digit,
				//	new GridMap(baseCellsMap & digitDistributions[digit], ProcessPeersWithoutItself),
				//	digitDistributions,
				//	temp,
				//	ref tempMask))
				//{
				//	continue;
				//}

				if (!flag)
				{
					extraRegionsMask = 0;
					return false;
				}
			}

			extraRegionsMask = tempMask;
			return true;
		}

		/// <summary>
		/// Deeply check of cross-line cells (franken/mutant exocets can be searched here).
		/// </summary>
		/// <param name="digit">The digit.</param>
		/// <param name="baseElimMap">The base elimination map.</param>
		/// <param name="digitMaps">The digit distributions.</param>
		/// <param name="tempCrossline">The cross-line map.</param>
		/// <param name="extraRegionsMask">
		/// (<see langword="ref"/> parameter) The extra regions.
		/// </param>
		/// <returns>The <see cref="bool"/> result.</returns>
		private bool DeepCrosslineCheck(
			int digit, GridMap baseElimMap, GridMap[] digitMaps, GridMap tempCrossline,
			ref int extraRegionsMask)
		{
			int i = default, p;
			foreach (int[] combination in GetCombinationsOfArray(tempCrossline.ToArray(), 3))
			{
				var (a, b, c) = (combination[0], combination[1], combination[2]);

				var (r1, c1, _) = Cell.GetRegion(a);
				var (r2, c2, _) = Cell.GetRegion(b);
				var (r3, c3, _) = Cell.GetRegion(c);
				if (r1 == r2 || r1 == r3 || r2 == r3 || c1 == c2 || c1 == c3 || c2 == c3)
				{
					continue;
				}

				bool flag = false;
				var check = digitMaps[digit] - (
					new GridMap(a, false) | new GridMap(b, false) | new GridMap(c, false) | baseElimMap);
				for (i = 9, p = 0; p < 27; i = (i + 1) % 27)
				{
					if (!RegionMaps[i].Overlaps(check))
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

			extraRegionsMask |= 1 << i;
			return true;
		}

		/// <summary>
		/// Compatibility test.
		/// </summary>
		/// <param name="baseCandidatesMask">The base candidates mask.</param>
		/// <param name="digitMaps">The digit distributions.</param>
		/// <param name="tempCrossline">The cross-line map.</param>
		/// <param name="baseCellsMap">The base cells map.</param>
		/// <param name="t1">The target cell 1.</param>
		/// <param name="t2">The target cell 2.</param>
		/// <returns>The mask of all incompatible values.</returns>
		private short CompatibilityTest(
			short baseCandidatesMask, GridMap[] digitMaps, GridMap tempCrossline,
			GridMap baseCellsMap, int t1, int t2)
		{
			short result = 0;
			foreach (int digit in baseCandidatesMask.GetAllSets())
			{
				bool flag = false;
				var baseElimsMap = new GridMap(baseCellsMap & digitMaps[digit], ProcessPeersWithoutItself);
				foreach (int[] combination in
					GetCombinationsOfArray((tempCrossline & digitMaps[digit]).ToArray(), 3))
				{
					var (a, b, c) = (combination[0], combination[1], combination[2]);
					var (r1, c1, _) = Cell.GetRegion(a);
					var (r2, c2, _) = Cell.GetRegion(b);
					var (r3, c3, _) = Cell.GetRegion(c);
					if (r1 == r2 || r1 == r3 || r2 == r3 || c1 == c2 || c1 == c3 || c2 == c3)
					{
						continue;
					}

					//bool flag2 = false;
					//var check = digitDistributions[digit] - (
					//	new GridMap(a, false) | new GridMap(b, false) | new GridMap(c, false) | baseElimsMap);
					//for (int i = 0; i < 27; i++)
					//{
					//	if (!RegionMaps[i].Overlaps(check))
					//	{
					//		flag2 = true;
					//		break;
					//	}
					//}
					//if (flag2)
					//{
					//	continue;
					//}

					int n = 0;
					for (int i = 0; i < 3; i++)
					{
						if (combination[i] == t1) n |= 1;
						if (combination[i] == t2) n |= 2;
					}

					if (n.CountSet() == 2)
					{
						continue;
					}
					else
					{
						flag = true;
						break;
					}
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
		/// <see cref="CompatibilityTest(short, GridMap[], GridMap, GridMap, int, int)"/>.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="compatibilityElims">The compatibility eliminations.</param>
		/// <param name="baseCellsMap">The base cells map.</param>
		/// <param name="digitMaps">The digit distributions.</param>
		/// <param name="candMaps">Candidate maps for each digit.</param>
		/// <param name="baseCandidatesMask">The base candidates mask.</param>
		/// <param name="t1">The target cell 1.</param>
		/// <param name="t2">The target cell 2.</param>
		/// <seealso cref="CompatibilityTest(short, GridMap[], GridMap, GridMap, int, int)"/>
		private void CompatibilityTest2(
			IReadOnlyGrid grid, ref CompatibilityTestEliminations compatibilityElims,
			GridMap baseCellsMap, GridMap[] digitMaps, GridMap[] candMaps,
			short baseCandidatesMask, int t1, int t2)
		{
			if (grid.GetStatus(t1) != Empty && grid.GetStatus(t2) != Empty)
			{
				return;
			}

			foreach (var (currentTarget, elimTarget) in stackalloc[] { (t1, t2), (t2, t1) })
			{
				var (r, c, b) = Cell.GetRegion(currentTarget);
				r += 9;
				c += 18;
				foreach (int digit in baseCandidatesMask.GetAllSets())
				{
					if ((grid.GetCandidatesReversal(currentTarget) >> digit & 1) == 0)
					{
						continue;
					}

					var temp = (new GridMap(currentTarget, false) & digitMaps[digit])
						- new GridMap(baseCellsMap, ProcessPeersWithoutItself);

					bool flag = false;
					var elimMap = GridMap.Empty;
					if (!temp.Overlaps(RegionMaps[r]))
					{
						flag = true;
						elimMap = new GridMap(
							new GridMap(currentTarget, false) & candMaps[digit] & RegionMaps[r] | baseCellsMap,
							ProcessPeersWithoutItself)
							& candMaps[digit];
					}
					else if (!temp.Overlaps(RegionMaps[c]))
					{
						flag = true;
						elimMap = new GridMap(
							new GridMap(currentTarget, false) & candMaps[digit] & RegionMaps[c] | baseCellsMap,
							ProcessPeersWithoutItself)
							& candMaps[digit];
					}
					else if (!temp.Overlaps(RegionMaps[b]))
					{
						flag = true;
						elimMap = new GridMap(
							new GridMap(currentTarget, false) & candMaps[digit] & RegionMaps[b] | baseCellsMap,
							ProcessPeersWithoutItself)
							& candMaps[digit];
					}
					if (!flag)
					{
						continue;
					}

					if (grid.Exists(elimTarget, digit) is true)
					{
						compatibilityElims.Add(new Conclusion(Elimination, elimTarget, digit));
					}

					if (elimMap.IsNotEmpty)
					{
						foreach (int cell in elimMap.Offsets)
						{
							compatibilityElims.Add(new Conclusion(Elimination, cell, digit));
						}
					}
				}
			}
		}
	}
}
