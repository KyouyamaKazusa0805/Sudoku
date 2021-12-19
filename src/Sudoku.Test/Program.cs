using System;
using System.Runtime.CompilerServices;

string? s = "Hello";
ThrowIfNull(s);
ThrowIfNull(s, "");

static partial class Program
{
	private static void ThrowIfNull<T>(T? obj, [CallerArgumentExpression("obj")] string? paramName = null) where T : class
	{
		if (obj is null)
			throw new ArgumentNullException(paramName);
	}
}