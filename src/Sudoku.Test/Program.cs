using System;

var s = new S { A = 1 };
R? p = null;
R o = new(1, 2D, 3F, "4");
_ = o is { };
_ = p is { };
_ = s is { };

record R(int A, double B, float C, string D);
readonly struct S
{
	public int A { get; init; }
}