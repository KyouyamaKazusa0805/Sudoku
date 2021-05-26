using System;

var r = new R(1, 3, 5);
if (r.A == 4 && r.B == 4 && r.C == 4) // SS0606.
{
	Console.WriteLine(r);
}
if (r.A == 3) // Don't raise SS0606 because only one expression.
{
	Console.WriteLine(r);
}
if (!(r.A == 1 && r.B != 3)) // SS0606 (Logical not).
{
	Console.WriteLine(r);
}

record R(int A, int B, int C);