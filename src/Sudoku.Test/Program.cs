using System;

Console.WriteLine();

class Temp
{
	private readonly int _a = 1, _b = 2, _c = 3, _d = 4;
	private readonly int[]? _e = null;

	public void Deconstruct(out int a, out int b)
	{
		a = _a;
		b = _b;
	}

	public void Deconstruct(int a, int b, out int c)
	{
		a = _a;
		b = _b;
		c = _c;
	}

	public void Deconstruct(out int a, out int b, out int c, out int d, params int[]? e)
	{
		a = _a;
		b = _b;
		c = _c;
		d = _d;
		e = _e;
	}
}