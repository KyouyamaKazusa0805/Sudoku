namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Encapsulates an anti-elimination step.
	/// </summary>
	/// <param name="Digit">The digit.</param>
	/// <param name="Cell">The cell.</param>
	public sealed record AntiEliminationStep(int Digit, int Cell) : IStep
	{
		/// <inheritdoc/>
		public void UndoStepTo(UndoableGrid grid)
		{
			unsafe
			{
				grid._innerGrid._values[Cell] |= (short)(1 << Digit);
			}
		}

		/// <inheritdoc/>
		public void DoStepTo(UndoableGrid grid)
		{
			unsafe
			{
				grid._innerGrid._values[Cell] &= (short)~(1 << Digit);
			}
		}
	}
}
