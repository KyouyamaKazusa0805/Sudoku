using static Sudoku.Data.SudokuGrid;

namespace Sudoku.Data.Stepping;

/// <summary>
/// Encapsulates an unfix step.
/// </summary>
/// <param name="AllCells">Indicates all cells.</param>
[Obsolete("In the future, this type won't be used.", false)]
public sealed record class UnfixStep(in Cells AllCells) : IStep
{
	/// <inheritdoc/>
	public unsafe void DoStepTo(UndoableGrid grid)
	{
		fixed (short* pGrid = grid)
		{
			foreach (int cell in AllCells)
			{
				pGrid[cell] = (short)(ModifiableMask | pGrid[cell] & MaxCandidatesMask);
			}
		}
	}

	/// <inheritdoc/>
	public unsafe void UndoStepTo(UndoableGrid grid)
	{
		fixed (short* pGrid = grid)
		{
			foreach (int cell in AllCells)
			{
				pGrid[cell] = (short)(GivenMask | pGrid[cell] & MaxCandidatesMask);
			}
		}
	}
}
