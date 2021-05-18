using System;
using System.Diagnostics.CodeAnalysis;
using Sudoku.Data;

if (
	SudokuGrid.TryParse(
		"5.782...9.81.....2....716...93.12...............59.36...218....8.....71.1...642.8",
		out var grid
	) && grid == default(SudokuGrid)
)
{
	Console.WriteLine(grid);
}

p();

static void p(SudokuGrid s = default, SudokuGrid q = new(), SudokuGrid r = new())
{
	Console.WriteLine(s);
	Console.WriteLine(q);
	Console.WriteLine(r);
}


for (var f = TestEnum.A; f < TestEnum.C; f++)
{
	Console.WriteLine(f);
}

[Closed]
enum TestEnum { A, B, C }
