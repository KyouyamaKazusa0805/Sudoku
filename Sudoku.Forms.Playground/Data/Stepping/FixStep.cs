namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Encapsulates a fix step.
	/// </summary>
	public sealed class FixStep : Step
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="allCells">All cells to fix.</param>
		public FixStep(GridMap allCells) => AllCells = allCells;


		/// <summary>
		/// Indicates all cells to fix.
		/// </summary>
		public GridMap AllCells { get; }


		/// <inheritdoc/>
		public override void DoStepTo(UndoableGrid grid)
		{
			foreach (int cell in AllCells.Offsets)
			{
				// To prevent the event re-invoke.
				ref short mask = ref grid._masks[cell];
				mask = (short)((int)CellStatus.Given << 9 | mask & 511);
			}
		}

		/// <inheritdoc/>
		public override void UndoStepTo(UndoableGrid grid)
		{
			foreach (int cell in AllCells.Offsets)
			{
				// To prevent the event re-invoke.
				ref short mask = ref grid._masks[cell];
				mask = (short)((int)CellStatus.Modifiable << 9 | mask & 511);
			}
		}
	}
}
