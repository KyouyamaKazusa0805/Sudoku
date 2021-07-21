using System;
using System.Linq;
using System.Text.RegularExpressions;

Console.WriteLine();

int[] a = { 1, 2, 3, 4, 5, 6 };
var selection2 = from e in a select e + 1 into f where f is var q select f;

abstract class MyClass
{
	[Regex]
	protected static readonly int P = 30;

	[Regex]
	protected static readonly string Q = @"\a";

	[Regex]
	protected static readonly string R = @"\w+";

	protected static readonly string S = @"\w+";
}