namespace Sudoku.Data.Meta
{
	/// <summary>
	/// Encapsulates a step of setting mask.
	/// </summary>
	public sealed class SetMaskStep : Step
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <param name="oldMask">The old mask.</param>
		/// <param name="newMask">The new mask.</param>
		public SetMaskStep(int cell, short oldMask, short newMask) =>
			(Cell, OldMask, NewMask) = (cell, oldMask, newMask);


		/// <summary>
		/// Indicates the cell.
		/// </summary>
		public int Cell { get; }

		/// <summary>
		/// Indicates the old mask.
		/// </summary>
		public short OldMask { get; }

		/// <summary>
		/// Indicates the new mask.
		/// </summary>
		public short NewMask { get; }


		/// <inheritdoc/>
		public override void DoStepTo(UndoableGrid grid) => grid._masks[Cell] = NewMask;

		/// <inheritdoc/>
		public override void UndoStepTo(UndoableGrid grid) => grid._masks[Cell] = OldMask;
	}
}
