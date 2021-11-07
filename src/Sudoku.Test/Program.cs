using System;

int[] array = { 1, 3, 6, 10, 15, 21, 28 };
foreach (ref int element in array.AsRefEnumerable())
{
	element++;
}

foreach (int element in array)
{
	Console.WriteLine(element);
}