using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

Console.WriteLine();

MyClass.Method(10, default);

abstract class MyClass
{
	[Regex]
	protected static readonly int P = 30;

	[Regex]
	protected static readonly string Q = @"\a";

	[Regex]
	protected static readonly string R = @"\w+";

	protected static readonly string S = @"\w+";


	public static void Method(int age, [Discard] double discardVariable)
	{
		age += 10;
		Console.WriteLine(age);
		Console.WriteLine(nameof(age));
		discardVariable -= 10;
		Console.WriteLine(discardVariable);
		Console.WriteLine(nameof(discardVariable));

#pragma warning disable IDE0039
		Action<int> f = static discardVariable => Console.WriteLine(discardVariable);
		f(10);

		Action<int> g = static delegate (int discardVariable) { Console.WriteLine(discardVariable); };
		g(20);
#pragma warning restore IDE0039

		static void h(double discardVariable)
		{
			discardVariable += 20;
			Console.WriteLine(discardVariable);
		}

		h(30);

		void i()
		{
			discardVariable += 20;
			Console.WriteLine(discardVariable);
		}

		i();
	}
}