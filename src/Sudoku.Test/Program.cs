using System;
using Sudoku.Data;

var cells = new Cells { "r1c1", "r1c2", "r1c3", "r1c4", "r1c5" };
Console.WriteLine(cells.ToString());
foreach (int[] a in cells.SubsetOfSize(3))
{
	Console.WriteLine(new Cells(a).ToString());
}

static partial class Program
{
}