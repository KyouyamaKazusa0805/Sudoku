using System;
using Sudoku.Data;

var cells = new Cells { 0, 1, 2, 3, 4 };
Console.WriteLine(cells);

foreach (var set in cells & 3)
{
	Console.WriteLine(set);
}

static partial class Program
{
}
