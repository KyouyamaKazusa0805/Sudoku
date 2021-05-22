using System;
using Sudoku.Data;

var s = new SudokuGrid();
Console.WriteLine(s.ToString("0"));


int[] arr = new int[10] { 3, 8, 1, 6, 5, 4, 7, 2, 9, 0 };
foreach (int element in arr)
{
	Console.WriteLine(element);
}


Console.WriteLine(nameof(TestEnum.A));

[Flags]
enum TestEnum
{
	A = 1,
	B = A,
	C = A + 1
}
