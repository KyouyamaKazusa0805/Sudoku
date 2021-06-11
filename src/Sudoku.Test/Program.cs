using System;

object o = new R(1, 2D, 3F, "4", null);

if (o is R { A: 1, B: _, C: 3F, D: _, Parent: { A: _, Parent: null } })
{
	Console.WriteLine(o);
}

record R(int A, double B, float C, string D, R? Parent);