namespace Sudoku.Data.Stepping;

/// <summary>
/// Encapsulates a step for setting a cell status.
/// </summary>
/// <param name="Cell">The cell.</param>
/// <param name="OldStatus">The old status.</param>
/// <param name="NewStatus">The new status.</param>
[Obsolete("In the future, this type won't be used.", false)]
public sealed record SetStatusStep(int Cell, CellStatus OldStatus, CellStatus NewStatus) : IStep
{
	/// <inheritdoc/>
	public unsafe void DoStepTo(UndoableGrid grid)
	{
		fixed (short* pGrid = grid)
		{
			pGrid[Cell] = (short)((int)NewStatus << 9 | pGrid[Cell] & SudokuGrid.MaxCandidatesMask);
		}
	}

	/// <inheritdoc/>
	public unsafe void UndoStepTo(UndoableGrid grid)
	{
		fixed (short* pGrid = grid)
		{
			pGrid[Cell] = (short)((int)OldStatus << 9 | pGrid[Cell] & SudokuGrid.MaxCandidatesMask);
		}
	}
}
