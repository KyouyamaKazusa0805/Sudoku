using System;

var s = new S(1, 2, 3);
if (s is var (_, _, _) and var (_, _) and (10, 30, 50))
{
	Console.WriteLine(nameof(s));
}

_ = S.TryParse(string.Empty, out var _);
_ = S.TryParse(string.Empty, out _);

readonly struct S
{
	readonly int _a, _b, _c;

	public S(int a, int b, int c) { _a = a; _b = b; _c = c; }

	public void Deconstruct(out int a, out int b) { a = _a; b = _b; }
	public void Deconstruct(out int a, out int b, out int c) { a = _a; b = _b; c = _c; }

	public static bool TryParse(string s, out S result)
	{
		_ = s;
		result = new();
		return true;
	}
}
