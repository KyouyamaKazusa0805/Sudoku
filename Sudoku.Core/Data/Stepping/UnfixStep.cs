namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Encapsulates an unfix step.
	/// </summary>
	/// <param name="AllCells">Indicates all cells.</param>
	public sealed unsafe record UnfixStep(in GridMap AllCells) : Step
	{
		/// <inheritdoc/>
		public override void DoStepTo(UndoableGrid grid)
		{
			foreach (int cell in AllCells)
			{
				// To prevent the event re-invoke.
				ref short mask = ref grid._innerGrid._values[cell];
				mask = (short)((int)CellStatus.Modifiable << 9 | mask & SudokuGrid.MaxCandidatesMask);
			}
		}

		/// <inheritdoc/>
		public override void UndoStepTo(UndoableGrid grid)
		{
			foreach (int cell in AllCells)
			{
				// To prevent the event re-invoke.
				ref short mask = ref grid._innerGrid._values[cell];
				mask = (short)((int)CellStatus.Given << 9 | mask & SudokuGrid.MaxCandidatesMask);
			}
		}
	}
}
