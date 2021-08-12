using System.Runtime.CompilerServices;
using static Sudoku.Constants.Tables;
using static Sudoku.Data.SudokuGrid;

namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Encapsulates an assignment step.
	/// </summary>
	/// <param name="Cell">The current cell to fill with a digit.</param>
	/// <param name="Digit">The current digit to assign.</param>
	/// <param name="Snapshot">The grid snapshot.</param>
	[Obsolete("In the future, this type won't be used.", false)]
	public sealed record AssignmentStep(int Cell, int Digit, in SudokuGrid Snapshot) : IStep
	{
		/// <inheritdoc/>
		public unsafe void UndoStepTo(UndoableGrid grid)
		{
			fixed (short* pGrid = grid, pTemp = Snapshot)
			{
				Unsafe.CopyBlock(pGrid, pTemp, sizeof(short) * 81);
			}
		}

		/// <inheritdoc/>
		public unsafe void DoStepTo(UndoableGrid grid)
		{
			switch (Digit)
			{
				case -1 when grid.GetStatus(Cell) == CellStatus.Modifiable:
				{
					fixed (short* pGrid = grid, p = &grid.GetPinnableReference(PinnedItem.InitialGrid))
					{
						Unsafe.CopyBlock(pGrid, p, sizeof(short) * 81);
					}

					break;
				}
				case >= 0 and < 9:
				{
					fixed (short* pGrid = grid)
					{
						pGrid[Cell] = (short)(ModifiableMask | MaxCandidatesMask & ~(1 << Digit));

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
