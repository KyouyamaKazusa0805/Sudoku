namespace Sudoku.UI.CustomControls;

partial class SudokuPanel
{
	/// <summary>
	/// Initializes property values.
	/// </summary>
	/// <remarks><i>This method is only called by constructor.</i></remarks>
	private void InitializeProperties() => GridCanvas = new(MainSudokuGrid);
}
