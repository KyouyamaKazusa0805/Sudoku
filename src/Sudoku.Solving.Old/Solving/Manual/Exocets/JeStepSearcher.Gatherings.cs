namespace Sudoku.Solving.Manual.Exocets;

partial class JeStepSearcher
{
	/// <summary>
	/// Gathering basic eliminations.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="elims">The elimination set.</param>
	/// <param name="cell">The cell to check eliminations.</param>
	/// <param name="baseCands">The mask that holds the digits in the base cells.</param>
	/// <param name="baseCandsWithAhsOrConjugatePair">
	/// The extra digits mask that holds in AHS or conjugate pair structure.
	/// </param>
	/// <returns>The <see cref="bool"/> result.</returns>
	private partial bool GatherBasic(
		in SudokuGrid grid,
		ref Candidates elims,
		int cell,
		short baseCands,
		short baseCandsWithAhsOrConjugatePair
	)
	{
		// Now get eliminations.
		// Firstly, we should get the possible digits that can be eliminated.
		// The digit can be removed if:
		// 1) The digit isn't the digit that belongs to the digits from base cells.
		// 2) The digit isn't the digit that belongs to the AHS or conjugate pair structure.
		// 3) Of course, the cell should be empty.
		short cellCands = grid.GetCandidates(cell);
		short cands = (short)(cellCands & ~baseCandsWithAhsOrConjugatePair);
		if (grid.GetStatus(cell) != CellStatus.Empty || cands == 0 || (cellCands & baseCands) == 0)
		{
			// None found.
			return false;
		}

		// Found any digits to remove. Now Iterate on each digit to remove them:
		// add them to the elimination set.
		foreach (int digit in cands)
		{
			elims.AddAnyway(cell * 9 + digit);
		}

		return !elims.IsEmpty;
	}
}
