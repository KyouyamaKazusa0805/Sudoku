using Sudoku.Data;

namespace Sudoku.UI.Controls
{
	/// <summary>
	/// Indicates the event handler triggered when undid a step.
	/// </summary>
	/// <param name="newGrid">(<see langword="in"/> parameter) The grid.</param>
	public delegate void UndoEventHandler(in SudokuGrid newGrid);
}
