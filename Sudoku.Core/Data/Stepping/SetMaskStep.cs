namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Encapsulates a step of setting mask.
	/// </summary>
	/// <param name="cell">Indicates the cell.</param>
	/// <param name="oldMask">Indicates the old mask.</param>
	/// <param name="newMask">Indicates the new mask.</param>
	public sealed record SetMaskStep(int Cell, short OldMask, short NewMask) : Step
	{
		/// <inheritdoc/>
		public override void DoStepTo(UndoableGrid grid) => grid._masks[Cell] = NewMask;

		/// <inheritdoc/>
		public override void UndoStepTo(UndoableGrid grid) => grid._masks[Cell] = OldMask;
	}
}
