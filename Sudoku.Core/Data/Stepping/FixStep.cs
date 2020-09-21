namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Encapsulates a fix step.
	/// </summary>
	/// <param name="AllCells">Indicates all cells to fix.</param>
	public sealed record FixStep(GridMap AllCells) : Step
	{
		/// <inheritdoc/>
		public override void DoStepTo(UndoableGrid grid)
		{
			foreach (int cell in AllCells)
			{
				// To prevent the event re-invoke.
				ref short mask = ref grid._masks[cell];
				mask = (short)((int)CellStatus.Given << 9 | mask & Grid.MaxCandidatesMask);
			}
		}

		/// <inheritdoc/>
		public override void UndoStepTo(UndoableGrid grid)
		{
			foreach (int cell in AllCells)
			{
				// To prevent the event re-invoke.
				ref short mask = ref grid._masks[cell];
				mask = (short)((int)CellStatus.Modifiable << 9 | mask & Grid.MaxCandidatesMask);
			}
		}
	}
}
