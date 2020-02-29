using Sudoku.Data.Meta;

namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Encapsulates a step for setting a cell status.
	/// </summary>
	public sealed class SetCellStatusStep : Step
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <param name="oldStatus">The old status.</param>
		/// <param name="newStatus">The new status.</param>
		public SetCellStatusStep(int cell, CellStatus oldStatus, CellStatus newStatus) =>
			(Cell, OldStatus, NewStatus) = (cell, oldStatus, newStatus);


		/// <summary>
		/// Indicates the cell.
		/// </summary>
		public int Cell { get; }

		/// <summary>
		/// Indicates the old status.
		/// </summary>
		public CellStatus OldStatus { get; }

		/// <summary>
		/// Indicates the new status.
		/// </summary>
		public CellStatus NewStatus { get; }


		/// <inheritdoc/>
		public override void DoStepTo(UndoableGrid grid)
		{
			// To prevent the infinity recursion.
			ref short mask = ref grid._masks[Cell];
			mask = (short)((int)NewStatus << 9 | mask & 511);
		}

		/// <inheritdoc/>
		public override void UndoStepTo(UndoableGrid grid)
		{
			// To prevent the infinity recursion.
			ref short mask = ref grid._masks[Cell];
			mask = (short)((int)OldStatus << 9 | mask & 511);
		}
	}
}
