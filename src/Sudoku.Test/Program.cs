using System.Text.RegularExpressions;

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