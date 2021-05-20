using System;
using Sudoku.Data;

var s = new SudokuGrid();
Console.WriteLine(s.ToString("0"));




var p = new Cells(stackalloc[] { 1, 10 });
var q = new Candidates(stackalloc[] { 1, 200 });
var r = new Cells(new[] { 0, 10 });
var t = new Candidates(new int[] { 3 });
var u = new Cells(stackalloc[] { 19 }) { 20 };
Console.WriteLine(p.ToString());
Console.WriteLine(q.ToString());
Console.WriteLine(r.ToString());
Console.WriteLine(t.ToString());






Console.WriteLine(nameof(TestEnum.A));

[Flags]
enum TestEnum
{
	A = 1,
	B = A,
	C = A + 1
}
