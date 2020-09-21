namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Encapsulates a step for setting a cell status.
	/// </summary>
	/// <param name="Cell">The cell.</param>
	/// <param name="OldStatus">The old status.</param>
	/// <param name="NewStatus">The new status.</param>
	public sealed record SetStatusStep(int Cell, CellStatus OldStatus, CellStatus NewStatus) : Step
	{
		/// <inheritdoc/>
		public override void DoStepTo(UndoableGrid grid)
		{
			// To prevent the infinity recursion.
			ref short mask = ref grid._masks[Cell];
			mask = (short)((int)NewStatus << 9 | mask & Grid.MaxCandidatesMask);
		}

		/// <inheritdoc/>
		public override void UndoStepTo(UndoableGrid grid)
		{
			// To prevent the infinity recursion.
			ref short mask = ref grid._masks[Cell];
			mask = (short)((int)OldStatus << 9 | mask & Grid.MaxCandidatesMask);
		}
	}
}
