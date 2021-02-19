namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Encapsulates an elimination step.
	/// </summary>
	/// <param name="Digit">Indicates the digit.</param>
	/// <param name="Cell">Indicates the cell.</param>
#if SUDOKU_UI
	[Obsolete("In the future, this type won't be used.", false)]
#endif
	public sealed record EliminationStep(int Digit, int Cell) : IStep
	{
		/// <inheritdoc/>
		public unsafe void UndoStepTo(UndoableGrid grid)
		{
			fixed (short* pGrid = grid)
			{
				pGrid[Cell] &= (short)~(1 << Digit);
			}
		}

		/// <inheritdoc/>
		public unsafe void DoStepTo(UndoableGrid grid)
		{
			fixed (short* pGrid = grid)
			{
				pGrid[Cell] |= (short)(1 << Digit);
			}
		}
	}
}
