using System;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// To mark on a parameter, which means the parameter is always a discard.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
	public sealed class DiscardAttribute : Attribute
	{
	}
}
