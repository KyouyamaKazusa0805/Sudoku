using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Models;
using Sudoku.Solving.Manual.Extensions;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.FastProperties;
using SolvingAlgorithms = Sudoku.Solving.Extensions.Algorithms;

namespace Sudoku.Solving.Manual.Exocets
{
	partial class JeStepSearcher
	{
		/// <summary>
		/// Check the cross-line cells.
		/// </summary>
		/// <param name="crossline">(<see langword="in"/> parameter) The cross line cells.</param>
		/// <param name="needChecking">The digits that need checking.</param>
		/// <returns>
		/// A <see cref="bool"/> value indicating whether the structure passed the validation.
		/// </returns>
		private unsafe partial bool CheckCrossline(in Cells crossline, short needChecking)
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
		private unsafe partial bool CheckTarget(
			in SudokuGrid grid, int pos1, int pos2, int baseCands, out short ahsOrConjugatePairCands)
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
			foreach (short mask in SolvingAlgorithms.GetMaskSubsets(nonBaseCands))
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

		/// <summary>
		/// Check mirror eliminations.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="target">The target cell.</param>
		/// <param name="lockedNontarget">The locked member that is non-target digits.</param>
		/// <param name="baseCands">The base candidate mask.</param>
		/// <param name="mirror">(<see langword="in"/> parameter) The mirror map.</param>
		/// <param name="onlyOne">The only one cell.</param>
		/// <param name="cellOffsets">The cell offsets.</param>
		/// <param name="candidateOffsets">The candidate offsets.</param>
		/// <param name="resultPair">
		/// The result eliminations (contains the target inference and mirror eliminations).
		/// </param>
		/// <returns>A <see cref="bool"/> result indicating whether the elimination exists.</returns>
		private unsafe partial void CheckMirror(
			in SudokuGrid grid, int target, short lockedNontarget, short baseCands, in Cells mirror,
			int onlyOne, IList<DrawingInfo> cellOffsets, IList<DrawingInfo> candidateOffsets,
			out (Candidates Target, Candidates Mirror) resultPair)
		{
			// Gets the basic information, and gets the digits that can be eliminated.
			Candidates targetElims = Candidates.Empty, mirrorElims = Candidates.Empty;
			int l = mirror[0], r = mirror[1];
			short mirrorCands = (short)(grid.GetCandidates(l) | grid.GetCandidates(r));
			short commonBase = (short)(mirrorCands & baseCands & grid.GetCandidates(target));
			short targetElimDigits = (short)(grid.GetCandidates(target) & ~(short)(commonBase | lockedNontarget));
			if (targetElimDigits != 0)
			{
				foreach (int digit in targetElimDigits)
				{
					targetElims.AddAnyway(target * 9 + digit);
				}
			}

			short m1 = (short)(grid.GetCandidates(l) & baseCands);
			short m2 = (short)(grid.GetCandidates(r) & baseCands);
			if (m1 != 0 ^ m2 != 0)
			{
				// One cell contains the digits from base cells,
				// and another cell is a given cell that is filled with non-base digits.
				int p = m1 == 0 ? r : l;
				short cands = (short)(grid.GetCandidates(p) & ~commonBase);
				if (cands != 0)
				{
					cellOffsets.Add(new(3, l));
					cellOffsets.Add(new(3, r));
					foreach (int digit in cands)
					{
						mirrorElims.AddAnyway(p * 9 + digit);
					}
				}
			}

			short nonBase = (short)(mirrorCands & ~baseCands);
			int regions = new Cells { l, r }.CoveredRegions;
			short locked = default;
			foreach (short mask in SolvingAlgorithms.GetMaskSubsets(nonBase))
			{
				foreach (int region in regions)
				{
					int count = 0;
					foreach (int p in RegionCells[region])
					{
						if (p == l || p == r || p == onlyOne)
						{
							continue;
						}

						if ((grid.GetCandidates(p) & mask) == 0)
						{
							continue;
						}

						count++;
					}

					if (count == PopCount((uint)mask) - 1)
					{
						foreach (int p in RegionCells[region])
						{
							if ((grid.GetCandidates(p) & mask) == 0
								|| grid.GetStatus(p) != CellStatus.Empty || p == onlyOne)
							{
								continue;
							}

							foreach (int digit in grid.GetCandidates(p) & mask)
							{
								candidateOffsets.Add(new(3, p * 9 + digit));
							}

							if (p == l || p == r || (grid.GetCandidates(p) & ~mask) == 0)
							{
								continue;
							}

							foreach (int digit in grid.GetCandidates(p) & ~mask)
							{
								mirrorElims.AddAnyway(p * 9 + digit);
							}
						}

						locked = mask;
						break;
					}
				}

				if (locked != 0)
				{
					// Here you should use '|' operator rather than '||'.
					// Operator '||' won't execute the second method if the first condition is true.
					if (g(grid, l, ref mirrorElims, baseCands, locked)
						| g(grid, r, ref mirrorElims, baseCands, locked))
					{
						cellOffsets.Add(new(3, l));
						cellOffsets.Add(new(3, r));
					}

					bool m1Locked = (grid.GetCandidates(l) & locked) != 0;
					bool m2Locked = (grid.GetCandidates(r) & locked) != 0;
					if ((locked & locked - 1) == 0 && m1Locked ^ m2Locked)
					{
						short targetCands = (short)(grid.GetCandidates(target) & baseCands);
						short cands = (short)(
							~(grid.GetCandidates(m1Locked ? r : l) & targetCands) & targetCands
						);
						if (cands != 0)
						{
							foreach (int digit in cands)
							{
								mirrorElims.AddAnyway(target * 9 + digit);
							}
						}
					}

					break;

					// Gathering.
					bool g(in SudokuGrid grid, int m, ref Candidates mirrorElims, short baseCands, short locked)
					{
						short cands = (short)(grid.GetCandidates(m) & ~(baseCands | locked));
						if (cands == 0)
						{
							return false;
						}

						foreach (int digit in locked)
						{
							if (grid.Exists(m, digit) is true)
							{
								candidateOffsets.Add(new(1, m * 9 + digit));
							}
						}

						foreach (int digit in cands)
						{
							mirrorElims.AddAnyway(m * 9 + digit);
						}

						return true;
					}
				}
			}

			resultPair = (targetElims, mirrorElims);
		}
	}
}
