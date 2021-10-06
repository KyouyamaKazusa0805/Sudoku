#undef NESTED_ANNOTATION

namespace Sudoku.Data;

partial struct SudokuGrid
{
	/// <summary>
	/// Provides arguments for the event <see cref="ValueChanged"/>.
	/// </summary>
	/// <param name="Cell">The cell offset. Must be in range 0 to 80.</param>
	/// <param name="OldMask">The old mask before modified.</param>
	/// <param name="NewMask">The mask to modify the cell.</param>
	/// <param name="SetValue">The value. -1 when this value is not required.</param>
	/// <seealso cref="ValueChanged"/>
	public readonly record struct ValueChangedArgs(int Cell, short OldMask, short NewMask, int SetValue);
}
