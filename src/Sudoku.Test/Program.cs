using System;
using System.Runtime.CompilerServices;

string? s = "Hello";
ThrowIfNull(s);

static partial class Program
{
	private static void ThrowIfNull<T>(T? obj, [CallerArgumentExpression("obj")] string? paramName = null) where T : class
	{
		if (obj is null)
			throw new ArgumentNullException(paramName);
	}

	private static void ThrowIfNull(object? a, [CallerArgumentExpression("a")] __arglist)
	{

	}

	private static void ThrowIfNull(object? a, [CallerArgumentExpression("a")] object? p = null)
	{

	}
}