namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Provides a step used for applying to a sudoku grid.
	/// </summary>
	public abstract class Step
	{
		/// <summary>
		/// Apply the step to the grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		public abstract void DoStepTo(UndoableGrid grid);

		/// <summary>
		/// Undo the step to the grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		public abstract void UndoStepTo(UndoableGrid grid);
	}
}
