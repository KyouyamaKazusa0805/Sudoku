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
				// To prevent the infinity recursion.
				ref short mask = ref grid._innerGrid._values[Cell];
				mask = (short)((int)NewStatus << 9 | mask & SudokuGrid.MaxCandidatesMask);
			}
		}

		/// <inheritdoc/>
		public void UndoStepTo(UndoableGrid grid)
		{
			unsafe
			{
				// To prevent the infinity recursion.
				ref short mask = ref grid._innerGrid._values[Cell];
				mask = (short)((int)OldStatus << 9 | mask & SudokuGrid.MaxCandidatesMask);
			}
		}
	}
}
