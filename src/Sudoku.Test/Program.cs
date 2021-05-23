using System;
using Sudoku.Data;

var s = new SudokuGrid();
Console.WriteLine(s.ToString("0"));

int v = 20;
v += 10;
if (v is 30)
{
	Console.WriteLine(v);
}

if (v is not 100)
{
	Console.WriteLine(v);
}
