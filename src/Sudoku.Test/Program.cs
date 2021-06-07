using System;

object? o = null;

if (o is null)
{
	o = new();
}

Console.WriteLine(o);