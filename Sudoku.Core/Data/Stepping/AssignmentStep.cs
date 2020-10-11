using System;
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
	public sealed record AssignmentStep(int Digit, int Cell, short Mask, GridMap InnerMap) : Step
	{
		/// <inheritdoc/>
		public override void UndoStepTo(UndoableGrid grid)
		{
			grid._masks[Cell] = Mask;

			foreach (int peerCell in InnerMap)
			{
				grid._masks[peerCell] &= (short)~(1 << Digit);
			}
		}

		/// <inheritdoc/>
		public override void DoStepTo(UndoableGrid grid)
		{
			switch (Digit)
			{
				case -1 when grid.GetStatus(Cell) == CellStatus.Modifiable:
				{
					Array.Copy(grid._initialMasks, grid._masks, 81);

					break;
				}
				case >= 0 and < 9:
				{
					grid._masks[Cell] = (short)((int)CellStatus.Modifiable << 9 | Grid.MaxCandidatesMask & ~(1 << Digit));
					foreach (int cell in Peers[Cell])
					{
						if (grid.GetStatus(cell) == CellStatus.Empty)
						{
							grid._masks[cell] |= (short)(1 << Digit);
						}
					}

					break;
				}
			}
		}
	}
}
