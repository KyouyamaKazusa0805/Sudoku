namespace Sudoku.UI.CustomControls;

partial class SudokuPanel
{
	/// <summary>
	/// Initializes property values.
	/// </summary>
	/// <remarks><i>This method is only called by constructor.</i></remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void InitializeProperties() => GridCanvas = new(Preference, MainSudokuGrid);
}
