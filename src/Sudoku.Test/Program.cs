using System;

var s = new S(1, 2, 3);
if (s is () and (a: 10) and (a: 10, b: 30))
{
	Console.WriteLine(nameof(s));
}

if (s is (a: 1, b: 2, c: _))
{
	Console.WriteLine(nameof(s));
}

readonly struct S
{
	readonly int _a, _b, _c;

	public S(int a, int b, int c) : this() { _a = a; _b = b; _c = c; }

	public void Deconstruct() { }
	public void Deconstruct(out int a) { a = _a; }
	public void Deconstruct(out int a, out int b) { a = _a; b = _b; }
	public void Deconstruct(out int a, out int b, out int c) { a = _a; b = _b; c = _c; }
}
