namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Encapsulates a step for setting a cell status.
	/// </summary>
	/// <param name="Cell">The cell.</param>
	/// <param name="OldStatus">The old status.</param>
	/// <param name="NewStatus">The new status.</param>
	public sealed record SetStatusStep(int Cell, CellStatus OldStatus, CellStatus NewStatus) : IStep
	{
		/// <inheritdoc/>
		public void DoStepTo(UndoableGrid grid)
		{
			unsafe
			{
				fixed (short* pGrid = grid)
				{
					pGrid[Cell] = (short)((int)NewStatus << 9 | pGrid[Cell] & SudokuGrid.MaxCandidatesMask);
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
					pGrid[Cell] = (short)((int)OldStatus << 9 | pGrid[Cell] & SudokuGrid.MaxCandidatesMask);
				}
			}
		}
	}
}
