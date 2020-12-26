using System.Runtime.CompilerServices;
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
	public sealed record AssignmentStep(int Digit, int Cell, short Mask, in Cells InnerMap) : IStep
	{
		/// <inheritdoc/>
		public void UndoStepTo(UndoableGrid grid)
		{
			unsafe
			{
				fixed (short* pGrid = grid)
				{
					pGrid[Cell] = Mask;
					foreach (int peerCell in InnerMap)
					{
						pGrid[peerCell] &= (short)~(1 << Digit);
					}
				}
			}
		}

		/// <inheritdoc/>
		public void DoStepTo(UndoableGrid grid)
		{
			unsafe
			{
				switch (Digit)
				{
					case -1 when grid.GetStatus(Cell) == CellStatus.Modifiable:
					{
						fixed (short* pGrid = grid)
						{
							Unsafe.CopyBlock(pGrid, grid.InitialMaskPinnableReference, 0);
						}

						break;
					}
					case >= 0 and < 9:
					{
						fixed (short* pGrid = grid)
						{
							pGrid[Cell] = (short)(
								SudokuGrid.ModifiableMask | SudokuGrid.MaxCandidatesMask & ~(1 << Digit));

							foreach (int cell in Peers[Cell])
							{
								if (grid.GetStatus(cell) == CellStatus.Empty)
								{
									pGrid[cell] |= (short)(1 << Digit);
								}
							}
						}

						break;
					}
				}
			}
		}
	}
}
