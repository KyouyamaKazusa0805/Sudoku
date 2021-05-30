using System;

var o = new R(1, 2D, 3F, "4");
if (o is { })
{
	Console.WriteLine(o);
}

record R(int A, double B, float C, string D);