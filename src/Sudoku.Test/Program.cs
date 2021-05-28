using System;

var s = new S(1, 2, 3);
if (s is (_, _) and (10, 30, 50))
{
	Console.WriteLine(nameof(s));
}

readonly struct S
{
	readonly int _a, _b, _c, _d;

	public S(int a, int b, int c) : this() { _a = a; _b = b; _c = c; }

	public void Deconstruct(out int a, out int b) { a = _a; b = _b; }
	public void Deconstruct(out int a, out int b, out int c) { a = _a; b = _b; c = _c; }
	public void Deconstruct(out int a, out int b, out int c, out int d) { a = _a; b = _b; c = _c; d = _d; }
}
