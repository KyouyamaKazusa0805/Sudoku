using System;

namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Encapsulates a reset step.
	/// </summary>
	/// <param name="OldMasks">Indicates the table of new grid masks.</param>
	/// <param name="NewMasks">Indicates the table of old grid masks.</param>
	public sealed record ResetStep(short[] OldMasks, short[] NewMasks) : Step
	{
		/// <inheritdoc/>
		public override void DoStepTo(UndoableGrid grid) => Array.Copy(OldMasks, grid._masks, 81);

		/// <inheritdoc/>
		public override void UndoStepTo(UndoableGrid grid) => Array.Copy(NewMasks, grid._masks, 81);
	}
}
