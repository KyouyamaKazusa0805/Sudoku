using System;
using Sudoku.Diagnostics.CodeAnalysis;

Console.WriteLine("Hello, world!");

f(400);

EventHandler _ = ([IsDiscard] _, _) =>
{
	Console.WriteLine();
};

static void f([IsDiscard] int p, [IsDiscard] params int[] parameter)
{
	Console.WriteLine(p);
	Console.WriteLine(parameter[0]);
}