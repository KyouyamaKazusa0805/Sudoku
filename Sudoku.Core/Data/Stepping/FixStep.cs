namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Encapsulates a fix step.
	/// </summary>
	/// <param name="AllCells">Indicates all cells to fix.</param>
	public sealed record FixStep(in GridMap AllCells) : IStep
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
					mask = (short)(SudokuGrid.GivenMask | mask & SudokuGrid.MaxCandidatesMask);
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
					mask = (short)(SudokuGrid.ModifiableMask | mask & SudokuGrid.MaxCandidatesMask);
				}
			}
		}
	}
}
