namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Encapsulates a step of setting mask.
	/// </summary>
	/// <param name="Cell">Indicates the cell.</param>
	/// <param name="OldMask">Indicates the old mask.</param>
	/// <param name="NewMask">Indicates the new mask.</param>
	public sealed record SetMaskStep(int Cell, short OldMask, short NewMask) : IStep
	{
		/// <inheritdoc/>
		public void DoStepTo(UndoableGrid grid)
		{
			unsafe
			{
				grid._innerGrid._values[Cell] = NewMask;
			}
		}

		/// <inheritdoc/>
		public void UndoStepTo(UndoableGrid grid)
		{
			unsafe
			{
				grid._innerGrid._values[Cell] = OldMask;
			}
		}
	}
}
