using System;

namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Encapsulates a reset step.
	/// </summary>
	/// <param name="OldMasks">Indicates the table of new grid masks.</param>
	/// <param name="NewMasks">Indicates the table of old grid masks.</param>
	public sealed unsafe record ResetStep(short* OldMasks, short* NewMasks) : Step
	{
		/// <inheritdoc/>
		public override void DoStepTo(UndoableGrid grid) =>
			SudokuGrid.InternalCopy(grid._innerGrid._values, OldMasks);

		/// <inheritdoc/>
		public override void UndoStepTo(UndoableGrid grid) =>
			SudokuGrid.InternalCopy(grid._innerGrid._values, NewMasks);
	}
}
