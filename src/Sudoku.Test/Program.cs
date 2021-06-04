using System;

Console.WriteLine();

class Temp
{
	private readonly int _a = 1;

	public void Deconstruct(out int a, out string text)
	{
		a = _a;
		text = "";
	}
}