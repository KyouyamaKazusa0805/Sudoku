using System;
using Sudoku.Data;

var s = new SudokuGrid();
Console.WriteLine(s.ToString("0"));



var cells = new Cells { 1, 10, 100 };
Console.WriteLine(cells.ToString());

var candidates = new Candidates { 1, 10, 100, 1000 };
Console.WriteLine(candidates.ToString());

var cells2 = new Cells(cells) { ~107 };
Console.WriteLine(cells2.ToString());


Console.WriteLine(nameof(TestEnum.A));

[Flags]
enum TestEnum
{
	A = 1,
	B = A,
	C = A + 1
}
