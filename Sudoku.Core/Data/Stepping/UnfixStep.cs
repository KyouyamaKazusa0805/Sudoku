namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Encapsulates an unfix step.
	/// </summary>
	/// <param name="AllCells">Indicates all cells.</param>
	public sealed record UnfixStep(in GridMap AllCells) : IStep
	{
		/// <inheritdoc/>
		public void DoStepTo(UndoableGrid grid)
		{
			unsafe
			{
				foreach (int cell in AllCells)
				{
					// To prevent the event re-invoke.
					ref short mask = ref grid._innerGrid._values[cell];
					mask = (short)((int)CellStatus.Modifiable << 9 | mask & SudokuGrid.MaxCandidatesMask);
				}
			}
		}

		/// <inheritdoc/>
		public void UndoStepTo(UndoableGrid grid)
		{
			unsafe
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
}
