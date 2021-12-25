using System;
using Sudoku.Data;

var cells = new Cells { "r1c1", "r1c2", "r1c4", "r1c6", "r1c9" };
Console.WriteLine(cells);

foreach (var set in cells.SubsetOfSize(3))
{
	Console.WriteLine(set);
}

static partial class Program
{
}
