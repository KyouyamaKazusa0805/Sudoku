using System;
using static Sudoku.Constants.Processings;

namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Encapsulates an assignment step.
	/// </summary>
	public sealed class AssignmentStep : Step
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="digit">The digit.</param>
		/// <param name="cell">The cell.</param>
		/// <param name="mask">The old mask to undo.</param>
		/// <param name="innerMap">
		/// The map which contains all cells that contains the digit.
		/// </param>
		public AssignmentStep(int digit, int cell, short mask, GridMap innerMap) =>
			(Digit, Cell, Mask, InnerMap) = (digit, cell, mask, innerMap);


		/// <summary>
		/// Indicates the digit.
		/// </summary>
		public int Digit { get; }

		/// <summary>
		/// Indicates the cell.
		/// </summary>
		public int Cell { get; }

		/// <summary>
		/// Indicates the mask of the cell.
		/// </summary>
		public short Mask { get; }

		/// <summary>
		/// Indicates the grid map.
		/// </summary>
		public GridMap InnerMap { get; }


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
			if (Digit >= 0 && Digit < 9)
			{
				grid._masks[Cell] = (short)((short)CellStatus.Modifiable << 9 | 511 & ~(1 << Digit));
				foreach (int cell in Peers[Cell])
				{
					if (grid.GetStatus(cell) != CellStatus.Empty)
					{
						continue;
					}

					grid._masks[cell] |= (short)(1 << Digit);
				}
			}
			else if (Digit == -1)
			{
				if (grid.GetStatus(Cell) == CellStatus.Modifiable)
				{
					Array.Copy(grid._initialMasks, grid._masks, 81);
				}
			}
		}
	}
}
