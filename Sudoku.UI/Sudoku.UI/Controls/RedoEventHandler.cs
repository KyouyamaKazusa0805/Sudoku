using Sudoku.Data;

namespace Sudoku.UI.Controls
{
	/// <summary>
	/// Indicates the event handler triggered when redid a step.
	/// </summary>
	/// <param name="oldGrid">(<see langword="in"/> parameter) The grid.</param>
	public delegate void RedoEventHandler(in SudokuGrid oldGrid);
}
