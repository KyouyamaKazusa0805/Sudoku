namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Encapsulates an unfix step.
	/// </summary>
	/// <param name="AllCells">Indicates all cells.</param>
	public sealed record UnfixStep(in Cells AllCells) : IStep
	{
		/// <inheritdoc/>
		public void DoStepTo(UndoableGrid grid)
		{
			unsafe
			{
				fixed (short* pGrid = grid)
				{
					foreach (int cell in AllCells)
					{
						pGrid[cell] = (short)(SudokuGrid.ModifiableMask | pGrid[cell] & SudokuGrid.MaxCandidatesMask);
					}
				}
			}
		}

		/// <inheritdoc/>
		public void UndoStepTo(UndoableGrid grid)
		{
			unsafe
			{
				fixed (short* pGrid = grid)
				{
					foreach (int cell in AllCells)
					{
						pGrid[cell] = (short)(SudokuGrid.GivenMask | pGrid[cell] & SudokuGrid.MaxCandidatesMask);
					}
				}
			}
		}
	}
}
