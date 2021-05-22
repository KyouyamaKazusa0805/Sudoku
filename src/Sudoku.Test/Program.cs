using System;
using Sudoku.Data;

var s = new SudokuGrid();
Console.WriteLine(s.ToString("0"));






Console.WriteLine(nameof(TestEnum.A));

[Flags]
enum TestEnum
{
	A = 1,
	B = A,
	C = A + 1
}
