using System;

var r = new R(1, 3, 5);
if (r._a == 4 && r._b == 4 && r._c == 4) // SS0606.
{
	Console.WriteLine(r);
}
if (r._a == 3) // SS0606 (Maybe use discard).
{
	Console.WriteLine(r);
}
if (!(r._a == 1 && r._b != 3)) // SS0606 (Logical not).
{
	Console.WriteLine(r);
}

readonly struct R
{
	internal readonly int _a, _b, _c;

	public R(int a, int b, int c) { _a = a; _b = b; _c = c; }

	public readonly void Deconstruct(out int a, out int b, out int c) { a = _a; b = _b; c = _c; }
	public readonly override string ToString() => (_a + _b + _c).ToString();
}