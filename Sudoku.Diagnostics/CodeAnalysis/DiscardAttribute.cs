using System;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
	public sealed class DiscardAttribute : Attribute
	{
	}
}
