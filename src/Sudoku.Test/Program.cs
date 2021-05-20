using System;
using Sudoku.Data;

#region Test item
var s = new SudokuGrid();
Console.WriteLine(s.ToString("0"));
#endregion

var p = new Cells { 1, 10, -9, -11, 20, 1, ~8 };
Console.WriteLine(p.ToString());

Console.WriteLine(nameof(TestEnum.A));

[Flags]
enum TestEnum
{
	A = 1,
	B = A,
	C = A + 1
}
