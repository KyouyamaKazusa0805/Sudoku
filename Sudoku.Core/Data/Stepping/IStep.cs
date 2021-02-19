namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Provides a step used for applying to a sudoku grid.
	/// </summary>
#if SUDOKU_UI
	[Obsolete("In the future, this type won't be used.", false)]
#endif
	public interface IStep
	{
		/// <summary>
		/// Apply the step to the grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		void DoStepTo(UndoableGrid grid);

		/// <summary>
		/// Undo the step to the grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		void UndoStepTo(UndoableGrid grid);
	}
}
