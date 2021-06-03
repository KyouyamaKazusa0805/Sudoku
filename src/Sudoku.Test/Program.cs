using System;

Temp.Deconstruct(out _, out _);

Console.WriteLine();

class Temp
{
	public static void Deconstruct(out int a, out int b)
	{
		a = 10;
		b = 20;
	}
}