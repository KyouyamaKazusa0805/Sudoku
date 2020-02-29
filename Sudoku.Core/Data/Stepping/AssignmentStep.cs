using Sudoku.Data.Meta;

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
			grid.SetMask(Cell, Mask);
			foreach (int peerCell in InnerMap.Offsets)
			{
				grid[peerCell, Digit] = false;
			}
		}

		/// <inheritdoc/>
		public override void DoStepTo(UndoableGrid grid) => grid[Cell] = Digit;
	}
}
