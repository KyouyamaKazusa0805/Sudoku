using System;
using Sudoku.Diagnostics.CodeAnalysis;

Console.WriteLine("Hello, world!");

f(400);

static void f([IsDiscard] int p)
{
	Console.WriteLine(nameof(p));
}