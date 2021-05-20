using System;
using Sudoku.Data;

var cells = new Cells { 1, 20, 40 };
Console.WriteLine(cells.Count == 0);

[Flags]
enum TestEnum
{
	A = 1,
	B = A,
	C = A + 1
}
