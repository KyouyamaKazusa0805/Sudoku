using System;
using Sudoku.Data;

var s = new SudokuGrid();
Console.WriteLine(s.ToString("0"));



int[] arr = new int[10] { 3, 8, 1, 6, 5, 4, 7, 2, 9, 0 };
for (int i = 0; i < arr.Length && i < args.Length; i++)
{
	Console.WriteLine(arr[i]);
}



Console.WriteLine(nameof(TestEnum.A));

[Flags]
enum TestEnum
{
	A = 1,
	B = A,
	C = A + 1
}
