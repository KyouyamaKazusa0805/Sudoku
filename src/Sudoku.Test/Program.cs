using System;

object o = new R(1, 2D, 3F, "4");
if (o is R { A: 1, B: _, C: 3F, D: _ })
{
	Console.WriteLine(o);
}

record R(int A, double B, float C, string D);