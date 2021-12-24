#pragma warning disable IDE1006

using System;
using Sudoku.Diagnostics.CodeAnalysis;

int a = 10, b = 20;
unsafe
{
	f(null, null);
	f(&a, &a);
	f(&b, &a);
	f((void*)0, (void*)0);
	g(a, &b);
	h(&a);
}

static partial class Program
{
	static unsafe void f([Restrict] void* p, [Restrict] void* q)
	{
		Console.WriteLine((IntPtr)p);
		Console.WriteLine((IntPtr)q);
	}

	static unsafe void g([Restrict] int q, [Restrict] int* r)
	{
		Console.WriteLine(q);
		Console.WriteLine((IntPtr)r);
	}

	static unsafe void h([Restrict] int* r) => Console.WriteLine((IntPtr)r);
}