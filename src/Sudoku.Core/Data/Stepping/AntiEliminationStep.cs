namespace Sudoku.Data.Stepping;

/// <summary>
/// Encapsulates an anti-elimination step.
/// </summary>
/// <param name="Digit">The digit.</param>
/// <param name="Cell">The cell.</param>
[Obsolete("In the future, this type won't be used.", false)]
public sealed record class AntiEliminationStep(int Digit, int Cell) : IStep
{
	/// <inheritdoc/>
	public unsafe void UndoStepTo(UndoableGrid grid)
	{
		fixed (short* pGrid = grid)
		{
			pGrid[Cell] |= (short)(1 << Digit);
		}
	}

	/// <inheritdoc/>
	public unsafe void DoStepTo(UndoableGrid grid)
	{
		fixed (short* pGrid = grid)
		{
			pGrid[Cell] &= (short)~(1 << Digit);
		}
	}
}
