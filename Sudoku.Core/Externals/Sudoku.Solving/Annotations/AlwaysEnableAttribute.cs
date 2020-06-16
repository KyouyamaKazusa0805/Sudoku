using System;

namespace Sudoku.Solving.Annotations
{
	/// <summary>
	/// To mark this on a technique searcher to make the technique always enable.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class AlwaysEnableAttribute : Attribute
	{
	}
}
