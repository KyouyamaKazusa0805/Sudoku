using System;

var s = new S(1, 2, 3, 4);
if (s is (a: 10, b: _) and (a: 100, b: 30, c: 50))
{
	Console.WriteLine(nameof(s));
}

readonly struct S
{
	private readonly int _a, _b, _c, _d;

	public S(int a, int b, int c, int d) { _a = a; _b = b; _c = c; _d = d; }

	public void Deconstruct(out int a, out int b) { a = _a; b = _b; }
	public void Deconstruct(out int a, out int b, out int c) { a = _a; b = _b; c = _c; }
	public void Deconstruct(out int a, out int b, out int c, out int d) { a = _a; b = _b; c = _c; d = _d; }
}