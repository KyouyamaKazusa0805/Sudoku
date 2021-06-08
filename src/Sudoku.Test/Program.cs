using System;

var o = new R(1);
int p = 3;
if (o is R)
{
	Console.WriteLine(o.ToString());
}

if (p is int q)
{
	Console.WriteLine(q);
}

record R(int A);