using static Sudoku.Constants.Processings;

namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Encapsulates an assignment step.
	/// </summary>
	/// <param name="Digit">The digit.</param>
	/// <param name="Cell">The cell.</param>
	/// <param name="Mask">The old mask to undo.</param>
	/// <param name="InnerMap">The map which contains all cells that contains the digit.</param>
	public sealed record AssignmentStep(int Digit, int Cell, short Mask, in GridMap InnerMap) : IStep
	{
		/// <inheritdoc/>
		public void UndoStepTo(UndoableGrid grid)
		{
			unsafe
			{
				grid._innerGrid._values[Cell] = Mask;

				foreach (int peerCell in InnerMap)
				{
					grid._innerGrid._values[peerCell] &= (short)~(1 << Digit);
				}
			}
		}

		/// <inheritdoc/>
		public void DoStepTo(UndoableGrid grid)
		{
			switch (Digit)
			{
				case -1 when grid.GetStatus(Cell) == CellStatus.Modifiable:
				{
					unsafe
					{
						fixed (short* pValues = grid._innerGrid._values)
						fixed (short* pInitialValues = grid._innerGrid._initialValues)
						{
							SudokuGrid.InternalCopy(pValues, pInitialValues);
						}
					}

					break;
				}
				case >= 0 and < 9:
				{
					unsafe
					{
						grid._innerGrid._values[Cell] =
							(short)(SudokuGrid.ModifiableMask | SudokuGrid.MaxCandidatesMask & ~(1 << Digit));
						foreach (int cell in Peers[Cell])
						{
							if (grid.GetStatus(cell) == CellStatus.Empty)
							{
								grid._innerGrid._values[cell] |= (short)(1 << Digit);
							}
						}
					}

					break;
				}
			}
		}
	}
}
