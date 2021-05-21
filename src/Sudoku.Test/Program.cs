using System;
using Sudoku.Data;

#region Test item
var s = new SudokuGrid();
Console.WriteLine(s.ToString("0"));
#endregion

var p = new Cells(stackalloc[] { 5, 10, 20, 40, 80 });
Console.WriteLine(p.ToString());

Console.WriteLine(nameof(TestEnum.A));

[Flags]
enum TestEnum
{
	A = 1,
	B = A,
	C = A + 1
}
