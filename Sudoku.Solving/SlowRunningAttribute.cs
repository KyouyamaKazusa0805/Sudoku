using System;

namespace Sudoku.Solving
{
	/// <summary>
	/// To mark on a technique searcher, it means that this technique searcher
	/// will have a slow speed to calculate.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class SlowAttribute : Attribute
	{
	}
}
