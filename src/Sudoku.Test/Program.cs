using System;

var s = new S(1, 2, 3);
if (s is (a: 1, b: 2, c: _))
{
	Console.WriteLine(nameof(s));
}

readonly struct S
{
	readonly int _a, _b, _c;
	readonly double _d;

	public S(int a, int b, int c) : this() { _a = a; _b = b; _c = c; }

	public void Deconstruct(out int a, out int b) { a = _a; b = _b; }
	public void Deconstruct(out int a, out double d) { a = _a; d = _d; }
	public void Deconstruct(out int a) { a = _a; }
	public void Deconstruct(out int a, out int b, out int c) { a = _a; b = _b; c = _c; }
}
