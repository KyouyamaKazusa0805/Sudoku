using System;

var v1 = new S(1, 2);
if (v1 is (_, _))
{
	Console.WriteLine(nameof(v1));
}

var v2 = new S(1, 2);
if (v2 is (_, _) v5)
{
	Console.WriteLine(nameof(v5));
}

R? v3 = null;
if (v3 is (_, _) v6)
{
	Console.WriteLine(nameof(v6));
}

T? v4 = null;
if (v4 is (_, _))
{
	Console.WriteLine(nameof(v4));
}

readonly struct S
{
	private readonly int _a, _b;

	public S(int a, int b) { _a = a; _b = b; }

	public void Deconstruct(out int a, out int b) { a = _a; b = _b; }
}

record R(int A, int B);
record T(int A)
{
	public int B { get; set; }

	public void Deconstruct(out int a, out int b) { a = A; b = B; }
}