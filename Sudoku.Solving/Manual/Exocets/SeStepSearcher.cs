using System.Collections.Generic;
using System.Extensions;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Manual.Exocets.Eliminations;
using static Sudoku.Constants.Processings;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Encapsulates a <b>senior exocet</b> (SE) technique searcher.
	/// </summary>
	public sealed class SeStepSearcher : ExocetStepSearcher
	{
		/// <inheritdoc/>
		public SeStepSearcher(bool checkAdvanced) : base(checkAdvanced)
		{
		}


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(96, nameof(TechniqueCode.Se))
		{
			DisplayLevel = 4
		};


		/// <inheritdoc/>
		[SkipLocalsInit]
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			var compatibleCells = (stackalloc int[4]);
			var cover = (stackalloc int[8]);
			foreach (var exocet in Patterns)
			{
				var (baseMap, targetMap, _) = exocet;
				var (b1, b2, tq1, tq2, tr1, tr2, s, mq1, mq2, mr1, mr2) = exocet;
				if (grid.GetCandidateMask(b1).PopCount() < 2 || grid.GetCandidateMask(b2).PopCount() < 2)
				{
					continue;
				}

				bool isRow = baseMap.CoveredLine < 18;
				var tempCrosslineMap = s | targetMap;
				short baseCandsMask = (short)(grid.GetCandidateMask(b1) | grid.GetCandidateMask(b2));

				int i = 0;
				int r = RegionLabel.Row.GetRegion(b1) - 9, c = RegionLabel.Column.GetRegion(b1) - 18;
				foreach (int pos in SudokuGrid.MaxCandidatesMask & ~(1 << (isRow ? r : c)))
				{
					cover[i++] = isRow ? pos + 9 : pos + 18;
				}

				i = 0;
				Cells temp;
				unsafe
				{
					foreach (int digit in baseCandsMask)
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
				}

				var tempTarget = new List<int>();
				for (i = 0; i < 8; i++)
				{
					if ((temp & RegionMaps[cover[i]]) is { Count: 1 } check)
					{
						tempTarget.Add(check.First);
					}
				}
				if (tempTarget.Count == 0)
				{
					continue;
				}

				int bOrT = isRow ? b1 / 9 / 3 : b1 % 9 / 3; // Base or target (B or T).
				foreach (int[] comb in tempTarget.ToArray().GetSubsets(2))
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)] static int a(int v) => v / 9 / 3;
					[MethodImpl(MethodImplOptions.AggressiveInlining)] static int b(int v) => v % 9 / 3;

					int v1 = comb[0], v2 = comb[1];
					if (isRow ? a(v1) == bOrT && a(v2) == bOrT : b(v1) == bOrT && b(v2) == bOrT)
					{
						continue;
					}

					int row1 = RegionLabel.Row.GetRegion(v1), column1 = RegionLabel.Column.GetRegion(v1);
					int row2 = RegionLabel.Row.GetRegion(v2), column2 = RegionLabel.Column.GetRegion(v2);
					if (isRow ? column1 == column2 : row1 == row2)
					{
						continue;
					}

					short elimDigits = (short)((
						grid.GetCandidateMask(v1) | grid.GetCandidateMask(v2)
					) & ~baseCandsMask);
					if (!CheckCrossline(baseMap, tempCrosslineMap, baseCandsMask, v1, v2, isRow))
					{
						continue;
					}

					// Get all target eliminations.
					var targetElims = new Target();
					short cands = (short)(elimDigits & grid.GetCandidateMask(v1));
					if (cands != 0)
					{
						foreach (int digit in cands)
						{
							targetElims.Add(new(ConclusionType.Elimination, v1, digit));
						}
					}
					cands = (short)(elimDigits & grid.GetCandidateMask(v2));
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
						if (grid.GetCandidateMask(comb[j]).PopCount() == 1)
						{
							tbCands |= grid.GetCandidateMask(comb[j]);
						}
					}

					// Get all true base eliminations.
					var trueBaseElims = new TrueBase();
					if (tbCands != 0 && (
						grid.GetStatus(v1) != CellStatus.Empty || grid.GetStatus(v2) != CellStatus.Empty))
					{
						for (int j = 0; j < 2; j++)
						{
							if (grid.GetStatus(comb[j]) != CellStatus.Empty)
							{
								continue;
							}

							if ((cands = (short)(grid.GetCandidateMask(comb[j]) & tbCands)) == 0)
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
							foreach (int cell in elimMap)
							{
								trueBaseElims.Add(new(ConclusionType.Elimination, cell, digit));
							}
						}
					}

					if (targetElims.Count == 0 && trueBaseElims.Count == 0)
					{
						continue;
					}

					// Get mirror and compatibility test eliminations.
					var cellOffsets = new List<DrawingInfo> { new(0, b1), new(0, b2) };
					foreach (int cell in tempCrosslineMap)
					{
						cellOffsets.Add(new(cell == v1 || cell == v2 ? 1 : 2, cell));
					}
					var candidateOffsets = new List<DrawingInfo>();

					int endoTargetCell = comb[s[v1] ? 0 : 1];
					short m1 = grid.GetCandidateMask(b1), m2 = grid.GetCandidateMask(b2), m = (short)(m1 | m2);
					foreach (int digit in m1)
					{
						candidateOffsets.Add(new(0, b1 * 9 + digit));
					}
					foreach (int digit in m2)
					{
						candidateOffsets.Add(new(0, b2 * 9 + digit));
					}

					accumulator.Add(
						new SeStepInfo(
							new List<Conclusion>(), // Special eliminations will use this empty list.
							new View[] { new() { Cells = cellOffsets, Candidates = candidateOffsets } },
							exocet,
							m.GetAllSets(),
							endoTargetCell,
							null,
							targetElims,
							trueBaseElims,
							null,
							null));
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
		/// <returns>The <see cref="bool"/> result.</returns>
		[SkipLocalsInit]
		private bool CheckCrossline(
			in Cells baseMap, in Cells tempCrossline, short baseCandidatesMask, int t1, int t2, bool isRow)
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

				if (!flag)
				{
					return false;
				}
			}

			return true;
		}
	}
}
