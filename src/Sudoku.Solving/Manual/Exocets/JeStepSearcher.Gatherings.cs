using Sudoku.Data;
using Sudoku.Models;
using System.Collections.Generic;
using System.Extensions;

namespace Sudoku.Solving.Manual.Exocets
{
	partial class JeStepSearcher
	{
		/// <summary>
		/// Gathering basic eliminations.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="elims">(<see langword="ref"/> parameter) The elimination set.</param>
		/// <param name="cell">The cell to check eliminations.</param>
		/// <param name="baseCands">The mask that holds the digits in the base cells.</param>
		/// <param name="baseCandsWithAhsOrConjugatePair">
		/// The extra digits mask that holds in AHS or conjugate pair structure.
		/// </param>
		/// <returns>The <see cref="bool"/> result.</returns>
		private partial bool GatherBasic(
			in SudokuGrid grid, ref Candidates elims, int cell, short baseCands,
			short baseCandsWithAhsOrConjugatePair)
		{
			// Now get eliminations.
			// Firstly, we should get the possible digits that can be eliminated.
			// The digit can be removed if:
			// 1) The digit isn't the digit that belongs to the digits from base cells.
			// 2) The digit isn't the digit that belongs to the AHS or conjugate pair structure.
			// 3) Of course, the cell should be empty.
			short cellCands = grid.GetCandidates(cell);
			short cands = (short)(cellCands & ~baseCandsWithAhsOrConjugatePair);
			if (grid.GetStatus(cell) == CellStatus.Empty && cands != 0 && (cellCands & baseCands) != 0)
			{
				// Found any digits to remove. Now Iterate on each digit to remove them:
				// add them to the elimination set.
				foreach (int digit in cands)
				{
					elims.AddAnyway(cell * 9 + digit);
				}

				return true;
			}
			else
			{
				// None found.
				return false;
			}
		}

		/// <summary>
		/// Gathering mirror eliminations.
		/// </summary>
		/// <param name="tq1">The target cells Q1.</param>
		/// <param name="tq2">The target cells Q2.</param>
		/// <param name="tr1">The target cells R1.</param>
		/// <param name="tr2">The target cells R2.</param>
		/// <param name="m1">(<see langword="in"/> parameter) The mirror cell 1.</param>
		/// <param name="m2">(<see langword="in"/> parameter) The mirror cell 2.</param>
		/// <param name="ahsOrConjugatePairDigits">
		/// The digits mask that holds the digits in AHS or conjuagte pair structure.
		/// </param>
		/// <param name="x">The extra digit.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="baseCands">The base digits.</param>
		/// <param name="cellOffsets">The highlight cells.</param>
		/// <param name="candidateOffsets">The highlight candidates.</param>
		/// <param name="resultPair">(<see langword="out"/> parameter) The result pair.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		private partial bool GatherMirror(
			int tq1, int tq2, int tr1, int tr2, in Cells m1, in Cells m2,
			short ahsOrConjugatePairDigits, int x, in SudokuGrid grid, short baseCands,
			List<DrawingInfo> cellOffsets, List<DrawingInfo> candidateOffsets,
			out (Elimination Target, Elimination Mirror) resultPair)
		{
			if ((grid.GetCandidates(tq1) & baseCands) != 0)
			{
				short mask1 = grid.GetCandidates(tr1), mask2 = grid.GetCandidates(tr2);
				short m1d = (short)(mask1 & baseCands), m2d = (short)(mask2 & baseCands);
				return CheckMirror(
					grid, tq1, tq2, ahsOrConjugatePairDigits != 0 ? ahsOrConjugatePairDigits : (short)0,
					baseCands, m1, x, (m1d, m2d) switch { (not 0, 0) => tr1, (0, not 0) => tr2, _ => -1 },
					cellOffsets, candidateOffsets, out resultPair
				);
			}
			else if ((grid.GetCandidates(tq2) & baseCands) != 0)
			{
				short mask1 = grid.GetCandidates(tq1), mask2 = grid.GetCandidates(tq2);
				short m1d = (short)(mask1 & baseCands), m2d = (short)(mask2 & baseCands);
				return CheckMirror(
					grid, tq2, tq1, ahsOrConjugatePairDigits != 0 ? ahsOrConjugatePairDigits : (short)0,
					baseCands, m2, x, (m1d, m2d) switch { (not 0, 0) => tr1, (0, not 0) => tr2, _ => -1 },
					cellOffsets, candidateOffsets, out resultPair
				);
			}
			else
			{
				resultPair = default;
				return false;
			}
		}
	}
}
