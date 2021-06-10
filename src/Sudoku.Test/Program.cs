using System;

var r = new S(1, 2, 3);
if (r is (a: 1, b: 2, c: _))
{
	Console.WriteLine(r.ToString());
}

readonly struct S
{
	private readonly int _a, _b, _c;


	public S(int a, int b, int c)
	{
		_a = a;
		_b = b;
		_c = c;
	}


	public void Deconstruct(out int a, out int b)
	{
		a = _a;
		b = _b;
	}

	public void Deconstruct(out int a, out int b, out int c)
	{
		a = _a;
		b = _b;
		c = _c;
	}

	public override string ToString() => (_a, _b, _c).ToString();
}