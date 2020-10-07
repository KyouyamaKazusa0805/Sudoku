using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Manual.Exocets.Eliminations;
using static Sudoku.Constants.Processings;
using static Sudoku.Constants.RegionLabel;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Encapsulates a <b>senior exocet</b> (SE) technique searcher.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.Se))]
	public sealed class SeniorExocetTechniqueSearcher : ExocetTechniqueSearcher
	{
		/// <inheritdoc/>
		public SeniorExocetTechniqueSearcher(bool checkAdvanced) : base(checkAdvanced)
		{
		}


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(96);


		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, Grid grid)
		{
			var compatibleCells = (stackalloc int[4]);
			var cover = (stackalloc int[8]);
			foreach (var exocet in Patterns)
			{
				var (baseMap, targetMap, _) = exocet;
				var (b1, b2, tq1, tq2, tr1, tr2, s, mq1, mq2, mr1, mr2) = exocet;
				if (grid.GetCandidateMask(b1).CountSet() < 2 || grid.GetCandidateMask(b2).CountSet() < 2)
				{
					continue;
				}

				bool isRow = baseMap.CoveredLine < 18;
				var tempCrosslineMap = s | targetMap;
				short baseCandidatesMask = (short)(grid.GetCandidateMask(b1) | grid.GetCandidateMask(b2));

				int i = 0;
				int r = GetRegion(b1, Row) - 9, c = GetRegion(b1, Column) - 18;
				foreach (int pos in Grid.MaxCandidatesMask & ~(1 << (isRow ? r : c)))
				{
					cover[i++] = isRow ? pos + 9 : pos + 18;
				}

				i = 0;
				GridMap temp = default;
				foreach (int digit in baseCandidatesMask)
				{
					if (i++ == 0)
					{
						temp = DigitMaps[digit];
					}
					else
					{
						temp |= DigitMaps[digit];
					}
				}
				temp &= tempCrosslineMap;

				var tempTarget = new List<int>();
				for (i = 0; i < 8; i++)
				{
					var check = temp & RegionMaps[cover[i]];
					if (check.Count != 1)
					{
						continue;
					}

					tempTarget.Add(check.First);
				}
				if (tempTarget.Count == 0)
				{
					continue;
				}
				int borT = isRow ? b1 / 9 / 3 : b1 % 9 / 3; // Base or target (B or T).
				foreach (int[] combination in tempTarget.ToArray().GetSubsets(2))
				{
					if (isRow
						? combination[0] / 9 / 3 == borT && combination[1] / 9 / 3 == borT
						: combination[0] % 9 / 3 == borT && combination[1] % 9 / 3 == borT)
					{
						continue;
					}

					int row1 = GetRegion(combination[0], Row), column1 = GetRegion(combination[0], Column);
					int row2 = GetRegion(combination[1], Row), column2 = GetRegion(combination[1], Column);
					if (isRow ? column1 == column2 : row1 == row2)
					{
						continue;
					}

					short elimDigits = (short)((
						grid.GetCandidateMask(combination[0]) | grid.GetCandidateMask(combination[1])
					) & ~baseCandidatesMask);
					if (!CheckCrossline(
						baseMap, tempCrosslineMap, baseCandidatesMask,
						combination[0], combination[1], isRow, out int[]? extraRegionsMask))
					{
						continue;
					}

					// Get all target eliminations.
					TargetEliminations targetElims = default;
					short cands = (short)(elimDigits & grid.GetCandidateMask(combination[0]));
					if (cands != 0)
					{
						foreach (int digit in cands)
						{
							targetElims.Add(new(Elimination, combination[0], digit));
						}
					}
					cands = (short)(elimDigits & grid.GetCandidateMask(combination[1]));
					if (cands != 0)
					{
						foreach (int digit in cands)
						{
							targetElims.Add(new(Elimination, combination[1], digit));
						}
					}

					short tbCands = 0;
					for (int j = 0; j < 2; j++)
					{
						if (grid.GetCandidateMask(combination[j]).CountSet() == 1)
						{
							tbCands |= grid.GetCandidateMask(combination[j]);
						}
					}

					// Get all true base eliminations.
					TrueBaseEliminations trueBaseElims = default;
					if (tbCands != 0
						&& (grid.GetStatus(combination[0]), grid.GetStatus(combination[1])) != (Empty, Empty))
					{
						for (int j = 0; j < 2; j++)
						{
							if (grid.GetStatus(combination[j]) != Empty)
							{
								continue;
							}

							if ((cands = (short)(grid.GetCandidateMask(combination[j]) & tbCands)) == 0)
							{
								continue;
							}

							foreach (int digit in cands)
							{
								trueBaseElims.Add(new(Elimination, combination[j], digit));
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
								trueBaseElims.Add(new(Elimination, cell, digit));
							}
						}
					}

					// Get mirror and compatibility test eliminations.
					MirrorEliminations mirrorElims = default;
					CompatibilityTestEliminations compatibilityElims = default;
					GridMap mir = default;
					int target = -1;
					var cellOffsets = new List<DrawingInfo> { new(0, b1), new(0, b2) };
					foreach (int cell in tempCrosslineMap)
					{
						cellOffsets.Add(new(cell == combination[0] || cell == combination[1] ? 1 : 2, cell));
					}
					var candidateOffsets = new List<DrawingInfo>();
					if (_checkAdvanced)
					{
						for (int k = 0; k < 2; k++)
						{
							if (combination[k] == tq1 && (baseCandidatesMask & grid.GetCandidateMask(tr2)) == 0)
							{
								target = combination[k];
								mir = mq1;
							}
							if (combination[k] == tq2 && (baseCandidatesMask & grid.GetCandidateMask(tr1)) == 0)
							{
								target = combination[k];
								mir = mq2;
							}
							if (combination[k] == tr1 && (baseCandidatesMask & grid.GetCandidateMask(tq2)) == 0)
							{
								target = combination[k];
								mir = mr1;
							}
							if (combination[k] == tr2 && (baseCandidatesMask & grid.GetCandidateMask(tq1)) == 0)
							{
								target = combination[k];
								mir = mr2;
							}
						}

						if (target != -1)
						{
							var (tempTargetElims, tempMirrorElims) =
								CheckMirror(
									grid, target, combination[target == combination[0] ? 1 : 0], 0,
									baseCandidatesMask, mir, 0, -1, cellOffsets, candidateOffsets);
							targetElims = TargetEliminations.MergeAll(targetElims, tempTargetElims);
							mirrorElims = MirrorEliminations.MergeAll(mirrorElims, tempMirrorElims);
						}

						short incompatible =
							CompatibilityTest(
								baseCandidatesMask, DigitMaps, tempCrosslineMap, baseMap, combination[0], combination[1]);
						if (incompatible != 0)
						{
							compatibleCells[0] = b1;
							compatibleCells[1] = b2;
							compatibleCells[2] = combination[0];
							compatibleCells[3] = combination[1];

							for (int k = 0; k < 4; k++)
							{
								cands = (short)(incompatible & grid.GetCandidateMask(compatibleCells[k]));
								if (cands == 0)
								{
									continue;
								}

								foreach (int digit in cands)
								{
									compatibilityElims.Add(new(Elimination, compatibleCells[k], digit));
								}
							}
						}

						CompatibilityTest2(
							grid, ref compatibilityElims, baseMap, baseCandidatesMask,
							combination[0], combination[1]);
					}

					if (_checkAdvanced
						? (mirrorElims.Count, compatibilityElims.Count, targetElims.Count, trueBaseElims.Count) == (0, 0, 0, 0)
						: (mirrorElims.Count, compatibilityElims.Count) == (0, 0))
					{
						continue;
					}

					int endoTargetCell = combination[s[combination[0]] ? 0 : 1];
					short m1 = grid.GetCandidateMask(b1), m2 = grid.GetCandidateMask(b2);
					short m = (short)(m1 | m2);
					foreach (int digit in m1)
					{
						candidateOffsets.Add(new(0, b1 * 9 + digit));
					}
					foreach (int digit in m2)
					{
						candidateOffsets.Add(new(0, b2 * 9 + digit));
					}

					// Gather extra region cells (mutant exocets).
					var extraMap = GridMap.Empty;
					for (int digit = 0; digit < 9; digit++)
					{
						foreach (int region in extraRegionsMask[digit])
						{
							foreach (int cell in RegionCells[region])
							{
								if (/*!tempCrosslineMap[cell] && */b1 != cell && b2 != cell && !extraMap[cell])
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
						new SeniorExocetTechniqueInfo(
							new List<Conclusion>(), // Special eliminations will use this empty list.
							new View[] { new(cellOffsets, candidateOffsets, null, null) },
							exocet,
							m.GetAllSets(),
							endoTargetCell,
							extraRegionsMask,
							targetElims,
							trueBaseElims,
							mirrorElims,
							compatibilityElims));
				}
			}
		}

		/// <summary>
		/// Check the cross-line cells.
		/// </summary>
		/// <param name="baseMap">The base cells map.</param>
		/// <param name="tempCrossline">The cross-line map.</param>
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
			GridMap baseMap, GridMap tempCrossline, short baseCandidatesMask,
			int t1, int t2, bool isRow, [NotNullWhen(true)] out int[]? extraRegionsMask)
		{
			var xx = new GridMap { t1, t2 };
			int[] tempMask = new int[9];
			foreach (int digit in baseCandidatesMask)
			{
				bool flag = true;
				var temp = (tempCrossline & DigitMaps[digit]) - xx;
				if ((isRow ? temp.RowMask : temp.ColumnMask).CountSet() > 2)
				{
					flag = false;
				}

				if (flag)
				{
					continue;
				}

				// Extra regions check.
				if (DeepCrosslineCheck(digit, (baseMap & DigitMaps[digit]).PeerIntersection, temp, tempMask))
				{
					continue;
				}

				if (!flag)
				{
					extraRegionsMask = null;
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
		/// <param name="tempCrossline">The cross-line map.</param>
		/// <param name="extraRegionsMask">The extra regions.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		private bool DeepCrosslineCheck(
			int digit, GridMap baseElimMap, GridMap tempCrossline, int[] extraRegionsMask)
		{
			int region = default, p;
			foreach (int[] combination in tempCrossline.ToArray().GetSubsets(3))
			{
				var (a, b, c) = (combination[0], combination[1], combination[2]);
				int r1 = GetRegion(a, Row), c1 = GetRegion(a, Column);
				int r2 = GetRegion(b, Row), c2 = GetRegion(b, Column);
				int r3 = GetRegion(c, Row), c3 = GetRegion(c, Column);
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

			extraRegionsMask[digit] |= 1 << region;
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
			foreach (int digit in baseCandidatesMask)
			{
				bool flag = false;
				var baseElimsMap = (baseCellsMap & digitMaps[digit]).PeerIntersection;
				foreach (int[] combination in (tempCrossline & digitMaps[digit]).ToArray().GetSubsets(3))
				{
					var (a, b, c) = (combination[0], combination[1], combination[2]);
					int r1 = GetRegion(a, Row);
					int c1 = GetRegion(a, Column);
					int r2 = GetRegion(b, Row);
					int c2 = GetRegion(b, Column);
					int r3 = GetRegion(c, Row);
					int c3 = GetRegion(c, Column);
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

					if (n.CountSet() == 2)
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
		/// <see cref="CompatibilityTest(short, GridMap[], GridMap, GridMap, int, int)"/>.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="compatibilityElims">The compatibility eliminations.</param>
		/// <param name="baseCellsMap">The base cells map.</param>
		/// <param name="baseCandidatesMask">The base candidates mask.</param>
		/// <param name="t1">The target cell 1.</param>
		/// <param name="t2">The target cell 2.</param>
		/// <seealso cref="CompatibilityTest(short, GridMap[], GridMap, GridMap, int, int)"/>
		private void CompatibilityTest2(
			Grid grid, ref CompatibilityTestEliminations compatibilityElims,
			GridMap baseCellsMap, short baseCandidatesMask, int t1, int t2)
		{
			if ((grid.GetStatus(t1), grid.GetStatus(t2)) is (not Empty, not Empty))
			{
				return;
			}

			foreach (var (currentTarget, elimTarget) in stackalloc[] { (t1, t2), (t2, t1) })
			{
				int r = GetRegion(currentTarget, Row);
				int c = GetRegion(currentTarget, Column);
				int b = GetRegion(currentTarget, Block);
				foreach (int digit in baseCandidatesMask)
				{
					if ((grid.GetCandidateMask(currentTarget) >> digit & 1) == 0)
					{
						continue;
					}

					var temp = (PeerMaps[currentTarget] & DigitMaps[digit]) - baseCellsMap.PeerIntersection;

					bool flag = false;
					var elimMap = GridMap.Empty;
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
						compatibilityElims.Add(new(Elimination, elimTarget, digit));
					}

					if (elimMap.IsNotEmpty)
					{
						foreach (int cell in elimMap)
						{
							compatibilityElims.Add(new(Elimination, cell, digit));
						}
					}
				}
			}
		}
	}
}
