namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Encapsulates an elimination step.
	/// </summary>
	/// <param name="Digit">Indicates the digit.</param>
	/// <param name="Cell">Indicates the cell.</param>
	public sealed record EliminationStep(int Digit, int Cell) : IStep
	{
		/// <inheritdoc/>
		public void UndoStepTo(UndoableGrid grid)
		{
			unsafe
			{
				fixed (short* pGrid = grid)
				{
					pGrid[Cell] &= (short)~(1 << Digit);
				}
			}
		}

		/// <inheritdoc/>
		public void DoStepTo(UndoableGrid grid)
		{
			unsafe
			{
				fixed (short* pGrid = grid)
				{
					pGrid[Cell] |= (short)(1 << Digit);
				}
			}
		}
	}
}
