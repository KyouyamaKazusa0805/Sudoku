namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Encapsulates a step of setting mask.
	/// </summary>
	/// <param name="Cell">Indicates the cell.</param>
	/// <param name="OldMask">Indicates the old mask.</param>
	/// <param name="NewMask">Indicates the new mask.</param>
	public sealed record SetMaskStep(int Cell, short OldMask, short NewMask) : Step
	{
		/// <inheritdoc/>
		public override void DoStepTo(UndoableGrid grid) => grid._masks[Cell] = NewMask;

		/// <inheritdoc/>
		public override void UndoStepTo(UndoableGrid grid) => grid._masks[Cell] = OldMask;
	}
}
