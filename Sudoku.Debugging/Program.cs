using Sudoku.Data;

unsafe
{
	var grid = SudokuGrid.Empty;
	SudokuGrid.ValueChanged(ref grid, new()); // SUDOKU014.
	SudokuGrid.RefreshingCandidates(ref grid); // SUDOKU014.
}
