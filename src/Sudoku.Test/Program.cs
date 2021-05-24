using System;

var r = new R(1, 3);
if (r._a == 4 && r._b == 4)
{
	Console.WriteLine(r);
}
if (r is (a: 4, b: 4))
{
	Console.WriteLine(r);
}
if (r._a == 1 && r._b == 3)
{
	Console.WriteLine(r);
}
if (r is (a: 1, b: 3, sum: 4, product: 3))
{
	Console.WriteLine(r);
}

static class REx
{
	public static void Deconstruct(this in R @this, out int a, out int b, out int sum, out int product)
	{
		a = @this._a;
		b = @this._b;
		sum = a + b;
		product = a * b;
	}
}

readonly struct R
{
	internal readonly int _a, _b;

	public R(int a, int b)
	{
		_a = a;
		_b = b;
	}

	public readonly void Deconstruct(out int a, out int b)
	{
		a = _a;
		b = _b;
	}

	public readonly override string ToString() => (_a + _b).ToString();
}