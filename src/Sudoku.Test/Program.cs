using System;
using System.Runtime.CompilerServices;
using System.Text;

Console.WriteLine(StringCatenatation($"{1}, {"2"}, {3F:N2}, {$"{4}"}"));

static partial class Program
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string StringCatenatation([InterpolatedStringHandlerArgument] StringHandler handler) =>
		handler.ToStringAndClear();
}