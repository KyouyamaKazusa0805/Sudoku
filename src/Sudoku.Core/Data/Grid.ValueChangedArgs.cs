namespace Sudoku.Data;

partial struct Grid
{
	/// <summary>
	/// Provides arguments for the event <see cref="ValueChanged"/>.
	/// </summary>
	/// <param name="Cell">Indicates the cell, which must be in range 0 to 80.</param>
	/// <param name="OldMask">Indicates the original mask in this cell.</param>
	/// <param name="NewMask">Indicates the new mask to modify the cell.</param>
	/// <param name="SetValue">Indicates the set value. If the action is a deletion, the value should be -1.</param>
	/// <seealso cref="ValueChanged"/>
	public readonly partial record struct ValueChangedArgs(int Cell, short OldMask, short NewMask, int SetValue);
}
