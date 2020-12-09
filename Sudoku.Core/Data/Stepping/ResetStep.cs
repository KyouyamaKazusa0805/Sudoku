using System;

namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Encapsulates a reset step.
	/// </summary>
	/// <param name="OldMasks">Indicates the table of new grid masks.</param>
	/// <param name="NewMasks">Indicates the table of old grid masks.</param>
	/// <remarks>
	/// Note that <see langword="record"/>s can't support pointers at present. Therefore,
	/// I use <see cref="IntPtr"/> instead.
	/// </remarks>
	/// <seealso cref="IntPtr"/>
	public sealed record ResetStep(IntPtr OldMasks, IntPtr NewMasks) : IStep
	{
		/// <inheritdoc/>
		public void DoStepTo(UndoableGrid grid)
		{
			unsafe
			{
				fixed (short* pGrid = grid)
				{
					SudokuGrid.InternalCopy(pGrid, (short*)OldMasks);
				}
			}
		}

		/// <inheritdoc/>
		public void UndoStepTo(UndoableGrid grid)
		{
			unsafe
			{
				fixed (short* pGrid = grid)
				{
					SudokuGrid.InternalCopy(pGrid, (short*)NewMasks);
				}
			}
		}
	}
}
