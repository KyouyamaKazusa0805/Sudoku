using System;
using System.Collections.Generic;
using System.Extensions;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Solving.Manual.Exocets.Eliminations;
using static Sudoku.Constants.Processings;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Encapsulates a <b>junior exocet</b> (JE) technique searcher.
	/// </summary>
	public sealed class JeStepSearcher : ExocetStepSearcher
	{
		/// <inheritdoc/>
		public JeStepSearcher(bool checkAdvanced) : base(checkAdvanced)
		{
		}


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(94, nameof(TechniqueCode.Je))
		{
			DisplayLevel = 4
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			// TODO: Extend JE eliminations checking.
			foreach (var exocet in Patterns)
			{
				var (baseMap, targetMap, _) = exocet;
				var (b1, b2, tq1, tq2, tr1, tr2, s, mq1, mq2, mr1, mr2) = exocet;

				// The base cells can't be given or modifiable.
				if (baseMap > EmptyMap)
				{
					continue;
				}

				// Base cells should be empty.
				if (grid.GetStatus(b1) != CellStatus.Empty || grid.GetStatus(b2) != CellStatus.Empty)
				{
					continue;
				}

				// The number of different candidates in base cells can't be greater than 5.
				short baseCandsMask = (short)(grid.GetCandidates(b1) | grid.GetCandidates(b2));
				if (baseCandsMask.PopCount() > 5)
				{
					continue;
				}

				// At least one cell should be empty.
				if (!targetMap.Overlaps(EmptyMap))
				{
					continue;
				}

				// Then check target eliminations.
				// Here 'nonBaseQ' and 'nonBaseR' are the conjugate pair in target Q and target R cells pair.
				if (!CheckTarget(grid, tq1, tq2, baseCandsMask, out short nonBaseQ)
					|| !CheckTarget(grid, tr1, tr2, baseCandsMask, out short nonBaseR))
				{
					continue;
				}

				// Get all locked members.
				int[] mq1o = mq1.Offsets, mq2o = mq2.Offsets, mr1o = mr1.Offsets, mr2o = mr2.Offsets;
				int v1 = grid.GetCandidates(mq1o[0]) | grid.GetCandidates(mq1o[1]);
				int v2 = grid.GetCandidates(mq2o[0]) | grid.GetCandidates(mq2o[1]);
				short temp = (short)(v1 | v2), needChecking = (short)(baseCandsMask & temp);

				v1 = grid.GetCandidates(mr1o[0]) | grid.GetCandidates(mr1o[1]);
				v2 = grid.GetCandidates(mr2o[0]) | grid.GetCandidates(mr2o[1]);
				temp = (short)(v1 | v2);
				needChecking &= temp;

				// Check crossline.
				if (!CheckCrossline(s, needChecking))
				{
					continue;
				}

				// Gather highlight cells and candidates.
				var cellOffsets = new List<DrawingInfo> { new(0, b1), new(0, b2) };
				var candidateOffsets = new List<DrawingInfo>();
				foreach (int digit in grid.GetCandidates(b1))
				{
					candidateOffsets.Add(new(0, b1 * 9 + digit));
				}
				foreach (int digit in grid.GetCandidates(b2))
				{
					candidateOffsets.Add(new(0, b2 * 9 + digit));
				}

				// Check target eliminations.
				// Here we can't replace the operator '|' with '||', because two methods both should be called.
				var targetElims = new Target();
				temp = (short)(nonBaseQ > 0 ? baseCandsMask | nonBaseQ : baseCandsMask);
				if (GatheringTargetElims(targetElims, tq1, grid, baseCandsMask, temp)
					| GatheringTargetElims(targetElims, tq2, grid, baseCandsMask, temp)
					&& nonBaseQ > 0
					&& grid.GetStatus(tq1) == CellStatus.Empty ^ grid.GetStatus(tq2) == CellStatus.Empty)
				{
					int conjugatPairDigit = nonBaseQ.FindFirstSet();
					if (grid.Exists(tq1, conjugatPairDigit) is true)
					{
						candidateOffsets.Add(new(1, tq1 * 9 + conjugatPairDigit));
					}
					if (grid.Exists(tq2, conjugatPairDigit) is true)
					{
						candidateOffsets.Add(new(1, tq2 * 9 + conjugatPairDigit));
					}
				}

				temp = (short)(nonBaseR > 0 ? baseCandsMask | nonBaseR : baseCandsMask);
				if (GatheringTargetElims(targetElims, tr1, grid, baseCandsMask, temp)
					| GatheringTargetElims(targetElims, tr2, grid, baseCandsMask, temp)
					&& nonBaseR > 0
					&& grid.GetStatus(tr1) == CellStatus.Empty ^ grid.GetStatus(tr2) == CellStatus.Empty)
				{
					int conjugatPairDigit = nonBaseR.FindFirstSet();
					if (grid.Exists(tr1, conjugatPairDigit) is true)
					{
						candidateOffsets.Add(new(1, tr1 * 9 + conjugatPairDigit));
					}
					if (grid.Exists(tr2, conjugatPairDigit) is true)
					{
						candidateOffsets.Add(new(1, tr2 * 9 + conjugatPairDigit));
					}
				}

				if (targetElims.Count == 0)
				{
					continue;
				}

				cellOffsets.Add(new(1, tq1));
				cellOffsets.Add(new(1, tq2));
				cellOffsets.Add(new(1, tr1));
				cellOffsets.Add(new(1, tr2));
				foreach (int cell in s)
				{
					cellOffsets.Add(new(2, cell));
				}

				accumulator.Add(
					new JeStepInfo(
						new List<Conclusion>(),
						new View[] { new() { Cells = cellOffsets, Candidates = candidateOffsets } },
						exocet,
						baseCandsMask.GetAllSets().ToArray(),
						null,
						null,
						targetElims,
						null,
						null,
						null,
						null));
			}
		}

		/// <summary>
		/// The method for gathering target eliminations.
		/// </summary>
		/// <param name="targetElims">The target eliminations.</param>
		/// <param name="cell">The cell.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="baseCandsMask">The base candidates mask.</param>
		/// <param name="temp">The temp mask.</param>
		/// <returns>
		/// A <see cref="bool"/> value indicating whether this method has been found eliminations.
		/// </returns>
		private static bool GatheringTargetElims(
			Target targetElims, int cell, in SudokuGrid grid, short baseCandsMask, short temp)
		{
			short candMask = (short)(grid.GetCandidates(cell) & ~temp);
			if (grid.GetStatus(cell) == CellStatus.Empty && candMask != 0
				&& (grid.GetCandidates(cell) & baseCandsMask) != 0)
			{
				foreach (int digit in candMask)
				{
					targetElims.Add(new(ConclusionType.Elimination, cell, digit));
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Check the cross-line cells.
		/// </summary>
		/// <param name="crossline">(<see langword="in"/> parameter) The cross line cells.</param>
		/// <param name="digitsNeedChecking">The digits that need checking.</param>
		/// <returns>
		/// A <see cref="bool"/> value indicating whether the structure passed the validation.
		/// </returns>
		private bool CheckCrossline(in Cells crossline, short digitsNeedChecking)
		{
			foreach (int digit in digitsNeedChecking)
			{
				var crosslinePerCandidate = crossline & DigitMaps[digit];
				short r = crosslinePerCandidate.RowMask, c = crosslinePerCandidate.ColumnMask;
				if ((r.PopCount(), c.PopCount()) is not ( > 2, > 2))
				{
					continue;
				}

				bool flag = false;
				foreach (int d1 in r)
				{
					foreach (int d2 in c)
					{
						if (crosslinePerCandidate < (RegionMaps[d1 + 9] | RegionMaps[d2 + 18]))
						{
							flag = true;
							goto FinalCheck;
						}
					}
				}

			FinalCheck:
				if (!flag)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Check the target cells.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="pos1">The cell 1 to determine.</param>
		/// <param name="pos2">The cell 2 to determine.</param>
		/// <param name="baseCandsMask">The base candidate mask.</param>
		/// <param name="otherCandsMask">
		/// (<see langword="out"/> parameter) The other candidate mask. If failed to check,
		/// the value will be <c>-1</c>.
		/// </param>
		/// <returns>The <see cref="bool"/> value.</returns>
		[SkipLocalsInit]
		private bool CheckTarget(
			in SudokuGrid grid, int pos1, int pos2, int baseCandsMask, out short otherCandsMask)
		{
			otherCandsMask = -1;

			short m1 = grid.GetCandidates(pos1), m2 = grid.GetCandidates(pos2);
			if ((baseCandsMask & m1) != 0 ^ (baseCandsMask & m2) != 0)
			{
				// One cell contains the digit that base candidate holds,
				// and another one doesn't contain.
				return true;
			}

			if ((m1 & baseCandsMask, m2 & baseCandsMask) == (0, 0)
				|| (m1 & ~baseCandsMask, m2 & ~baseCandsMask) == (0, 0))
			{
				// Two cells don't contain any digits in the base cells neither,
				// or both contains only digits from base cells,
				// which is not allowed in the exocet rule (or doesn't contain
				// any eliminations).
				return false;
			}

			// Now we check the special cases, in other words, the last two cells both contain
			// digits from base cells, and at least one cell contains non-base digits.
			// Therefore, we should check on non-base digits, whether the non-base digits
			// covers only one of two last cells; otherwise, false.
			short candidatesMask = (short)((m1 | m2) & ~baseCandsMask);
			var span = (Span<int>)stackalloc[]
			{
				RegionLabel.Block.ToRegion(pos1),
				RegionLabel.Row.ToRegion(pos1) == RegionLabel.Row.ToRegion(pos2)
				? RegionLabel.Row.ToRegion(pos1)
				: RegionLabel.Column.ToRegion(pos1)
			};
			foreach (short mask in Algorithms.GetMaskSubsets(candidatesMask))
			{
				for (int i = 0; i < 2; i++)
				{
					int count = 0;
					for (int j = 0; j < 9; j++)
					{
						int p = RegionCells[span[i]][j];
						if (p == pos1 || p == pos2 || grid.GetStatus(p) != CellStatus.Empty
							|| (grid.GetCandidates(p) & mask) == 0)
						{
							continue;
						}

						count++;
					}

					if (count == mask.PopCount() - 1)
					{
						for (int j = 0; j < 9; j++)
						{
							int p = RegionCells[span[i]][j];
							if (grid.GetStatus(p) != CellStatus.Empty || (grid.GetCandidates(p) & mask) == 0
								|| (grid.GetCandidates(p) & ~mask) == 0 || p == pos1 || p == pos2)
							{
								continue;
							}
						}

						otherCandsMask = mask;
						return true;
					}
				}
			}

			return false;
		}
	}
}
