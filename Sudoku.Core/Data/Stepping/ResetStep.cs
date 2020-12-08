namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Encapsulates a reset step.
	/// </summary>
	/// <param name="OldMasks">Indicates the table of new grid masks.</param>
	/// <param name="NewMasks">Indicates the table of old grid masks.</param>
	public sealed unsafe record ResetStep(short* OldMasks, short* NewMasks) : IStep
	{
		/// <inheritdoc/>
		public void DoStepTo(UndoableGrid grid)
		{
			fixed (short* pGrid = grid)
			{
				SudokuGrid.InternalCopy(pGrid, OldMasks);
			}
		}

		/// <inheritdoc/>
		public void UndoStepTo(UndoableGrid grid)
		{
			fixed (short* pGrid = grid)
			{
				SudokuGrid.InternalCopy(pGrid, NewMasks);
			}
		}
	}
}
