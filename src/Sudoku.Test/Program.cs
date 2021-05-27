using System;

int? i = 20;
if (i is null)
	i = 100;

double? j = 3D;
if (j == null)
	j = 100D;

object? o = new();
if (o is null)
{
	o = new();
}

Console.WriteLine(i);
Console.WriteLine(j);