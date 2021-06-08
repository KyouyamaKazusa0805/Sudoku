using System;

object? o = 3;

if (o is not null)
{
	Console.WriteLine(o);
}

if (o is object variable1)
{
	Console.WriteLine(variable1);
}

if (o is not null and var variable2)
{
	Console.WriteLine(variable2);
}

if (o is not null && o is var variable3) // This case can be converted to the above one via IDE0078.
{
	Console.WriteLine(variable3);
}