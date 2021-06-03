using System;

var (_, _) = new Temp();
var (_, _, _) = new Temp();
var (_, _, _, _) = new Temp();

Console.WriteLine();

class Temp
{
	private readonly int _a = 1, _b = 2, _c = 3, _d = 4;

	private void Deconstruct(out int a, out int b)
	{
		a = _a;
		b = _b;
	}

	private protected void Deconstruct(out int a, out int b, out int c)
	{
		a = _a;
		b = _b;
		c = _c;
	}

	void Deconstruct(out int a, out int b, out int c, out int d)
	{
		a = _a;
		b = _b;
		c = _c;
		d = _d;
	}
}