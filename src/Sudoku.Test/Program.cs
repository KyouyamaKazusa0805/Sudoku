using System;
using Sudoku.Data;

var s = new SudokuGrid();
Console.WriteLine(s.ToString("0"));

int? v = 20;
int a = 10;
const int b = 10;
if (v == a)
{
	Console.WriteLine(v);
}

if (v != b)
{
	Console.WriteLine(v);
}

if (v == 30)
{
	Console.WriteLine(v);
}

if (v != 100)
{
	Console.WriteLine(v);
}
