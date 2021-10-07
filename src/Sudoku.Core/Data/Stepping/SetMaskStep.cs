namespace Sudoku.Data.Stepping;

/// <summary>
/// Encapsulates a step of setting mask.
/// </summary>
/// <param name="Cell">Indicates the cell.</param>
/// <param name="OldMask">Indicates the old mask.</param>
/// <param name="NewMask">Indicates the new mask.</param>
[Obsolete("In the future, this type won't be used.", false)]
public sealed record SetMaskStep(int Cell, short OldMask, short NewMask) : IStep
{
	/// <inheritdoc/>
	public unsafe void DoStepTo(UndoableGrid grid)
	{
		fixed (short* pGrid = grid)
		{
			pGrid[Cell] = NewMask;
		}
	}

	/// <inheritdoc/>
	public unsafe void UndoStepTo(UndoableGrid grid)
	{
		fixed (short* pGrid = grid)
		{
			pGrid[Cell] = OldMask;
		}
	}
}
