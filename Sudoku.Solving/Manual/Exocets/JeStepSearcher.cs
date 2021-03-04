using System;
using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Models;
using Sudoku.Solving.Manual.Extensions;
using Sudoku.Techniques;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.FastProperties;

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
		public static TechniqueProperties Properties { get; } = new(34, nameof(Technique.Je))
		{
			DisplayLevel = 4
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			// TODO: Extend JE eliminations checking.

			// We'll introduce the algorithm using this puzzle:
			// .-------------------------.---------------------.------------------------.
			// | 123578  23567     13568 | 4      567 B  267 B | 235679   236789  23589 |
			// | 4       23567 TQ  356   | 2567   8      9     | 1    TR  2367    235   |
			// | 2578    9     TQ  568   | 2567   3      1     | 2567 TR  24678   2458  |
			// :-------------------------+---------------------+------------------------:
			// | 123     234       1349  | 1278   1479   5     | 2379     123789  6     |
			// | 125     8         159   | 3      1679   267   | 4        1279    129   |
			// | 6       234       7     | 128    149    248   | 239      5       12389 |
			// :-------------------------+---------------------+------------------------:
			// | 357     1         3456  | 9      4567   3467  | 8        2346    2345  |
			// | 3578    34567     2     | 15678  14567  34678 | 3569     13469   13459 |
			// | 9       3456      34568 | 1568   2      3468  | 356      1346    7     |
			// '-------------------------'---------------------'------------------------'

			foreach (var exocet in Patterns)
			{
				var (b1, b2, tq1, tq2, tr1, tr2, s, mq1, mq2, mr1, mr2, _, targetMap, _) = exocet;

				// Base cells should be empty:
				// {567} and {267} are empty cells.
				if (grid.GetStatus(b1) != CellStatus.Empty || grid.GetStatus(b2) != CellStatus.Empty)
				{
					continue;
				}

				// The number of different candidates in base cells can't be greater than 5:
				// {267} | {567} = {2567}, Count{2567} = 4, which is <= 5.
				short baseCands = (short)(grid.GetCandidates(b1) | grid.GetCandidates(b2));
				if (PopCount((uint)baseCands) > 5)
				{
					continue;
				}

				// At least one cell in the target cells should be empty:
				// {23567}, {2567} are empty cells.
				if ((targetMap & EmptyMap).IsEmpty)
				{
					continue;
				}

				// Then check target eliminations.
				// Here 'nonBaseQ' and 'nonBaseR' are the conjugate pair (or AHS) digits
				// in target Q and target R cells pair.
				if (!CheckTarget(grid, tq1, tq2, baseCands, out short nonBaseQ)
					|| !CheckTarget(grid, tr1, tr2, baseCands, out short nonBaseR))
				{
					continue;
				}

				// Get all locked members.
				int[] mq1o = mq1.ToArray(), mq2o = mq2.ToArray(), mr1o = mr1.ToArray(), mr2o = mr2.ToArray();
				int v1 = grid.GetCandidates(mq1o[0]) | grid.GetCandidates(mq1o[1]);
				int v2 = grid.GetCandidates(mq2o[0]) | grid.GetCandidates(mq2o[1]);
				short temp = (short)(v1 | v2), needChecking = (short)(baseCands & temp);

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
				var targetElimsMap = Candidates.Empty;
				temp = (short)(nonBaseQ > 0 ? baseCands | nonBaseQ : baseCands);
				if (GatheringTargetElims(grid, ref targetElimsMap, tq1, baseCands, temp)
					| GatheringTargetElims(grid, ref targetElimsMap, tq2, baseCands, temp)
					&& nonBaseQ > 0
					&& grid.GetStatus(tq1) == CellStatus.Empty ^ grid.GetStatus(tq2) == CellStatus.Empty)
				{
					int conjugatPairDigit = TrailingZeroCount(nonBaseQ);
					if (grid.Exists(tq1, conjugatPairDigit) is true)
					{
						candidateOffsets.Add(new(1, tq1 * 9 + conjugatPairDigit));
					}
					if (grid.Exists(tq2, conjugatPairDigit) is true)
					{
						candidateOffsets.Add(new(1, tq2 * 9 + conjugatPairDigit));
					}
				}

				temp = (short)(nonBaseR > 0 ? baseCands | nonBaseR : baseCands);
				if (GatheringTargetElims(grid, ref targetElimsMap, tr1, baseCands, temp)
					| GatheringTargetElims(grid, ref targetElimsMap, tr2, baseCands, temp)
					&& nonBaseR > 0
					&& grid.GetStatus(tr1) == CellStatus.Empty ^ grid.GetStatus(tr2) == CellStatus.Empty)
				{
					int conjugatPairDigit = TrailingZeroCount(nonBaseR);
					if (grid.Exists(tr1, conjugatPairDigit) is true)
					{
						candidateOffsets.Add(new(1, tr1 * 9 + conjugatPairDigit));
					}
					if (grid.Exists(tr2, conjugatPairDigit) is true)
					{
						candidateOffsets.Add(new(1, tr2 * 9 + conjugatPairDigit));
					}
				}

				// Check mirror candidates.
				Elimination targetInferElims, mirrorElims;
				if (CheckAdvanced)
				{
					var (tar1, mir1) = GatheringMirrorElims(
						tq1, tq2, tr1, tr2, mq1, mq2, nonBaseQ, 0, grid,
						baseCands, cellOffsets, candidateOffsets
					);
					var (tar2, mir2) = GatheringMirrorElims(
						tr1, tr2, tq1, tq2, mr1, mr2, nonBaseR, 1, grid,
						baseCands, cellOffsets, candidateOffsets
					);

					targetInferElims = tar1 + tar2;
					mirrorElims = mir1 + mir2;
				}

				if (targetElimsMap.IsEmpty)
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

				var targetElims = new Elimination(targetElimsMap, EliminatedReason.Basic);
				unsafe
				{
					accumulator.Add(
						new JeStepInfo(
							new View[] { new() { Cells = cellOffsets, Candidates = candidateOffsets } },
							exocet,
							baseCands.GetAllSets().ToArray(),
							null,
							null,
							CheckAdvanced
							? new Elimination[]
							{
								targetElims,
								*&targetInferElims,
								*&mirrorElims
							}
							: new Elimination[] { targetElims }));
				}
			}
		}

		/// <summary>
		/// The method for gathering target eliminations.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="targetElimsMap">(<see langword="ref"/> parameter) The target eliminations.</param>
		/// <param name="cell">The cell.</param>
		/// <param name="baseCands">The base candidates mask.</param>
		/// <param name="temp">The temp mask.</param>
		/// <returns>
		/// A <see cref="bool"/> value indicating whether this method has been found eliminations.
		/// </returns>
		private bool GatheringTargetElims(
			in SudokuGrid grid, ref Candidates targetElimsMap, int cell, short baseCands, short temp)
		{
			short cands = (short)(grid.GetCandidates(cell) & ~temp);
			if (grid.GetStatus(cell) == CellStatus.Empty
				&& cands != 0 && (grid.GetCandidates(cell) & baseCands) != 0)
			{
				foreach (int digit in cands)
				{
					targetElimsMap.AddAnyway(cell * 9 + digit);
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Gathering mirror eliminations. This method is an entry for the method check mirror in base class.
		/// </summary>
		/// <param name="tq1">The target Q1 cell.</param>
		/// <param name="tq2">The target Q2 cell.</param>
		/// <param name="tr1">The target R1 cell.</param>
		/// <param name="tr2">The target R2 cell.</param>
		/// <param name="m1">(<see langword="in"/> parameter) The mirror 1 cell.</param>
		/// <param name="m2">(<see langword="in"/> parameter) The mirror 2 cell.</param>
		/// <param name="lockedNonTarget">The locked digits that is not the target digits.</param>
		/// <param name="x">The X digit.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="baseCands">The base candidates mask.</param>
		/// <param name="cellOffsets">The highlight cells.</param>
		/// <param name="candidateOffsets">The highliht candidates.</param>
		/// <returns>The result.</returns>
		private (Elimination Target, Elimination Mirror) GatheringMirrorElims(
			int tq1, int tq2, int tr1, int tr2, in Cells m1, in Cells m2, short lockedNonTarget,
			int x, in SudokuGrid grid, short baseCands, List<DrawingInfo> cellOffsets,
			List<DrawingInfo> candidateOffsets)
		{
			if ((grid.GetCandidates(tq1) & baseCands) != 0)
			{
				short mask1 = grid.GetCandidates(tr1), mask2 = grid.GetCandidates(tr2);
				short m1d = (short)(mask1 & baseCands), m2d = (short)(mask2 & baseCands);
				return CheckMirror(
					grid, tq1, tq2, lockedNonTarget != 0 ? lockedNonTarget : (short)0,
					baseCands, m1, x,
					(m1d, m2d) switch { (not 0, 0) => tr1, (0, not 0) => tr2, _ => -1 },
					cellOffsets, candidateOffsets);
			}
			else if ((grid.GetCandidates(tq2) & baseCands) != 0)
			{
				short mask1 = grid.GetCandidates(tq1), mask2 = grid.GetCandidates(tq2);
				short m1d = (short)(mask1 & baseCands), m2d = (short)(mask2 & baseCands);
				return CheckMirror(
					grid, tq2, tq1, lockedNonTarget != 0 ? lockedNonTarget : (short)0,
					baseCands, m2, x,
					(m1d, m2d) switch { (not 0, 0) => tr1, (0, not 0) => tr2, _ => -1 },
					cellOffsets, candidateOffsets);
			}
			else
			{
				return default;
			}
		}

		/// <summary>
		/// Check the cross-line cells.
		/// </summary>
		/// <param name="crossline">(<see langword="in"/> parameter) The cross line cells.</param>
		/// <param name="needChecking">The digits that need checking.</param>
		/// <returns>
		/// A <see cref="bool"/> value indicating whether the structure passed the validation.
		/// </returns>
		private bool CheckCrossline(in Cells crossline, short needChecking)
		{
			foreach (int digit in needChecking)
			{
				var crosslinePerCandidate = crossline & DigitMaps[digit];
				short r = crosslinePerCandidate.RowMask, c = crosslinePerCandidate.ColumnMask;

				// Basic check.
				// If the cells that contains the digit to check is spanned more than two regions,
				// the cross-line will be invalid; furthermore, the exocet is invalid.
				if (PopCount((uint)r) <= 2 || PopCount((uint)c) <= 2)
				{
					continue;
				}

				if (CheckAdvanced)
				{
					// Advanced check.
					// This checking is only used for complex exocets.
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
			}

			return true;
		}

		/// <summary>
		/// Check the target cells.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="pos1">The cell 1 to determine.</param>
		/// <param name="pos2">The cell 2 to determine.</param>
		/// <param name="baseCands">The candidates that is from base two cells.</param>
		/// <param name="ahsOrConjugatePairCands">
		/// (<see langword="out"/> parameter) The other candidate mask. If failed to check,
		/// the value will be -1.
		/// </param>
		/// <returns>The <see cref="bool"/> value.</returns>
		private unsafe bool CheckTarget(
			in SudokuGrid grid, int pos1, int pos2, int baseCands,
			out short ahsOrConjugatePairCands)
		{
			// According to the puzzle, we just describe for the TQ1 and TQ2 ({23567} and 9).
			// .------------------------.---------------------.-----------------------.
			// | 123578  23567    13568 | 4      567 B  267 B | 235679  236789  23589 |
			// | 4       23567 T  356   | 2567   8      9     | 1    T  2367    235   |
			// | 2578    9     T  568   | 2567   3      1     | 2567 T  24678   2458  |
			// :------------------------+---------------------+-----------------------:

			ahsOrConjugatePairCands = -1;

			// m1: {23567}
			// m2: {9}
			short m1 = grid.GetCandidates(pos1), m2 = grid.GetCandidates(pos2);

			// One cell contains the digit that base candidate holds,
			// and another one can't contain.
			// In this case, the target cells is valid.
			if ((baseCands & m1) != 0 ^ (baseCands & m2) != 0)
			{
				return true;
			}

			// Here checks:
			// 1) Target cells must hold the digits from base cells.
			// 2) Target cells must hold other digits against the base cells.
			// Otherwise, either no eliminations or invalid exocet structure.
			if ((m1 & baseCands) == 0 && (m2 & baseCands) == 0
				|| (m1 & ~baseCands) == 0 && (m2 & ~baseCands) == 0)
			{
				return false;
			}

			// Here we use another example, due to the above puzzle is invalid here.
			// New example:
			// .----------------------.------------------------.------------------------.
			// | 478 B  478 B   3     | 124578    1245   9     | 12478      6     12578 |
			// | 5      46789   46789 | 123478 T  1246   12678 | 1234789 T  1234  12378 |
			// | 2      1       46789 | 34578  T  456    5678  | 34789   T  345   3578  |
			// :----------------------+------------------------+------------------------:
			// | 468    24568   4568  | 1245      12456  3     | 128        7     9     |
			// | 3478   234578  1     | 9         245    257   | 6          235   2358  |
			// | 3679   235679  5679  | 1257      8      12567 | 123        1235  4     |
			// :----------------------+------------------------+------------------------:
			// | 1678   5678    5678  | 1258      3      4     | 127        9     1267  |
			// | 13469  34569   4569  | 125       7      125   | 1234       8     1236  |
			// | 13478  3478    2     | 6         9      18    | 5          134   137   |
			// '----------------------'------------------------'------------------------'

			// Suppose we use TQ1 and TQ2, so the values are:
			// m1: {123478}
			// m2: {34578}
			// baseCands: {478}
			// nonBaseCands: {1235}
			// regions: column 4 and block 2 (1 and 21)
			short nonBaseCands = (short)((m1 | m2) & ~baseCands);
			int* regions = stackalloc[]
			{
				pos1.ToRegion(RegionLabel.Block),
				pos1.ToRegion(
					pos1.ToRegion(RegionLabel.Row) == pos2.ToRegion(RegionLabel.Row)
					? RegionLabel.Row
					: RegionLabel.Column
				)
			};

			// Iterate on each combination of non-base candidates.
			// All cases: 1, 2, 3, 5, 12, 13, 15, 23, 25, 35, 123, 125, 135, 235, 1235.
			foreach (short mask in Algorithms.GetMaskSubsets(nonBaseCands))
			{
				// Iterate on each region in 'regions'.
				// All cases: block 2, column 4.
				for (int i = 0; i < 2; i++)
				{
					// Count the cells.
					int count = 0;
					for (int j = 0; j < 9; j++)
					{
						int p = RegionCells[regions[i]][j];

						// Check cases:
						// 1) Can't be the target cells.
						// 2) Can't be value cells (given or modifiable).
						// 3) Must hold the digits from base cells.
						// If at least one condition isn't satisfied, invalid.
						if (p == pos1 || p == pos2 || grid.GetStatus(p) != CellStatus.Empty
							|| (grid.GetCandidates(p) & mask) == 0)
						{
							continue;
						}

						// Cells to check:
						//  ↓Column 4
						// .-------------------------.
						// | 124578  :  1245         |
						// |         :  1246   12678 | block 2
						// |         :  456    5678  |
						// +---------+---------------+
						// | 1245    :
						// |         :
						// | 1257    :
						// +---------:
						// | 1258    :
						// | 125     :
						// |         :
						// '---------:
						// For example, if we check the combination {3} as the value of the variable 'mask',
						// 'count' should be 0.
						count++;
					}

					// Here is the AHS or conjugate pair checking, using the rank theory.
					// For example, if 'mask' is {3}, 'count' should be 0.
					// In this case, the condition returns true.
					// The right-side expression 'PopCount(mask) - 1' means
					// the region being checked should hold (n - 1) cells that contains the
					// values from 'mask', where 'n' means the number of the set bits of the variable 'mask'.
					//
					// AHS example:
					// .-----------------.-----------------------.-------------------.
					// | 24    14  5     | 1234      6      7    | 1234  8     9     |
					// | 2468  9   1234  | 12348  T  5      1234 | 7     136   12346 |
					// | 2468  78  12347 | 123489 T  289 E  1234 | 1234  136   5     |
					// :-----------------+-----------------------+-------------------:
					// | 1     56  24    | 2468      278    9    | 345   357   348   |
					// | 7     56  49    | 1468      3      146  | 1459  2     148   |
					// | 249   3   8     | 5         27     124  | 6     179   14    |
					// :-----------------+-----------------------+-------------------:
					// | 589   78  79    | 2369      1      2356 | 239   4     236   |
					// | 3     2   19    | 69        4      8    | 159   1569  7     |
					// | 459   14  6     | 7         29     235  | 8     139   123   |
					// '-----------------'-----------------------'-------------------'
					// Where the cell 'E' is the extra cell to construct an AHS structure of digits 8 and 9
					// with the target cells.
					// In this case, the elimination set will be extended to the extra cell 'E'.
					if (count == PopCount((uint)mask) - 1)
					{
						for (int j = 0; j < 9; j++)
						{
							int p = RegionCells[regions[i]][j];
							if (grid.GetStatus(p) == CellStatus.Empty
								&& (grid.GetCandidates(p) & mask) != 0
								&& (grid.GetCandidates(p) & ~mask) != 0
								&& p != pos1 && p != pos2)
							{
								// The conditon passed when:
								// 1) Can't be target cells.
								// 2) Can't be value cells (given or modifiable).
								// 3) Must contain base candidates.
								// 4) Must contain non-base candiates.
								ahsOrConjugatePairCands = mask;
								return true;
							}
						}
					}
				}
			}

			return false;
		}
	}
}
