using System;
using System.Collections.Generic;
using Sudoku.Diagnostics.CodeAnalysis;

F(new Inner());

static partial class Program
{
	static void F(IComparer<Element> comparer) => Console.WriteLine(comparer.GetType());

	private sealed class Element { }

	[AnonymousInnerType]
	private sealed class Inner : IComparer<Element>
	{
		public int Compare([IsDiscard] Element? x, [IsDiscard] Element? y) => throw new NotImplementedException();
	}
}