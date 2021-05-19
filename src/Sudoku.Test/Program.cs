using System;
using Sudoku.Data;

var s = new SudokuGrid();
Console.WriteLine(s.ToString("0"));

if (
	SudokuGrid.TryParse(
		"5.782...9.81.....2....716...93.12...............59.36...218....8.....71.1...642.8",
		out var grid
	) && grid == default(SudokuGrid)
)
{
	Console.WriteLine(grid);
}

[Flags]
enum TestEnum
{
	A = 1,
	B = A,
	C = A + 1
}
