using System;
using Sudoku.Data;

#region Test item
var s = new SudokuGrid();
Console.WriteLine(s.ToString("0"));
#endregion

int[] arr = { 3, 8, 1, 6, 5, 4, 7, 2, 9, 0 };
for (int i = 0; i < arr.Length; i++)
{
	Console.WriteLine(i);
}

Console.WriteLine(nameof(TestEnum.A));

[Flags]
enum TestEnum
{
	A = 1,
	B = A,
	C = A + 1
}
